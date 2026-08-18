namespace SPMS_API.DTOs
{
    public class ReadTaskStatus
    {
        public int TaskStatusID { get; set; }

        public string TaskStatusName { get; set; } = string.Empty;

        public string TaskStatusCssClass { get; set; } = string.Empty;
    }


    public class CreateTaskStatus
    {
        public string TaskStatusName { get; set; } = string.Empty;

        public string TaskStatusCssClass { get; set; } = string.Empty;
    }


    public class UpdateTaskStatus
    {
        public string TaskStatusName { get; set; } = string.Empty;

        public string TaskStatusCssClass { get; set; } = string.Empty;
    }
}