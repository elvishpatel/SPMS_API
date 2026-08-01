using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPMS_API.Models
{
    public class UserType
    {
        [Key]
        public int UserTypeId { get; set; }   

        [Required, MaxLength(50)]
        public string UserTypeName { get; set; }

        [MaxLength(250)]
        public string? Description { get; set; }
    }
}
