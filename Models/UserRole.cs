using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPMS_API.Models
{
    public class UserRole
    {
        [Key]
        public int RolePermissionId { get; set; }

        public Role? Role { get; set; }

        [Required,ForeignKey("Role")]
        public int RoleId { get; set; }


        public User? User { get; set; }

        [Required, ForeignKey("User")]
        public int UserId { get; set; }


    }
}
