namespace BizyBoard.Models.DbEntities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Tenant : EntityBase
    {
        [MaxLength(128)]
        public string Name { get; set; }

        public virtual ICollection<AppUser> Users { get; set; }
    }
}