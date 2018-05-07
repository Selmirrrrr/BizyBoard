namespace BizyBoard.Models.DbEntities
{
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.AspNetCore.Identity;

    public class AppUser : IdentityUser<int>
    {
        public bool IsEnabled { get; set; }

        public string Firstname { get; set; }
        
        public string Lastname { get; set; }
        
        public string ErpUsername { get; set; }
        
        public string ErpPassword { get; set; }

        public decimal WeeklyWorkingHours { get; set; }

        public int TenantId { get; set; }
    
        [InverseProperty("Users")]    
        public virtual Tenant Tenant { get; set; }

        [InverseProperty("User")]
        public AppUserPhoto ProfilePhoto { get; set; }

        [NotMapped]
        public string FullName => $"{Firstname} {Lastname}";
    }
}