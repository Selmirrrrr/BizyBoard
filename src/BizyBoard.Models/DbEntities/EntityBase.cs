namespace BizyBoard.Models.DbEntities
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class EntityBase : IEntityBase
    {
        public EntityBase()
        {
        }
        
        [Key]
        public int Id { get; set; }
        
        public int? CreatedById { get; set; }
        
        public AppUser CreatedBy { get; set; }
        
        [MaxLength(128)]
        public string CreatedByFullName { get; set; }
        
        public DateTime CreationDate { get; set; }

        public int? LastUpdateById { get; set; }
        
        public AppUser LastUpdateBy { get; set; }
        
        [MaxLength(128)]
        public string LastUpdateByFullName { get; set; }
        
        public DateTime LastUpdateDate { get; set; }
        
        [Timestamp]
        public byte[] RowVersion { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}