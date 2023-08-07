using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Models.DataModels;
using Models.DataModel.AponentDataModel;
using Models.DataModel.MonetizedDataModel;
using Models.DataModel.EventDataUserModel;
using Models.DataModel.ChatEventModel;
using Models.DataModel.DeleteSympathyModel;
using System.Text.Json.Serialization;

namespace Models.DataModel.MainDataUserModel
{
    public abstract class MainUserDTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }    

        [Required]
        [MaxLength(12)]
        public string FirstName { get; set; }  

        [Required]
        public DateTime DateOfBirth { get; set; }

        [ConcurrencyCheck]
        [Required]
        [MaxLength(5)]
        public string? Gender { get; set; } 
        
        [Required]
        [MaxLength(30)]
        public string? Сountry { get; set; }    

        [Required]
        [MaxLength(30)]
        public string? City { get; set; }       

        [Required]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$", ErrorMessage = "Please, enter Valid Email (format: example@examp.com")]
        public string? EmailAdress { get; set; }     

        [Required]
        public long? Telephone { get; set; } = null;      
        
        [Required]
        [MaxLength(50), MinLength(5)]
        public string Password { get; set; }   
        
        [Required]
        [MaxLength(50), MinLength(5)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
      
        public short? Height { get; set; }      
        public short? Weight { get; set; }     
        [MaxLength(30)]
        public string? Language { get; set; }
        [MaxLength(500)]
        public string? Interests { get; set; }      
        [MaxLength(500)]
        public string? AboutMe { get; set; }

        //Зависимая       
        public Guid? UsersMapDTOId { get; set; }       
        public UsersMapDTO? UsersMapDTO { get; set; }

        //Зависящие       
        public AponentDTO? AponentDTO { get; set; }       
        public MonetizedDataDTO? MonetizedDataDTO { get; set; }  
        
        public List<EventUserDTO>? EventUserDTO { get; set; }
        
        public List<ChatDTO>? ChatDTO { get; set; }      
        public List<DeleteSympathyDTO>? DeleteSympathyDTO { get; set; } 
           
        //Метод для обьеденение таблиц
        public override bool Equals(object? obj)
        {
            if (obj is MainUserDTO user) return Id == user.Id;
            return false;
        }
        public override int GetHashCode() => Id.GetHashCode();
    }
}
