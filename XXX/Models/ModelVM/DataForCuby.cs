using Models.DataModel.EventDataUserModel;
using Models.DataModel.MainDataUserModel;
using Models.DataModel.MonetizedDataModel;
using Models.JsonTpansportModel;

namespace Models.ModelVM
{
    public enum Gender : byte
    {
        Man, Woman,Contact
    }

    public class DataForCuby 
    {       
        public string? JwtToken { get; set; }         
        public AponentJson ApponentDataJson { get; set; } = new();
        public MonetizedDataJson monetizedData { get; set; } = new();
        public List<EventUserJson> MyEvents { get; set; } = new();       
        public List<MainDataJson>? MyContacts { get; set; } = new();
        //пользователи
        public List<MainDataJson> ManListJson { get; set; } = new();
        public List<MainDataJson> WomanListJson { get; set; } = new();

        //Method: Инициализирую списко пользователей JSON для сенриализациии и отправки        
        public async Task InicializedUsersForSerializeAsync(List<MainUserDTO> users, Gender gender,
            Func<string[], Task<string>> GetDirectory, Func<string, Task<string[]>> GetImgBase64)
        {
            await Task.Run(async () =>
            {
                //Проверка на наличае пользователей в коллекции           
                if (users.Any())
                {
                    foreach (var data in users)
                    {
                        List<Task> tasks = new List<Task>();

                        var obj = new MainDataJson();//Создаю новый обтект для JSON коллекции

                        tasks.Add(Task.Run(async () => await obj.OnInitializDataAsync(data)));//инициализирую основные данные
                        tasks.Add(Task.Run(async () => await obj.OnInitializIMGAsync(data, GetDirectory, GetImgBase64)));    //инициализирую фото                                                           
                        if (!string.IsNullOrEmpty(JwtToken))
                        {
                            tasks.Add(Task.Run(async () => await obj.OnInitializApponentAsync(data)));  //Данные апонента  
                            if (data.EventUserDTO?.FirstOrDefault() is not null)//Инициализирую событие если есть
                            {
                                obj.EventUserJs = new EventUserJson();
                                tasks.Add(Task.Run(async () => await obj.EventUserJs.InicializedApponentID(data.EventUserDTO.FirstOrDefault())));
                            }

                            obj.Chat = data.ChatDTO.FirstOrDefault(); //Initialized chat
                        }
                        await Task.WhenAll(tasks);

                        if (gender == Gender.Man)
                            ManListJson.Add(obj); //Добавляю в коллекцию пользователей для  сериализации
                        else if (gender == Gender.Woman)
                            WomanListJson.Add(obj);
                        else
                            MyContacts.Add(obj);

                    }
                }
            });
        }
       
        //Method:Инициализация событий данноого пользователя
        public async Task InicializedMyEventsAsync(List<EventUserDTO> events)
        {
            await Task.Run(() =>
            {
                if (events.Any())
                    foreach (var data in events)
                    {
                        EventUserJson newEvent = new EventUserJson();
                        newEvent.InicializedApponentID(data);
                        MyEvents.Add(newEvent);
                    }
            });
        }

        //Method:Инициализация манетизацию данного пользователя
        public async Task InicializedMonetizedDataAsync(MonetizedDataDTO mData)
        {
            await Task.Run(() =>
            {
                monetizedData = new();
                monetizedData.InicializedLikesAsync(mData);
            });
        }        
    }
}
