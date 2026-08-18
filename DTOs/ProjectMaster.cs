namespace SPMS_API.DTOs
{
    public class ReadProjectMaster
    {
        public int ProjectId { get; set; }

        public string ProjectTitle { get; set; } = string.Empty;

        public string? Description { get; set; }
    }


    public class CreateProjectMaster
    {
        public string ProjectTitle { get; set; } = string.Empty;

        public string? Description { get; set; }
    }


    public class UpdateProjectMaster
    {
        public string ProjectTitle { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}