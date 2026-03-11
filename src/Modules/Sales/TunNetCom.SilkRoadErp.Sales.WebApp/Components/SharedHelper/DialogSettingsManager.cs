using Microsoft.JSInterop;
using System.Text.Json;
using static TunNetCom.SilkRoadErp.Sales.WebApp.Components.Pages.ReciptionNotes.AddOrUpdateRecipietNote;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Components.SharedHelper;

public static class DialogSettingsManager
{
    public static async Task<DialogSettings> LoadSettingsAsync(IJSRuntime jsRuntime)
    {
        var result = await jsRuntime.InvokeAsync<string>("window.localStorage.getItem", "DialogSettings");
        return string.IsNullOrEmpty(result)
            ? new DialogSettings()
            : JsonSerializer.Deserialize<DialogSettings>(result) ?? new DialogSettings();
    }

    public static async Task SaveSettingsAsync(IJSRuntime jsRuntime, DialogSettings settings)
    {
        await jsRuntime.InvokeVoidAsync("window.localStorage.setItem", "DialogSettings",
            JsonSerializer.Serialize(settings));
    }

    public static DialogOptions CreateDialogOptions(
        DialogSettings settings,
        Action<System.Drawing.Size> onResize,
        Action<System.Drawing.Point> onDrag)
    {
        return new DialogOptions
        {
            Resizable = true,
            Draggable = true,
            Resize = onResize,
            Drag = onDrag,
            Width = settings?.Width ?? "700px",
            Height = settings?.Height ?? "512px",
            Left = settings?.Left,
            Top = settings?.Top
        };
    }

    public static void OnDrag(IJSRuntime jsRuntime, System.Drawing.Point point, ref DialogSettings settings)
    {
        _ = jsRuntime.InvokeVoidAsync("eval", $"console.log('Dialog drag. Left:{point.X}, Top:{point.Y}')");
        settings ??= new DialogSettings();
        settings.Left = $"{point.X}px";
        settings.Top = $"{point.Y}px";
        _ = SaveSettingsAsync(jsRuntime, settings);
    }

    public static void OnResize(IJSRuntime jsRuntime, System.Drawing.Size size, ref DialogSettings settings)
    {
        _ = jsRuntime.InvokeVoidAsync("eval", $"console.log('Dialog resize. Width:{size.Width}, Height:{size.Height}')");
        settings ??= new DialogSettings();
        settings.Width = $"{size.Width}px";
        settings.Height = $"{size.Height}px";
        _ = SaveSettingsAsync(jsRuntime, settings);
    }
}

