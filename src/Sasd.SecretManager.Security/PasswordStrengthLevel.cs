namespace Sasd.SecretManager.Security;

/// <summary>
/// Grobe Einordnung der Qualität eines Master-Passworts.
/// Die Skala soll keine absolute Sicherheit versprechen,
/// sondern nur eine praktische UX-Hilfe darstellen.
/// </summary>
public enum PasswordStrengthLevel
{
    VeryWeak = 0,
    Weak = 1,
    Moderate = 2,
    Good = 3,
    Strong = 4,
}
