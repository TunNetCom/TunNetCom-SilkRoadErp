using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Helpers;

public static class PropertyHelper
{
    public static object GetPropertyValue(GetDeliveryNoteBaseInfos item, string propertyName)
    {
        if (item == null || string.IsNullOrEmpty(propertyName))
        {
            return null;
        }

        var propertyInfo = item.GetType().GetProperty(propertyName);
        return propertyInfo?.GetValue(item);
    }
}
