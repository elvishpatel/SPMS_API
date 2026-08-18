namespace SPMS_API.DTOs
{
    public class ReadRole
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; } = string.Empty;

        public string? Description { get; set; }
    }


    public class CreateRole
    {
        public string RoleName { get; set; } = string.Empty;

        public string? Description { get; set; }
    }


    public class UpdateRole
    {
        public string RoleName { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}