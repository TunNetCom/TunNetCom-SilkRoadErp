using Microsoft.Playwright;

namespace TunNetCom.SilkRoadErp.Sales.UiTests.Pages;

/// <summary>Page Object for /login (Components/Pages/Login/LoginPage.razor).</summary>
public sealed class LoginPage(IPage page)
{
    public IPage Page { get; } = page;

    public ILocator UsernameInput => Page.GetByPlaceholder("Enter your username");
    public ILocator PasswordInput => Page.GetByPlaceholder("Enter your password");
    public ILocator SignInButton => Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "SIGN IN" });
    public ILocator ErrorAlert => Page.Locator(".error-alert");

    public async Task GotoAsync() => await Page.GotoAsync("/login");

    public async Task LoginAsync(string username, string password)
    {
        await UsernameInput.FillAsync(username);
        await PasswordInput.FillAsync(password);
        await SignInButton.ClickAsync();
    }
}
