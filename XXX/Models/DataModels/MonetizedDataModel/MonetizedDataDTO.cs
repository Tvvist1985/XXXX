using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Models.DataModel.MainDataUserModel;

namespace Models.DataModel.MonetizedDataModel
{
    public abstract class MonetizedDataDTO 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]       
        public Guid? Id { get; set; }
        public short Likes { get; set; } = 100;       
        public DateTime? TimeLastSession { get; set; }  
        
        public Guid MainUserDTOId { get; set; }  
        
        public MainUserDTO? MainUserDTO { get; set; }
    }
}
