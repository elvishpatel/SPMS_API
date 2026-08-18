namespace SPMS_API.DTOs
{
    public class ReadUser
    {
        public int UserId { get; set; }

        public int UserTypeId { get; set; }

        public string? UserTypeName { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string? UserCode { get; set; }

        public string Email { get; set; } = string.Empty;

        public string MobileNumber { get; set; } = string.Empty;

        public string ProfilePicturePath { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public bool? IsDeleted { get; set; }
    }


    public class CreateUser
    {
        public int UserTypeId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string? UserCode { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string MobileNumber { get; set; } = string.Empty;

        public string ProfilePicturePath { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }


    public class UpdateUser
    {
        public int UserTypeId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string? UserCode { get; set; }

        public string Email { get; set; } = string.Empty;

        public string? Password { get; set; } // Nullable in update to allow leaving unchanged

        public string MobileNumber { get; set; } = string.Empty;

        public string ProfilePicturePath { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public bool? IsDeleted { get; set; }

    }
}