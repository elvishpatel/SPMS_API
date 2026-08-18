using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPMS_API.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]

        public string RoleName { get; set; }

        [MaxLength(250)]

        public string? Description { get; set; } = string.Empty;

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
