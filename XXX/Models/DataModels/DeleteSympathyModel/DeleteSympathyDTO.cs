using Models.DataModel.MainDataUserModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.DataModel.DeleteSympathyModel
{
    public abstract class DeleteSympathyDTO 
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]        
        public Guid? Id { get; set; }       
        public Guid? ApponentID { get; set; }        
        public Guid MainUserDTOId { get; set; }       
        public MainUserDTO? MainUserDTO { get; set; }
    }
}
