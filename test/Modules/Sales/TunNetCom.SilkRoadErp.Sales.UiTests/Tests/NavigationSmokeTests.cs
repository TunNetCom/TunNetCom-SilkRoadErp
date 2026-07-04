using FluentAssertions;
using Microsoft.Playwright;
using TunNetCom.SilkRoadErp.Sales.UiTests.Infrastructure;

namespace TunNetCom.SilkRoadErp.Sales.UiTests.Tests;

/// <summary>
/// LIST-01..10: every main list page renders after login — no auth bounce,
/// no Blazor error overlay, and (for grid pages) a Radzen data grid appears.
/// Catches broken pages cheaply after any refactor.
/// </summary>
[Collection(UiTestsCollection.Name)]
public class NavigationSmokeTests(UiTestsFixture fixture)
{
    private const float TimeoutMs = 30_000;

    [Theory]
    [InlineData("/customers_list", true)]      // LIST-01
    [InlineData("/products_list", true)]       // LIST-02
    [InlineData("/providers_list", true)]      // LIST-03
    [InlineData("/orders", true)]              // LIST-04
    [InlineData("/invoices", true)]            // LIST-05
    [InlineData("/delivery-notes", true)]      // LIST-06
    [InlineData("/quotations", true)]          // LIST-07
    [InlineData("/dashboard", false)]          // LIST-08 (KPI cards, not a grid)
    [InlineData("/paiements-client", true)]    // LIST-09
    [InlineData("/accounting-years", true)]    // LIST-10
    public async Task ListPage_Renders_WithoutError(string route, bool expectsGrid)
    {
        var page = await fixture.NewAuthenticatedPageAsync();

        await page.GotoAsync(route);

        if (expectsGrid)
        {
            // Radzen grid rendered — implies the page loaded and its API call succeeded.
            await Assertions.Expect(page.Locator(".rz-data-grid, .rz-datatable").First)
                .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = TimeoutMs });
        }
        else
        {
            // Non-grid page: the main layout body must render.
            await Assertions.Expect(page.Locator(".rz-body"))
                .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = TimeoutMs });
        }

        // Not bounced to login, and Blazor's error overlay is not shown.
        page.Url.Should().NotContain("/login");
        await Assertions.Expect(page.Locator("#blazor-error-ui")).ToBeHiddenAsync();

        await page.Context.DisposeAsync();
    }
}
