using System.Xml.Schema;
using System.Xml;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;

/// <summary>
/// Validates TEJ DeclarationsRS XML against the official XSD schema (TEJDeclarationRS_v1.0.xsd).
/// </summary>
public static class TejXsdValidator
{
    private const string SchemaFileName = "TEJDeclarationRS_v1.0.xsd";

    /// <summary>
    /// Validates the XML bytes against the TEJ schema.
    /// </summary>
    /// <param name="xmlBytes">UTF-8 XML content.</param>
    /// <param name="schemasDirectory">Directory containing the XSD files (default: Schemas under app base).</param>
    /// <returns>List of validation error messages; empty if valid.</returns>
    public static IReadOnlyList<string> Validate(byte[] xmlBytes, string? schemasDirectory = null)
    {
        var errors = new List<string>();
        schemasDirectory ??= Path.Combine(AppContext.BaseDirectory, "Schemas");
        var schemaPath = Path.Combine(schemasDirectory, SchemaFileName);

        if (!File.Exists(schemaPath))
        {
            errors.Add($"Schéma TEJ introuvable: {schemaPath}. La validation XSD est ignorée.");
            return errors;
        }

        try
        {
            var schemaSet = new XmlSchemaSet();
            using (var reader = XmlReader.Create(schemaPath, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit }))
            {
                schemaSet.Add(null, reader);
            }
            schemaSet.Compile();

            var settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema,
                Schemas = schemaSet,
                DtdProcessing = DtdProcessing.Prohibit
            };
            settings.ValidationEventHandler += (_, e) =>
            {
                if (e.Severity == XmlSeverityType.Error)
                    errors.Add($"{e.Message} (Ligne {e.Exception?.LineNumber ?? 0})");
            };

            using var xmlStream = new MemoryStream(xmlBytes);
            using var xmlReader = XmlReader.Create(xmlStream, settings);
            while (xmlReader.Read()) { }
        }
        catch (Exception ex)
        {
            errors.Add($"Erreur lors de la validation XSD: {ex.Message}");
        }

        return errors;
    }
}
