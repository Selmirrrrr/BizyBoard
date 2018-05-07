namespace BizyBoard.Models.DbEntities
{
    public interface ITenantEntity : IEntityBase
    {
        int TenantId { get; set; }
    
        Tenant Tenant { get; set; }
    }
}