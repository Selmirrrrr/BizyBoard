namespace BizyBoard.Models.DbEntities
{
    public class AppUserPhoto : EntityBase, ITenantEntity
    {
        public string ContentType { get; set; }
        
        public byte[] Content { get; set; }
        
        public int UserId { get; set; }
        
        public AppUser User { get; set; }
        
        public int TenantId { get; set; }
    
        public virtual Tenant Tenant { get; set; }
    }
}