namespace BizyBoard.Models.DbEntities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.AspNetCore.Identity;

    public class AppUser : IdentityUser<int>, ITenantEntity
    {
        public bool IsEnabled { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string ErpUsername { get; set; }

        public string ErpPassword { get; set; }

        public int TenantId { get; set; }

        [InverseProperty("Users")]
        public virtual Tenant Tenant { get; set; }

        [InverseProperty("User")]
        public AppUserPhoto ProfilePhoto { get; set; }

        public virtual ICollection<IdentityUserRole<int>> Roles { get; set; }

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }

        [NotMapped]
        public string FullName => $"{Firstname} {Lastname}";
    }
}