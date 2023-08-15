using Models.JsonTpansportModel;
using Models.ModelVM;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Services.LocationsService
{
    public class LocationsSR : ILocationsSR
    {
        public Dictionary<string, List<City>> Locations { get; } = new();

        //Method:initialize dictionary
        public void InitializeDictionary(Stream stream)
        {
            //Get Locations.json from static files  to string           
            using var reader = new StreamReader(stream);
            var dataResourceText = reader.ReadToEndAsync().Result;

            //Deserialize to List
            var opt = new JsonSerializerOptions()
            {
                //Option for encoding  \u0436\u0430\u0440\u043A\u043E
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                WriteIndented = true
            };
            List<Country> Loc_s = JsonSerializer.Deserialize<List<Country>>(dataResourceText, opt);
            //initialized dictionary
            foreach (var loc in Loc_s)
                Locations.Add(loc.name, loc.cities);
        }

        //Method: Get new list cities for user page on teg InputSelect
        public void GetNewCitiesForUser(string currentKey, UserVM userVM)
        {
            while (currentKey.Equals(userVM.Сountry)) { }
            userVM.City = Locations[userVM.Сountry].FirstOrDefault()?.name;
        }
        //Method: Get new list cities for user search page on teg InputSelect
        public void GetNewCitiesForUserSearch(string currentKey, AponentJson data)
        {
            while (currentKey.Equals(data.Сountry)) { }
            data.City = Locations[data.Сountry].FirstOrDefault()?.name;
        }
    }
    public record class Country(string name, List<City> cities);
    public record class City(string name);
}
