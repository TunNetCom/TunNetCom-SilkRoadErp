namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services;

public class BadRequestResponse
{
    public int? Status { get; set; }
    public string Title { get; set; }
    public string Detail { get; set; }
    public string Instance { get; set; }
    public IDictionary<string, string[]> errors { get; set; }

}

public static class BadRequestResponseExtensions
{
    public static List<string> ToErrorsList(this BadRequestResponse badRequestResponse)
    {
        if (badRequestResponse?.errors == null || badRequestResponse.errors.Count == 0)
            return new List<string>();

        var first = badRequestResponse.errors.FirstOrDefault();
        if (first.Value == null || first.Value.Length == 0)
            return new List<string>();

        return first.Value.ToList();
    }
}
