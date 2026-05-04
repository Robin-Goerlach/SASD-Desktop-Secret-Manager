using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Sasd.SecretManager.Domain;
using Sasd.SecretManager.Security;

namespace Sasd.SecretManager.Storage;

/// <summary>
/// Implementiert das interne .svault-Dateiformat für Milestone 4.
/// Gespeichert wird ein verschlüsselter JSON-Tresorcontainer.
/// </summary>
public sealed class VaultFileRepository : IVaultRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
    };

    private readonly VaultKeyDerivationService _keyDerivationService = new();
    private readonly VaultEncryptionService _encryptionService = new();

    public async Task<SecretVault> LoadAsync(string filePath, string masterPassword, CancellationToken cancellationToken = default)
    {
        ValidateFilePath(filePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(masterPassword);

        if (!File.Exists(filePath))
        {
            throw new VaultStorageException($"Die Tresordatei wurde nicht gefunden: {filePath}");
        }

        try
        {
            await using var stream = File.OpenRead(filePath);
            var container = await JsonSerializer.DeserializeAsync<VaultFileContainer>(stream, JsonOptions, cancellationToken)
                ?? throw new VaultStorageException("Die Tresordatei konnte nicht gelesen werden.");

            ValidateContainer(container);

            var salt = Convert.FromBase64String(container.SaltBase64);
            var nonce = Convert.FromBase64String(container.NonceBase64);
            var tag = Convert.FromBase64String(container.TagBase64);
            var cipherBytes = Convert.FromBase64String(container.CipherTextBase64);

            var key = _keyDerivationService.DeriveKey(masterPassword, salt, container.Iterations);
            var plainBytes = _encryptionService.Decrypt(cipherBytes, key, nonce, tag);

            var vault = JsonSerializer.Deserialize<SecretVault>(plainBytes, JsonOptions)
                ?? throw new VaultStorageException("Der entschlüsselte Tresorinhalt ist ungültig.");

            NormalizeVault(vault);
            return vault;
        }
        catch (VaultStorageException)
        {
            throw;
        }
        catch (CryptographicException exception)
        {
            throw new VaultStorageException("Tresor konnte nicht entschlüsselt werden. Master-Passwort oder Dateiinhalt sind ungültig.", exception);
        }
        catch (JsonException exception)
        {
            throw new VaultStorageException("Tresordatei besitzt kein gültiges internes Format.", exception);
        }
        catch (Exception exception)
        {
            throw new VaultStorageException("Beim Laden des Tresors ist ein unerwarteter Fehler aufgetreten.", exception);
        }
    }

    public async Task SaveAsync(SecretVault vault, string filePath, string masterPassword, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(vault);
        ValidateFilePath(filePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(masterPassword);

        try
        {
            var salt = _keyDerivationService.CreateRandomSalt();
            var iterations = VaultKeyDerivationService.DefaultIterations;
            var key = _keyDerivationService.DeriveKey(masterPassword, salt, iterations);
            var nonce = _encryptionService.CreateRandomNonce();

            var plainBytes = JsonSerializer.SerializeToUtf8Bytes(vault, JsonOptions);
            var cipherBytes = _encryptionService.Encrypt(plainBytes, key, nonce, out var tag);

            var container = new VaultFileContainer
            {
                Header = new VaultFileHeader(),
                Iterations = iterations,
                SaltBase64 = Convert.ToBase64String(salt),
                NonceBase64 = Convert.ToBase64String(nonce),
                TagBase64 = Convert.ToBase64String(tag),
                CipherTextBase64 = Convert.ToBase64String(cipherBytes),
                SavedUtc = DateTimeOffset.UtcNow,
            };

            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var tempFilePath = filePath + ".tmp";
            await using (var stream = File.Create(tempFilePath))
            {
                await JsonSerializer.SerializeAsync(stream, container, JsonOptions, cancellationToken);
                await stream.FlushAsync(cancellationToken);
            }

            CreateBackupIfNeeded(filePath);
            File.Move(tempFilePath, filePath, true);
        }
        catch (VaultStorageException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new VaultStorageException("Beim Speichern des Tresors ist ein unerwarteter Fehler aufgetreten.", exception);
        }
    }

    private static void CreateBackupIfNeeded(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return;
        }

        var backupPath = filePath + ".bak";
        File.Copy(filePath, backupPath, true);
    }

    private static void ValidateContainer(VaultFileContainer container)
    {
        if (!string.Equals(container.Header.Magic, VaultFileConstants.Magic, StringComparison.Ordinal))
        {
            throw new VaultStorageException("Datei ist kein gültiger .svault-Tresor.");
        }

        if (container.Header.FormatVersion != VaultFileConstants.CurrentFormatVersion)
        {
            throw new VaultStorageException($"Nicht unterstützte Tresorformat-Version: {container.Header.FormatVersion}");
        }

        if (container.Iterations <= 0)
        {
            throw new VaultStorageException("Ungültige KDF-Iterationszahl im Tresorcontainer.");
        }
    }

    private static void NormalizeVault(SecretVault vault)
    {
        vault.Groups ??= [];
        vault.Entries ??= [];
        vault.KnownTags ??= [];

        foreach (var entry in vault.Entries)
        {
            entry.Tags ??= [];
            entry.CustomFields ??= [];
        }
    }

    private static void ValidateFilePath(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        if (!filePath.EndsWith(".svault", StringComparison.OrdinalIgnoreCase))
        {
            throw new VaultStorageException("Interne Tresore müssen die Dateiendung .svault verwenden.");
        }
    }
}
