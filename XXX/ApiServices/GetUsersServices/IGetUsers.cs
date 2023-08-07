using Models.DataModel.MainDataUserModel;
using Models.ModelVM;

namespace APIService.GetUsersServices
{
    public interface IGetUsers
    {
        public void OnInitializedListsUsers(DataForCuby data, ref List<MainUserDTO> ManList, ref List<MainUserDTO> WomanList);
    }
}
