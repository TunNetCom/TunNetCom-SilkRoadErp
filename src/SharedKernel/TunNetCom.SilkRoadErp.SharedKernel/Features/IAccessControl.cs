namespace TunNetCom.SilkRoadErp.SharedKernel.Features;

public interface IAccessControl
{
    bool HasAccess(string featureKey, string permissionKey);
}
