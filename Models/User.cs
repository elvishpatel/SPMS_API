using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SPMS_API.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [JsonIgnore]
        public UserType? UserType { get; set; }

        [ForeignKey("UserType")]
        public int UserTypeId { get; set; }

        [Required, MaxLength(250)]
        public string FullName { get; set; }


        public string? UserCode { get; set; } = string.Empty;


        [Required, MaxLength(150)]
        [EmailAddress]
        public string Email { get; set; }

        [Required, MaxLength(255)]
        public string Password { get; set; }

        [MaxLength(15)]
        public string MobileNumber { get; set; } = string.Empty;

        [MaxLength(500)]
        public string ProfilePicturePath { get; set; } = string.Empty;

        [Required]
        public bool IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<ProjectAllocation> StudentProjectAllocations { get; set; } = new List<ProjectAllocation>();
        public ICollection<ProjectAllocation> FacultyProjectAllocations { get; set; } = new List<ProjectAllocation>();
    }
}
