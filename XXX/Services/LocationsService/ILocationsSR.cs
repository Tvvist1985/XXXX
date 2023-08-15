using Models.JsonTpansportModel;
using Models.ModelVM;

namespace Services.LocationsService
{
    public interface ILocationsSR
    {
        public Dictionary<string, List<City>> Locations { get; }
        public void InitializeDictionary(Stream stream);
        public void GetNewCitiesForUser(string currentKey, UserVM userVM);
        public void GetNewCitiesForUserSearch(string currentKey, AponentJson data);
    }
}
