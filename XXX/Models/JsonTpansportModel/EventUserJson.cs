using Models.DataModel.EventDataUserModel;

namespace Models.JsonTpansportModel
{
    public class EventUserJson : EventUserDTO
    {
        public async Task InicializedApponentID(EventUserDTO data) => ApponentID = data.ApponentID;       
    }
}
