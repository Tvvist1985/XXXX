using Microsoft.AspNetCore.Mvc;
using Models.DataModel.AponentDataModel.AponentForMan;
using Models.DataModel.AponentDataModel.AponentForWoman;
using Models.DataModel.MainDataUserModel.Man;
using Models.DataModel.MainDataUserModel.Woman;
using Models.DataModel.MonetizedDataModel.Man;
using Models.DataModel.MonetizedDataModel.Woman;
using Models.ModelVM;
using Services.DataDB.Repository;
using System.Text.Json.Serialization;
using System.Text.Json;
using Models.DataModel.MainDataUserModel;
using ApiServices.WorkingWithImage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Models.DataModel.AponentDataModel;
using Models.DataModel.DeleteSympathyModel.Man;
using Models.DataModel.DeleteSympathyModel.Woman;
using Models.DataModel.DeleteSympathyModel;
using Models.DataModel.EventDataUserModel.Man;
using Models.DataModel.EventDataUserModel.Woman;
using Models.DataModel.EventDataUserModel;
using Models.DataModel.MonetizedDataModel;
using Models.DataModel.ChatEventModel;
using System.Security.Claims;

namespace ApiCuby.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AddOrUpdateUserController : Controller
    {      
        private readonly IStoreDbRepository db;       
        public AddOrUpdateUserController(IStoreDbRepository db) { this.db = db; }
       
        ////Method: Выбора создание пользователя
        [HttpPost]
        [Route("{action}")]
        public async Task<IActionResult> AddUser(UserVM user)
        {
            Guid id = default;
            //Выбор пола
            if (user.Gender == "Man")
            {
                //Проверка на уникальность почты
                if (!db.DirectRequestToDatabase(p => p.MainUserDTO.Any(p => p.EmailAdress == user.EmailAdress)))
                {
                    //Уникальный id
                    id = GetUniqID();
                    //сохраняю фото
                    Task task = GreateTasksForAddImageAsync(user, user.Gender, id.ToString());

                    //Создания экземпляров таблиц исхлдя от выбора
                    UserManDTOTBL1 main = new() { Id = id, UsersMapDTOId = db.DirectRequestToDatabase(p => p.UsersMapDTO.FirstOrDefault(p => p.Gender == user.Gender).Id) };
                    //Инициализирую связные сущности
                    main.AponentDTO = new AponentForManDTOTbl1() { Id = id, City = user.City, Сountry = user.Сountry, MyGender = user.Gender };
                    main.MonetizedDataDTO = new MonetizedForManTBL1() { Id = id, TimeLastSession = DateTime.Now };
                    //Инициальзирую Основные данные
                    Task task1 = user.MainDataInitialization(main);
                    //сохраняю в базе
                    db.Add(main);

                    await Task.WhenAll(task, task1);
                }
                else
                    return BadRequest("Bad");
            }
            //WOMAN
            else
            {
                if (!db.DirectRequestToDatabase(p => p.MainUserDTO.Any(p => p.EmailAdress == user.EmailAdress)))
                {              
                    id = GetUniqID();

                    Task task = GreateTasksForAddImageAsync(user, user.Gender, id.ToString());

                    UserWomanDTOTBL1 main = new() { Id = id, UsersMapDTOId = db.DirectRequestToDatabase(p => p.UsersMapDTO.FirstOrDefault(p => p.Gender == user.Gender).Id) };
                    main.AponentDTO = new AponentForWomanDTOTbl1() { Id = id, City = user.City, Сountry = user.Сountry, MyGender = user.Gender };
                    main.MonetizedDataDTO = new MonetizedForWomanTBL1() { Id = id, TimeLastSession = DateTime.Now };
                    Task task1 = user.MainDataInitialization(main);
                    db.Add(main);

                    await Task.WhenAll(task,task1);
                }
                else 
                    return BadRequest("Bad");
            }         
            return Ok(id.ToString());
        }

        //Method:Получаю данные текущего пользователя
        [HttpGet]
        [Route("{action}/{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserForUpdate(string id)
        {
            MainUserDTO user = db.DirectRequestToDatabase(p => p.MainUserDTO.FirstOrDefault(p => p.Id == new Guid(id)));
            //инициализирую для отправки клиунту

            //сериализую данные
            var options = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault};            
            string jsonUser = JsonSerializer.Serialize(user, options);
            UserVM data = JsonSerializer.Deserialize<UserVM>(jsonUser);  
            
            //инициализирую фото
            await data.InitializIMG(WorkingWithIMG.GetDirectoryAsync, WorkingWithIMG.GetFileNameAsync, WorkingWithIMG.GetStringBase64FromFileAsync);
                      
            return Json(data, options); 
        }

        //Method: изменение пользователя
        [HttpPost]
        [Route("{action}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(UserVM content)
        {
            //проверка на уникальность почты
            if (db.DirectRequestToDatabase(p => p.MainUserDTO.Any(p => p.Id != content.Id && p.EmailAdress == content.EmailAdress)))
                return BadRequest("Bad");

            //Сохраняю фото 
            Task task = GreateTasksForAddImageAsync(content, content.Gender, content.Id.ToString());

            //ЕДИНСТВЕННЫЙ СПОСОБ  (Даже при сериализации  и копировании полного обьекта  ничё не работает)
            var user = db.DirectRequestToDatabase(p => p.MainUserDTO.FirstOrDefault(p => p.Id == content.Id));           
            Task task1 = content.MainDataInitialization(user);

            await Task.WhenAll(task, task1);

            db.Update(user);

            return Ok();
        }

        //Method: Перемещение пользователя
        [HttpPost]
        [Route("{action}")]
        [Authorize]
        public async Task<IActionResult> MoveUser(UserVM content)
        {
            //проверка на уникальность почты
            if (db.DirectRequestToDatabase(p => p.MainUserDTO.Any(p => p.Id != content.Id && p.EmailAdress == content.EmailAdress)))
                return BadRequest("Bad");
                         
            Guid NewMapId = db.DirectRequestToDatabaseAsync(p => p.UsersMapDTO.AsNoTracking().FirstOrDefault(p =>p.Gender == content.Gender)).Result.Id; //Получаю Id карты
                                                                                                                                                         //
            var user = db.DirectRequestToDatabase(p => p.MainUserDTO //получаю все данные кроме чатов
            .Include(p => p.AponentDTO).Include(p => p.EventUserDTO)
            .Include(p => p.MonetizedDataDTO).Include(p => p.DeleteSympathyDTO)
            .Include(p => p.ChatDTO)
            .FirstOrDefault(p => p.Id == content.Id));
            
            db.DeleteByEntity(user); //Удаляю пользователя   и связь многие ко многим   удаляеться только таким способом
                      
            MainUserDTO main = content.Gender == "Man" ? new UserManDTOTBL1() { Id = content.Id, ChatDTO = user.ChatDTO, UsersMapDTOId = NewMapId }  //Создаю основнные данные
            : new UserWomanDTOTBL1() { Id = content.Id, ChatDTO = user.ChatDTO, UsersMapDTOId = NewMapId };
           
            Parallel.Invoke( //Инициализирую данные
                () => MoveDirImg(content, user.Gender),//перемещение фото
                () => content.MainDataInitialization(main),
                () => InitializedAponentData(content, user.AponentDTO, main),
                () => InitializedMonetizedData(content, user.MonetizedDataDTO, main),
                () => InitializedEvent(content, user.EventUserDTO, main),
                () => InitializedDeleteSympathy(content, user.DeleteSympathyDTO, main));

            db.Add(main);
         
            return Ok();
        }

        //Method:Инициализация поисковика
        private void InitializedAponentData(UserVM content, AponentDTO currentData, MainUserDTO main)
        {
            AponentDTO futureData = default;
            if (content.Gender == "Man")
                futureData = new AponentForManDTOTbl1();
            else
                futureData = new AponentForWomanDTOTbl1();

            futureData.Id = currentData.Id;
            futureData.InitialAge = currentData.InitialAge;
            futureData.FinalAge = currentData.FinalAge;
            futureData.Man = currentData.Man;
            futureData.Woman = currentData.Woman;
            futureData.Сountry = currentData.Сountry;
            futureData.City = currentData.City;
            //гендер новый
            futureData.MyGender = content.Gender;

            main.AponentDTO = futureData;
        }

        //Method:Инициализация Монитизированых данных
        private void InitializedMonetizedData(UserVM content, MonetizedDataDTO currentData, MainUserDTO main)
        {
            MonetizedDataDTO futureData = default;
            if (content.Gender == "Man")
                futureData = new MonetizedForManTBL1();
            else
                futureData = new MonetizedForWomanTBL1();

            futureData.Id = currentData.Id;
            futureData.Likes = currentData.Likes;
            futureData.TimeLastSession = currentData.TimeLastSession;

            main.MonetizedDataDTO = futureData;
        }

        //Method: инициализирую события для перемещения в таблицу
        private void InitializedEvent(UserVM content, List<EventUserDTO> currentListEvent, MainUserDTO main)
        {
            List<EventUserDTO> futureListEvent = new();
            if (currentListEvent.Any() && content.Gender == "Man")
                foreach (var item in currentListEvent)
                    futureListEvent.Add(new EventManDTOTBL1() { Id = item.Id, ApponentID = item.ApponentID });
            else if (currentListEvent.Any() && content.Gender == "Woman")
                foreach (var item in currentListEvent)
                    futureListEvent.Add(new EventWomanDTOTBL1() { Id = item.Id, ApponentID = item.ApponentID });

            main.EventUserDTO = futureListEvent;
        }

        //Method: инициализирую меток удаления  для перемещения в таблицу
        private void InitializedDeleteSympathy(UserVM content, List<DeleteSympathyDTO> currentdeleteSympathy, MainUserDTO main)
        {
            List<DeleteSympathyDTO> futuredeleteSympathy = new();

            if (currentdeleteSympathy.Any() && content.Gender == "Man")
                foreach (var item in currentdeleteSympathy)
                    futuredeleteSympathy.Add(new DeleteSympathyForManTBL1() { Id = item.Id, ApponentID = item.ApponentID });
            else if (currentdeleteSympathy.Any() && content.Gender == "Woman")
                foreach (var item in currentdeleteSympathy)
                    futuredeleteSympathy.Add(new DeleteSympathyForWomanTBL1() { Id = item.Id, ApponentID = item.ApponentID });

            main.DeleteSympathyDTO = futuredeleteSympathy;
        }

        //Method: Создание задачь под сохранение фотографий
        private async Task GreateTasksForAddImageAsync(UserVM user, string gender, string id)
        {
            await Task.Run(async () =>
            {
                List<Task> tasks = new List<Task>(6);  //создаю Список Задач

                var path = WorkingWithIMG.CreateGetDirectoryAsync("Images", gender, id).Result;  //Создаю директорию

                byte j = 0;
                for (byte i = 0; i < user.imageBase64.Length; i++) //копирую изображения на сервер
                    tasks.Add(Task.Run(() => AddImg(j++)));

                await Task.WhenAll(tasks);

                void AddImg(byte i)
                {
                    if (!string.IsNullOrEmpty(user.ext[i]))
                    {
                        string fileName = user.AvatarTitle[i] + user.ext[i];
                        var images = WorkingWithIMG.GetFileNameAsync(path).Result;

                        foreach (var image in images)   //Удаляю фаил если есть
                        {
                            int index = image.IndexOf(".");
                            string clierName = image.Substring(0, index);

                            if (user.AvatarTitle[i].Equals(clierName))
                            {
                                WorkingWithIMG.DeleteFileAsync(Path.Combine(path, image)).Wait();
                                break;
                            }
                        }

                        WorkingWithIMG.CreateFileFromBase64StringAsync(path, fileName, user.imageBase64[i]).Wait();   //Создаю фото в директории
                    }
                }
            });
        }

        //Method: Перемещение директории с фото
        private void MoveDirImg(UserVM user, string gender)
        {
            string oldPath;
            string newPath;
            Task task;

            if (user.Gender != "Man")
            {
                task = GreateTasksForAddImageAsync(user, gender, user.Id.ToString());//Сохраняю фото в старую папку
                oldPath = WorkingWithIMG.GetDirectoryAsync("Images", gender, user.Id.ToString()).Result;//получаю директорию 
            }
            else
            {
                task = GreateTasksForAddImageAsync(user, gender, user.Id.ToString());
                oldPath = WorkingWithIMG.GetDirectoryAsync("Images", gender, user.Id.ToString()).Result;
            }

            newPath = WorkingWithIMG.GetDirectoryAsync("Images", user.Gender, user.Id.ToString()).Result;//получаю новую Dir
            task.Wait();
            Directory.Move(oldPath, newPath);//перемещаю директорию           
        }

        //Method: Получане уникального ID
        private Guid GetUniqID()
        {          
            while (true)
            {
                Guid id = Guid.NewGuid();
                if (!db.DirectRequestToDatabase(p => p.MainUserDTO.Any(p => p.Id == id))) return id;
            }
        }             
    }           
}