using ApiCuby.JwtToken;
using APIService.GetUsersServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.DataModel.AponentDataModel;
using Models.DataModel.MainDataUserModel;
using Models.DataModel.MonetizedDataModel;
using Models.ModelVM;
using Services.DataDB.Repository;
using System.Text.Json.Serialization;
using System.Text.Json;
using Models.JsonTpansportModel;
using ApiServices.WorkingWithImage;
using Models.DataModel.DeleteSympathyModel;

namespace ApiCuby.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AboutUsersController : Controller
    {
        private readonly IStoreDbRepository db;
        private readonly DataForCuby data;
        private readonly IGetUsers getUsers;

        public AboutUsersController(IStoreDbRepository db, DataForCuby data, IGetUsers getUsers)
        {
            this.db = db;
            this.data = data;
            this.getUsers = getUsers;
        }

        [HttpGet]
        [Route("{action}/{id}/{email}")]
        public async Task<IActionResult> GetUsers(string id, string email)
        {                  
            var currentUser = db.DirectRequestToDatabase(p => p.MainUserDTO.Include(p =>p.EventUserDTO) //get current user and associated data
            .Include(p => p.MonetizedDataDTO).Include(p => p.AponentDTO).Include(p => p.DeleteSympathyDTO).FirstOrDefault(p => p.Id == new Guid(id)));
                     
            data.JwtToken = AddJwtToken.AddToken(email, id);  //создаю JWT нужен для поисковика в базе
            
            await data.ApponentDataJson.InicializedApponentJsonAsync(currentUser.AponentDTO);////инициализирую поисковик текущего аппонента

            List<MainUserDTO> ManList = new List<MainUserDTO>();//создаю обьекты для получение коллекций из базы
            List<MainUserDTO> WomanList = new List<MainUserDTO>();           
            getUsers.OnInitializedListsUsers(data, ref ManList, ref WomanList);  //получаю пользователей из базы
                      
            List<MainUserDTO> contacts = GetContacts(new Guid(id), currentUser.DeleteSympathyDTO); //получаю контакты текущего пользователя
            AccrualOfLikes(currentUser.MonetizedDataDTO); //начисляю лайки если время пришло
           
            Task task = data.InicializedUsersForSerializeAsync(ManList, Gender.Man,          //Сериализую Man
                WorkingWithIMG.CreateGetDirectoryAsync, WorkingWithIMG.GetAllBase64FromFileAsync);          
            Task task1 = data.InicializedUsersForSerializeAsync(WomanList, Gender.Woman,          //Сериализую Woman
                WorkingWithIMG.CreateGetDirectoryAsync, WorkingWithIMG.GetAllBase64FromFileAsync);
            Task task2 = data.InicializedUsersForSerializeAsync(contacts, Gender.Contact,     //Сериализую контакты
                WorkingWithIMG.CreateGetDirectoryAsync, WorkingWithIMG.GetAllBase64FromFileAsync);

            //Инициализирую события текущего пользователя
            Task task4 = data.InicializedMyEventsAsync(currentUser.EventUserDTO);
            //Инициализирую MonetizedData
            Task task5 = data.InicializedMonetizedDataAsync(currentUser.MonetizedDataDTO);
            //Ожидания задач
            await Task.WhenAll(task, task1, task2, task4, task5);

            var options = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };
            return Json(data, options);
        }

        ///Method: получаю рандомных
        [HttpGet]
        [Route("{action}")]
        public async Task<IActionResult> GetRandomUsers()
        {
            //создаю обьекты для получение коллекций из базы
            List<MainUserDTO> ManList = new List<MainUserDTO>();
            List<MainUserDTO> WomanList = new List<MainUserDTO>();
            //получаю пользователей из базы
            getUsers.OnInitializedListsUsers(data, ref ManList, ref WomanList);
            
            //Сериализую man
            Task task = data.InicializedUsersForSerializeAsync(ManList, Gender.Man,
                WorkingWithIMG.CreateGetDirectoryAsync, WorkingWithIMG.GetAllBase64FromFileAsync);
            //Сериализую Womanman
            Task task1 = data.InicializedUsersForSerializeAsync(WomanList, Gender.Woman, 
                WorkingWithIMG.CreateGetDirectoryAsync, WorkingWithIMG.GetAllBase64FromFileAsync);

            //Ожидания задач
            await Task.WhenAll(task, task1);

            var options = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };
            return Json(data, options);
        }

        //Method: Получить JWT как метод контроллеа для постоянных обновлений
        [HttpGet]
        [Route("{action}/{email}/{id}")]
        [Authorize]
        public string GetJwtToken(string email, string id) => AddJwtToken.AddToken(email, id);

        //Method: Аунтефикации пользователя
        [HttpGet]
        [Route("{action}/{email}/{password}")]
        public IActionResult IdentityUser(string email, string password)
        {
            //проверка на соответствия логина
            MainUserDTO user = db.DirectRequestToDatabase(p => p.MainUserDTO.FirstOrDefault(p => p.EmailAdress == email));
            //проверка на соответствие данных
            if (user is null) return BadRequest("User not found.");
            else if (user.Password != password) return BadRequest("Invalid User Password.");

            var options = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };
            return Json(new LoginVM { Id = user.Id.ToString(), Email = user.EmailAdress, Gender = user.Gender }, options); 
        }

        //Method: Изменение поисковика текущего прользователя
        [HttpPost]
        [Route("{action}")]
        [Authorize]
        public void ReinstallSearch(AponentJson data)
        {           
            //инициализирую поисковик для базы
            AponentDTO newSearch = data.InicializedApponentDTOAsync(data).Result;
            db.Update(newSearch);
        }

        #region
        //Method: Прлучаю контакты  
        private List<MainUserDTO> GetContacts(Guid id, List<DeleteSympathyDTO> entity)
        {
            //получаю совпадения контактов
            List<MainUserDTO> Coincidences =  GetCoincidences(id);
            //Получаю контакты через чат
            List<MainUserDTO> contactsFromChat = GetContactChat(id, entity);
            //Обьеденяю коллекции
            return UnionContats(Coincidences, contactsFromChat);
        }

        //Method: Получаем контакты и фильтрую через собития текущего пользователя
        private List<MainUserDTO> GetCoincidences(Guid id)
        {
            List<MainUserDTO> Coincidences = new();

            //загружаю пользователей связанных с текущим пользователем        
            var contacts = db.DirectRequestToDatabase(p => p.MainUserDTO.AsNoTracking().Include(p => p.EventUserDTO.Where(p => p.ApponentID == id))
            .Include(p => p.ChatDTO.Where(p => p.MainUserDTO.Any(p => p.Id == id))).ThenInclude(p => p.ChatMessages)
            .Include(p => p.AponentDTO).Where(p => p.EventUserDTO.Any(p => p.ApponentID == id)).ToList());

            //Выбираю совпадения по симпатиям           
            foreach (var data in data.MyEvents)
            {
                var user = contacts.FirstOrDefault(p => p.Id == data.ApponentID);
                if (user is not null) Coincidences.Add(user);
            }
            return Coincidences;
        }

        //Method: Получаю список пользователй с контактом через чат 
        private List<MainUserDTO> GetContactChat(Guid id, List<DeleteSympathyDTO> entity)
        {
            var contactsFromChat = db.DirectRequestToDatabase(p => p.MainUserDTO.AsNoTracking().Where(p => p.ChatDTO.Any(p => p.MainUserDTO.Any(p => p.Id == id))
            && p.Id != id).Include(p => p.EventUserDTO.Where(p => p.ApponentID == id))
            .Include(p => p.ChatDTO.Where(p => p.MainUserDTO.Any(p => p.Id == id))).ThenInclude(p => p.ChatMessages)
            .Include(p => p.AponentDTO).ToList());
                     
            //Удаляю пользователей с меткой удалить
            foreach (var label in entity)
            {
                var index = contactsFromChat.IndexOf(contactsFromChat.FirstOrDefault(p => p.Id == label.ApponentID));
                if (index > -1) contactsFromChat.RemoveAt(index);
            }

            return contactsFromChat;
        }

        //Method: обьединение таблиц Контактов
        private List<MainUserDTO> UnionContats(List<MainUserDTO> Coincidences, List<MainUserDTO> contactsFromChat)
        {
            //Обьеденяю 2 таблици
            if (Coincidences.Any() && contactsFromChat.Any()) return Coincidences.Union(contactsFromChat).ToList();            
            else if (Coincidences.Any() && !contactsFromChat.Any()) return Coincidences;           
            else return contactsFromChat;            
        }
       
        //Method: Ежедневное начисление лайков
        private void AccrualOfLikes(MonetizedDataDTO monetizedData)
        {
            var days = (DateTime.Now - monetizedData.TimeLastSession).Value.Days;
            
            if (days > 0)
            {
                //Изменяю данные (лайки и последнее время после изменения)
                monetizedData.Likes += (short)(days * 2);
                monetizedData.TimeLastSession = monetizedData.TimeLastSession.Value.AddDays(days);

                //Сохраняю изменения
                db.Update(monetizedData);
            }
        }
        #endregion
    }
}
