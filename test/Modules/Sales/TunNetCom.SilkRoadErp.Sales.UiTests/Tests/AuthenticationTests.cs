using FluentAssertions;
using Microsoft.Playwright;
using TunNetCom.SilkRoadErp.Sales.UiTests.Infrastructure;
using TunNetCom.SilkRoadErp.Sales.UiTests.Pages;

namespace TunNetCom.SilkRoadErp.Sales.UiTests.Tests;

[Collection(UiTestsCollection.Name)]
public class AuthenticationTests(UiTestsFixture fixture)
{
    private const float NavigationTimeoutMs = 30_000;

    [Fact]
    public async Task ProtectedPage_WithoutLogin_RedirectsToLogin()
    {
        var page = await fixture.NewPageAsync();

        await page.GotoAsync("/");

        await page.WaitForURLAsync(
            url => url.Contains("/login"),
            new PageWaitForURLOptions { Timeout = NavigationTimeoutMs });

        page.Url.Should().Contain("/login");
        await page.Context.DisposeAsync();
    }

    [Fact]
    public async Task Login_WithValidCredentials_LeavesLoginPage()
    {
        var page = await fixture.NewPageAsync();
        var loginPage = new LoginPage(page);

        await loginPage.GotoAsync();
        await loginPage.LoginAsync(UiTestsFixture.AdminUsername, UiTestsFixture.AdminPassword);

        await page.WaitForURLAsync(
            url => !url.Contains("/login"),
            new PageWaitForURLOptions { Timeout = NavigationTimeoutMs });

        page.Url.Should().NotContain("/login");
        await page.Context.DisposeAsync();
    }

    // AUTH-04
    [Fact]
    public async Task Login_WithReturnUrl_RedirectsBackToRequestedPage()
    {
        var page = await fixture.NewPageAsync();

        await page.GotoAsync("/invoices");
        await page.WaitForURLAsync(
            url => url.Contains("/login"),
            new PageWaitForURLOptions { Timeout = NavigationTimeoutMs });

        var loginPage = new LoginPage(page);
        await loginPage.LoginAsync(UiTestsFixture.AdminUsername, UiTestsFixture.AdminPassword);

        await page.WaitForURLAsync(
            url => url.Contains("/invoices"),
            new PageWaitForURLOptions { Timeout = NavigationTimeoutMs });

        page.Url.Should().Contain("/invoices");
        await page.Context.DisposeAsync();
    }

    // AUTH-05
    [Fact]
    public async Task Logout_RedirectsToLogin_AndClearsSession()
    {
        var page = await fixture.NewAuthenticatedPageAsync();

        // Logout lives in the sidebar: "Administration" panel menu → "Déconnexion".
        var sidebar = page.Locator(".rz-panel-menu");
        await sidebar.GetByText("Administration", new LocatorGetByTextOptions { Exact = true }).ClickAsync();
        await sidebar.GetByText("Déconnexion").ClickAsync();

        await page.WaitForURLAsync(
            url => url.Contains("/login"),
            new PageWaitForURLOptions { Timeout = NavigationTimeoutMs });

        // Session is really gone: a protected page bounces back to login.
        await page.GotoAsync("/");
        await page.WaitForURLAsync(
            url => url.Contains("/login"),
            new PageWaitForURLOptions { Timeout = NavigationTimeoutMs });

        page.Url.Should().Contain("/login");
        await page.Context.DisposeAsync();
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShowsErrorMessage()
    {
        var page = await fixture.NewPageAsync();
        var loginPage = new LoginPage(page);

        await loginPage.GotoAsync();
        await loginPage.LoginAsync(UiTestsFixture.AdminUsername, "WrongPassword!");

        await Assertions.Expect(loginPage.ErrorAlert)
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = NavigationTimeoutMs });

        page.Url.Should().Contain("/login");
        await page.Context.DisposeAsync();
    }
}
