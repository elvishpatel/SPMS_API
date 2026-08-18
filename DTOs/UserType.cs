namespace SPMS_API.DTOs
{
    public class ReadUserType
    {
        public int UserTypeId { get; set; }

        public string UserTypeName { get; set; } = string.Empty;

        public string? Description { get; set; }
    }


    public class CreateUserType
    {
        public string UserTypeName { get; set; } = string.Empty;

        public string? Description { get; set; }
    }


    public class UpdateUserType
    {
        public string UserTypeName { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}