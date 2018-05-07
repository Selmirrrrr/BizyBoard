namespace BizyBoard.Models.DataTransfertObjects
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        public int TenantId { get; set; }

        public UserDto()
        {
        }

        public UserDto(int id, string email, string fullname, int tenantId)
        {
            Id = id;
            Email = email;
            Fullname = fullname;
            TenantId = tenantId;
        }
    }
}