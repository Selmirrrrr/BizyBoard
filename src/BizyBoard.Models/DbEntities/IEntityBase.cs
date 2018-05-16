namespace BizyBoard.Models.DbEntities
{
    using System;

    public interface IEntityBase
    {
        AppUser CreatedBy { get; set; }
        string CreatedByFullName { get; set; }
        int? CreatedById { get; set; }
        DateTime CreationDate { get; set; }
        int Id { get; set; }
        bool IsDeleted { get; set; }
        AppUser LastUpdateBy { get; set; }
        string LastUpdateByFullName { get; set; }
        int? LastUpdateById { get; set; }
        DateTime LastUpdateDate { get; set; }
        byte[] RowVersion { get; set; }
    }
}