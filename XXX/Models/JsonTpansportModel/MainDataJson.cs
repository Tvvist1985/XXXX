using Models.DataModel.ChatEventModel;
using Models.DataModel.MainDataUserModel;
using System.Text.Json.Serialization;

namespace Models.JsonTpansportModel
{
    public class MainDataJson : MainUserDTO
    {       
        public AponentJson? AponentJs { get; set; }   //WARNING  NEED FOR PERSONAL PAGE  !!!!!!!!!!!!       
        public EventUserJson? EventUserJs { get; set; }
        public ChatDTO Chat { get; set; }
        public string?[] imageBase64 { get; set; }     
        [JsonIgnore]
        public short? Age { get; set; }

        //Method:инициализирую основные данные
        public async Task OnInitializDataAsync(MainUserDTO data)
        {
            await Task.Run(() =>
            {
                //инициализирую основные данные
                Id = data.Id;
                FirstName = data.FirstName;
                DateOfBirth = data.DateOfBirth;
                Gender = data.Gender;
                Сountry = data.Сountry;
                City = data.City;
                EmailAdress = data.EmailAdress;
                Telephone = 0;
                Password = " ";
                ConfirmPassword = " ";
                Height = data.Height;
                Weight = data.Weight;
                Language = data.Language;
                Interests = data.Interests;
                AboutMe = data.AboutMe;
            });

        }
        //Method:инициализирую фото
        public async Task OnInitializIMGAsync(MainUserDTO data, Func<string[], Task<string>> GetDirectory, Func<string, Task<string[]>> GetImgBase64)
        {
            await Task.Run(() =>
            {
                //инициализация фото
                var path = GetDirectory(new string[] { "Images", data.Gender, data.Id.ToString()}).Result;
                imageBase64 = GetImgBase64(path).Result;
            });
        }
        //Method:инициализирую аппонента
        public async Task OnInitializApponentAsync(MainUserDTO data)
        {
            await Task.Run(() =>
            {
                AponentJs = new AponentJson();
                AponentJs.InicializedApponentJsonAsync(data.AponentDTO).Wait();
            });
        }

        //Метод для обьеденение таблиц
        public override bool Equals(object? obj)
        {
            if (obj is MainDataJson user) return Id == user.Id;
            return false;
        }
        public override int GetHashCode() => Id.GetHashCode();
    }
}
