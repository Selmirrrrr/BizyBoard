namespace BizyBoard.Models.DbEntities
{
    public interface ITenantEntity
    {
        int TenantId { get; set; }

        Tenant Tenant { get; set; }
    }
}