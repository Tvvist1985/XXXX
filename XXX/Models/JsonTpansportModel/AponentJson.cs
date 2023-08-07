using Models.DataModel.AponentDataModel;
using Models.DataModel.AponentDataModel.AponentForMan;
using Models.DataModel.AponentDataModel.AponentForWoman;

namespace Models.JsonTpansportModel
{
    public class AponentJson : AponentDTO
    {
        //Method: инициализирую поисковик для отправки клиенту
        public async Task InicializedApponentJsonAsync(AponentDTO apponent)
        {
            await Task.Run(() =>
            {
                Id = apponent.Id;
                MyGender = apponent.MyGender;
                City = apponent.City;
                Сountry = apponent.Сountry;
                Man = apponent.Man;
                Woman = apponent.Woman;
                FinalAge = apponent.FinalAge;
                FinalAge = apponent.FinalAge;
            });          
        }
        //Method: дессиарелизация JSON Apponent
        public async Task<AponentDTO> InicializedApponentDTOAsync(AponentJson apponent)
        {
            AponentDTO newSearch = default;
            await Task.Run(() =>
            {

                if (apponent.MyGender == "Man") newSearch = new AponentForManDTOTbl1();
                else newSearch = new AponentForWomanDTOTbl1();

                newSearch.Id = apponent.Id;
                newSearch.MyGender = apponent.MyGender;
                newSearch.City = apponent.City;
                newSearch.Сountry = apponent.Сountry;
                newSearch.Man = apponent.Man;
                newSearch.Woman = apponent.Woman;
                newSearch.InitialAge = apponent.InitialAge;
                newSearch.FinalAge = apponent.FinalAge;
                newSearch.MainUserDTOId = apponent.Id;
            });
            return newSearch; 
        }
    }
}
