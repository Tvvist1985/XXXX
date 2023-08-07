using Models.DataModel.MainDataUserModel;
using System.ComponentModel.DataAnnotations;

namespace Models.DataModels
{
    public class UsersMapDTO 
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [MaxLength(5)]
        public string Gender { get; set; } = default;
        public List<MainUserDTO>? MainUserDTO { get; set; } = new();
    }
}
