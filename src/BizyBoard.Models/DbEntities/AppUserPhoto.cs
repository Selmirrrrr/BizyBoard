namespace BizyBoard.Models.DbEntities
{
    using System.ComponentModel.DataAnnotations.Schema;

    public class AppUserPhoto : EntityBase
    {
        public string ContentType { get; set; }

        public byte[] Content { get; set; }

        public int UserId { get; set; }

        [InverseProperty("ProfilePhoto")]
        public AppUser User { get; set; }
    }
}