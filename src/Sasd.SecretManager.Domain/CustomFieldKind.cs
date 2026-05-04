namespace Sasd.SecretManager.Domain;

/// <summary>
/// Beschreibt die erwartete Art eines frei definierbaren Zusatzfelds.
/// </summary>
public enum CustomFieldKind
{
    Text = 0,
    Secret = 1,
    Url = 2,
    Hostname = 3,
    Port = 4,
    Email = 5,
    Date = 6,
    Number = 7,
    Boolean = 8,
}
