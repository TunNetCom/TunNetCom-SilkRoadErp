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
        return badRequestResponse.errors.FirstOrDefault().Value
            .Select(t => t)
            .ToList();
    }
}
