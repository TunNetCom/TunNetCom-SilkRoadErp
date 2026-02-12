using Microsoft.EntityFrameworkCore;
using System.Text;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Tiers.ExportTiers;

public class ExportTiersToTxtQueryHandler(
    SalesContext context,
    ILogger<ExportTiersToTxtQueryHandler> logger)
    : IRequestHandler<ExportTiersToTxtQuery, byte[]>
{
    public async Task<byte[]> Handle(ExportTiersToTxtQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting Tiers export to text file (Type: {Type})", request.Type?.ToString() ?? "All");

        var stringBuilder = new StringBuilder();

        // Handle Clients
        if (request.Type == null || request.Type == TierType.Client)
        {
            var clients = await context.Client
                .AsNoTracking()
                .OrderBy(c => c.Id)
                .ToListAsync(cancellationToken);

            logger.LogInformation("Found {Count} clients to export", clients.Count);

            foreach (var client in clients)
            {
                // Strict requirement: C + Id
                var code = $"C{client.Id}";
                var line = FormatTierLine(code, client.Nom, "041100000");
                stringBuilder.AppendLine(line);
            }
        }

        // Handle Suppliers (Fournisseurs)
        if (request.Type == null || request.Type == TierType.Supplier)
        {
            var fournisseurs = await context.Fournisseur
                .AsNoTracking()
                .OrderBy(f => f.Id)
                .ToListAsync(cancellationToken);

            logger.LogInformation("Found {Count} suppliers to export", fournisseurs.Count);

            foreach (var fournisseur in fournisseurs)
            {
                // Strict requirement: F + Id
                var code = $"F{fournisseur.Id}";
                var line = FormatTierLine(code, fournisseur.Nom, "140100000");
                stringBuilder.AppendLine(line);
            }
        }

        logger.LogInformation("Tiers export completed.");

        return Encoding.UTF8.GetBytes(stringBuilder.ToString());
    }

    private static string FormatTierLine(string code, string name, string accountingCode)
    {
        // Format: Code (17 chars) + Name (35 chars) + Accounting Code (14 chars)
        // Based on the Tiers.txt example format
        var formattedCode = code.PadRight(17);
        var formattedName = name.Length > 35 ? name[..35] : name.PadRight(35);
        var formattedAccountingCode = accountingCode.PadRight(14);

        return $"{formattedCode}{formattedName}{formattedAccountingCode}";
    }
}
