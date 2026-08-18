namespace SPMS_API.DTOs
{
    public class ReadTaskPriority
    {
        public int TaskPriorityId { get; set; }

        public string TaskPriorityName { get; set; } = string.Empty;

        public string TaskPriorityCssClass { get; set; } = string.Empty;
    }


    public class CreateTaskPriority
    {
        public string TaskPriorityName { get; set; } = string.Empty;

        public string TaskPriorityCssClass { get; set; } = string.Empty;
    }


    public class UpdateTaskPriority
    {
        public string TaskPriorityName { get; set; } = string.Empty;

        public string TaskPriorityCssClass { get; set; } = string.Empty;
    }
}