using Microsoft.AspNetCore.Mvc;
using Models.DataModel.EventDataUserModel.Man;
using Models.DataModel.EventDataUserModel.Woman;
using Models.DataModel.EventDataUserModel;
using Services.DataDB.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json.Serialization;
using System.Text.Json;
using Models.JsonTpansportModel;
using Models.DataModel.ChatEventModel;
using Models.DataModels.ChatMessage;
using Models.DataModel.DeleteSympathyModel.Man;
using Models.DataModel.DeleteSympathyModel.Woman;
using Models.DataModel.DeleteSympathyModel;

namespace ApiCuby.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ChatAndEventController : Controller
    {
        private readonly IStoreDbRepository db;
        public ChatAndEventController(IStoreDbRepository db) { this.db = db; }

        //Method: сохранение контата через лайк
        [HttpGet]
        [Route("{action}/{myId}/{apponentId}/{myGender}")]
        public void AddContacts(string myId, string apponentId, string myGender)
        {                                  
            var id = GetUniqEventID(); //унакальное id
                          
            EventUserDTO modelEvent = myGender == "Man" ? new EventManDTOTBL1 { Id = id, ApponentID = new Guid(apponentId), MainUserDTOId = new Guid(myId) } : //Создаю обьект события для симпатии 
                new EventWomanDTOTBL1 { Id = id, ApponentID = new Guid(apponentId), MainUserDTOId = new Guid(myId) };
            db.Add(modelEvent);

            var data = db.DirectRequestToDatabase(p => p.MonetizedDataDTO.FirstOrDefault(p => p.Id == new Guid(myId)));// Пересчитываю количество лайков
            data.Likes--;
            db.Update(data);

            ClearDeletionLabelAsync(myId, apponentId); //Удаляю метку если есть о удалении
        }
       
        //Method: Remove event
        [HttpGet]
        [Route("{action}/{myId}/{apponentId}")]
        public void DeleteEvent(string myId, string apponentId)
        {                       
            EventUserDTO modelEvent = db.DirectRequestToDatabase(p => p.EventUserDTO.FirstOrDefault(p => p.MainUserDTOId == new Guid(myId) && p.ApponentID == new Guid(apponentId)));  //получаю событие связанное с пользователем                  
            if(modelEvent != null) db.DeleteByEntity(modelEvent);
        }

        //Method: Создание метки удаления
        [HttpGet]
        [Route("{action}/{myId}/{apponentId}/{myGender}")]
        public void AddDeletionLabel(string myId, string apponentId, string myGender)
        {
            Guid id = default;  //Uniq id
            while (true)
            {
                id = Guid.NewGuid();               
                if (!db.DirectRequestToDatabase(p => p.DeleteSympathyDTO.Any(p => p.Id == id))) break;  //Проверка по базе на уникальность
            }

            DeleteSympathyDTO setRemove = myGender == "Man" ? new DeleteSympathyForManTBL1 { Id = id, MainUserDTOId = new Guid(myId), ApponentID = new Guid(apponentId)}
            : new DeleteSympathyForWomanTBL1{ Id = id, MainUserDTOId = new Guid(myId), ApponentID = new Guid(apponentId) };            
            db.Add(setRemove);

            DeleteEvent(myId, apponentId); //Remove event from events  current user
        }

        //Method: Удаляю пометку о удалении
        private void ClearDeletionLabelAsync(string myId, string apponentId, bool ifChat = false)
        {
            DeleteSympathyDTO myLabel = db.DirectRequestToDatabase(p => p.DeleteSympathyDTO.FirstOrDefault(p => p.MainUserDTOId == new Guid(myId) && p.ApponentID == new Guid(apponentId))); //Get Label current user
            if (myLabel != null) db.DeleteByEntity(myLabel);

            if (ifChat)
            {
                DeleteSympathyDTO apponentLabel = db.DirectRequestToDatabase(p => p.DeleteSympathyDTO.FirstOrDefault(p => p.MainUserDTOId == new Guid(apponentId)  && p.ApponentID == new Guid(myId))); //Get the apponent label 
                if (apponentLabel != null) db.DeleteByEntity(apponentLabel);
            }
        }

        //Method: Получане уникального ID
        private Guid GetUniqEventID()
        {
            while (true)
            {
                Guid id = Guid.NewGuid();
                if (!db.DirectRequestToDatabase(p => p.EventUserDTO.Any(p => p.Id == id))) return id;
            }
        }

        //Method: получаю чат с текущем пользователем
        [HttpGet]
        [Route("{action}/{myId}/{apponentId}")]
        public IActionResult GetCurrentChat(string myId, string apponentId)
        {
           var chat = db.DirectRequestToDatabase(p => p.ChatDTO.Include(p => p.ChatMessages).AsNoTracking().FirstOrDefault(p => p.MainUserDTO
           .Any(p => p.Id == new Guid(apponentId)) && p.MainUserDTO.Any(p => p.Id == new Guid(myId))));

            var options = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };
            return Json(chat, options);            
        }
        [HttpPost]
        [Route("{action}")]
        //Method: добавляет сообщение в чате
        public void AddChatMessage(ChatJson message)
        {
            while (true)    //protection against user movement
            {
                try
                {
                    ChatDTO chat = db.DirectRequestToDatabase(p => p.ChatDTO.FirstOrDefault(p => p.MainUserDTO
                    .Any(p => p.Id == message.MyId) && p.MainUserDTO.Any(p => p.Id == message.ApponentId)));  //get chat from DB

                    if (chat is not null) // add new chat                                        
                        db.Add(new ChatMessage() { ChatDTOId = chat.Id, Message = message.Message, UserName = message.MyId.ToString() });    //add message                       
                    else
                    {
                        chat = new ChatDTO();

                        var apponent = db.DirectRequestToDatabase(p => p.MainUserDTO.FirstOrDefault(p => p.Id == message.ApponentId)); // i do it first, for checks on shift apponent gender
                        apponent.ChatDTO = apponent.ChatDTO ?? new List<ChatDTO>();
                        apponent.ChatDTO.Add(chat);   // add chat to user
                                                      
                        var iAm = db.DirectRequestToDatabase(p => p.MainUserDTO.FirstOrDefault(p => p.Id == message.MyId)); // get users associated with current chat
                        iAm.ChatDTO = iAm.ChatDTO ?? new List<ChatDTO>();
                        iAm.ChatDTO.Add(chat);   // add chat to user
                        db.DirectRequestToDatabase(p => p.SaveChanges());

                        db.Add(new ChatMessage() { ChatDTOId = chat.Id, Message = message.Message, UserName = message.MyId.ToString() });    //add message    
                    }

                    ClearDeletionLabelAsync(message.MyId.ToString(), message.ApponentId.ToString(), true);  //Delete Label about User Delete

                    break;
                }
                catch { }
            }
        }
    }
}
