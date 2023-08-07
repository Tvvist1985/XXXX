using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Models.DataModel.MainDataUserModel;

namespace Models.DataModel.AponentDataModel
{
    public abstract class AponentDTO 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]        
        public Guid Id { get; set; }

        [Range(18, 99)]
        public short InitialAge { get; set; } = 18;
        [Range(18, 99)]
        public short FinalAge { get; set; } = 18;
        [MaxLength(30)]
        public string? Сountry { get; set; } = string.Empty;
        [MaxLength(30)]
        public string? City { get; set; } = string.Empty;
        public bool Man { get; set; } = false;
        public bool Woman { get; set; } = false;
        [MaxLength(5)]
        public string? MyGender { get; set; } = default;       
        public Guid MainUserDTOId { get; set; }        
        public MainUserDTO? MainUserDTO { get; set; }
    }
}
