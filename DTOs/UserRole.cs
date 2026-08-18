namespace SPMS_API.DTOs
{
    public class ReadUserRole
    {
        public int RolePermissionId { get; set; }

        public int RoleId { get; set; }

        public string? RoleName { get; set; }

        public int UserId { get; set; }

        public string? FullName { get; set; }

    }


    public class CreateUserRole
    {
        public int RoleId { get; set; }

        public int UserId { get; set; }
    }


    public class UpdateUserRole
    {
        public int RoleId { get; set; }

        public int UserId { get; set; }
    }
}