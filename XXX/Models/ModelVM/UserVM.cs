using Models.DataModel.MainDataUserModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Models.ModelVM
{
    public class UserVM : MainUserDTO
    {               
        [JsonIgnore]
        public string[] AvatarTitle { get; set; } = { "Avatar", "Photo1", "Photo2", "Photo3", "Photo4", "Photo5" };
        [JsonIgnore]
        public string[] ErrerMessage { get; set; } = new string[6];
        public string? ErrerMessageEmailUniq { get; set; }
        public string? Success { get; set; }
        public string?[] imageBase64 { get; set; } = new string[6];
        public string[] ext { get; set; } = new string[6];
               
        [Required(ErrorMessage = "Avatar photo is required!")]
        public string? PhotoName { get; set; } 
       
        [Required(ErrorMessage = "The age cannot be less than 18 years.")]
        [Range(18, 90, ErrorMessage = "The age cannot be less than 18 years.")]
        public int Age { get; set; }

        //Инициальзация обьект для базы        
        public async Task MainDataInitialization(MainUserDTO mainData)
        {
            await Task.Run(() =>
            {                          
                mainData.FirstName = FirstName;
                mainData.DateOfBirth = DateOfBirth;
                mainData.Gender = Gender;
                mainData.Сountry = Сountry;
                mainData.City = City;
                mainData.EmailAdress = EmailAdress;
                mainData.Telephone = Telephone;
                mainData.Password = Password;
                mainData.ConfirmPassword = ConfirmPassword;
                mainData.Height = Height;
                mainData.Weight = Weight;
                mainData.Language = Language;
                mainData.Interests = Interests;
                mainData.AboutMe = AboutMe;
            });
        }

        //Method: Initialized country and city fo InputSelect
        public void InitializedForInputSelect(string country, string city)
        {
            if (string.IsNullOrEmpty(Сountry))
            {
                Сountry = country;
                City = city;
            }
        }

        //Method: инициализирую фото в base64 для отправки
        public async Task InitializIMG(Func<string[], Task<string>> GetDirectory, Func<string, Task<string[]>> GetFileNames,
            Func<string,string,Task<string>> GetImgBase64)
        {
            await Task.Run(async () =>
            {
                //инициализация фото
                var path = GetDirectory(new string[] {"Images", Gender, Id.ToString()}).Result;
                string[] fileNames = GetFileNames(path).Result;
                List<Task> tasks = new List<Task>(6);

                //Получаю чистые имена без расширения
                foreach (var img in fileNames)
                {
                    tasks.Add(Task.Run(() =>
                    {                    
                        int index = img.IndexOf(".");
                        var cleanName = img.Substring(0, index);

                        //Сохраняю фото в массив              
                        imageBase64[Array.IndexOf(AvatarTitle, cleanName)] = GetImgBase64(path, img).Result;
                    }));                    
                };                  
                await Task.WhenAll(tasks);      
            });
        }
    }
}
