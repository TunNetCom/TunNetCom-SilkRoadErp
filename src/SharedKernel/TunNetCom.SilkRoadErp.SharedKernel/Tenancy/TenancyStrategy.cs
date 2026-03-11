namespace TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

public enum TenancyStrategy
{
    SharedDatabaseSharedSchema = 1,
    SharedDatabaseSeparateSchema = 2,
    DatabasePerTenant = 3
}
