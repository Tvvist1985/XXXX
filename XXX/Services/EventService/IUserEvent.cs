using Models.JsonTpansportModel;
using Services.CubyServices.CubyCollectionSR;

namespace Services.EventService
{
    public interface IUserEvent
    {
        public ICubyCollection cubyCollection { get; set; }    
        public string MessageAboutLikes { get; set; }
        public string Icon { get; set; }
        public string LikeStyle { get; set; }      
        public void LoadIcon(Guid apponentId, byte numberContainer);
        public string LoadIcon(MainDataJson UserModel);
        public void CreateOrDeleteEvent(MainDataJson user,short numberContainer = -1);
        public void DeleteContact(MainDataJson user);
    }
}
