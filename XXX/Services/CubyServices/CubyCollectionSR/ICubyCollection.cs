using Models.JsonTpansportModel;
using Models.ModelVM;

namespace Services.CubyServices.CubyCollectionSR
{
    public interface ICubyCollection
    {       
        public List<MainDataJson> Re_cuttingContainer { get; set; } 
        public List<List<MainDataJson>> UsersContainers { get; set; }
        public byte NumberContainerForSelectUser { get; set; }
        public byte NumberSelectUser { get; set; }
        public byte NumberPhoto { get; set; }
        public DataForCuby DataForXXX { get; set; }
        public Task OnInitializedContainerAsync();
        public void GetNextUser();
    }
}
