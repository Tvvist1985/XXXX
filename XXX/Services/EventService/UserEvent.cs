using Services.CubyServices.CubyCollectionSR;
using Models.JsonTpansportModel;
using Services.HttpClientService;
using Services.HttpClientService.AddressesControllersAndActions;

namespace Services.EventService
{
    public class UserEvent : IUserEvent
    {                 
        public ICubyCollection cubyCollection { get; set; }
        private readonly IHttpClientService iHttpClientService;
  
        public string Icon { get; set; } = "heart-not-like";
        public string LikeStyle { get; set; } = default;
        public string MessageAboutLikes { get; set; } = default;              
        public UserEvent(ICubyCollection cubyCollection, IHttpClientService iHttpClientService, Guid id, byte numberContainer)
        {
            this.cubyCollection = cubyCollection;
            this.iHttpClientService = iHttpClientService;           
            //Load first like
            LoadIcon(id, numberContainer);
        }
        
        //Method: Загрузка иконки (есть или нету совпадений)
        public void LoadIcon(Guid apponentId, byte numberContainer)
        {            
            var myEvent = cubyCollection.DataForXXX.MyEvents.Any(p => p.ApponentID == apponentId); //Получаю из коллекций событий события связанные.
            var apponentEvent = cubyCollection.UsersContainers[numberContainer].Any(p => p.EventUserJs?.ApponentID == cubyCollection.DataForXXX.ApponentDataJson.Id);

            if (myEvent && apponentEvent) LikeStyle = "heart-double-like"; //Возвращаю вид иконки исходя из совпадений
            else LikeStyle = myEvent ? "heart-like" : Icon;                           
        }

        //Method: Загрузка иконки лайка для страници контактов
        public string LoadIcon(MainDataJson UserModel)
        {
            //Получаю из коллекций событий события связанные.
            var myEvent = cubyCollection.DataForXXX.MyEvents.Any(p => p.ApponentID == UserModel.Id);
            var userEvent = UserModel.EventUserDTO?.FirstOrDefault(p => p.ApponentID == cubyCollection.DataForXXX.ApponentDataJson.Id);

            //Возвращаю вид иконки исходя из совпадений
            if (myEvent && userEvent is not null)
                return "heart-double-like";
            else
                return myEvent ? "heart-like" : Icon;
        }

        //Method: Создаёт обьект  или удаляю с лайком 
        public void CreateOrDeleteEvent(MainDataJson user, short numberContainer = -1)
        {
            //Если нету  лайка
            if (!cubyCollection.DataForXXX.MyEvents.Any(p => p.ApponentID == user.Id))
            {
                if (cubyCollection.DataForXXX.monetizedData.Likes > 0)
                {
                    cubyCollection.DataForXXX.monetizedData.Likes--; //Вычитаю лайк

                    iHttpClientService.GetStringAsync(ChatAndEvent.Controller, ChatAndEvent.AddContacts, cubyCollection.DataForXXX.ApponentDataJson.Id.ToString()    //Сохраняю в базе новый контакт
                        , user.Id.ToString(), cubyCollection.DataForXXX.ApponentDataJson.MyGender);

                    cubyCollection.DataForXXX.MyEvents.Add(new EventUserJson() { ApponentID = user.Id });  //сохраняю в коллекцию событий текущего пользователя

                    if (!cubyCollection.DataForXXX.MyContacts.Any(p => p.Id == user.Id))  //Добавляю пользователя в контакты                                            
                        cubyCollection.DataForXXX.MyContacts.Add(user);
                    
                    CreateIcon(cubyCollection.DataForXXX.ApponentDataJson.Id, user); // создания лайка                  
                }
                else
                {
                    MessageAboutLikes = "Your likes are over.";
                    if (numberContainer != -1)
                        LikeStyle = Icon;
                }                
            }
            else //Delete
            {
                iHttpClientService.GetStringAsync(ChatAndEvent.Controller, ChatAndEvent.DeleteEvent, cubyCollection.DataForXXX.ApponentDataJson.Id.ToString()       //Удаляю событие из базы
                    , user.Id.ToString());

                if (cubyCollection.DataForXXX.MyContacts.Any(p => p.Id == user.Id)      //Удаляю пользователя из контактов если контакт был основан только на собитии
                && cubyCollection.DataForXXX.MyContacts.FirstOrDefault(p => p.Id == user.Id)?.Chat is null)
                {
                    cubyCollection.DataForXXX.MyContacts.RemoveAt(cubyCollection.DataForXXX.MyContacts
                        .IndexOf(cubyCollection.DataForXXX.MyContacts.FirstOrDefault(p => p.Id == user.Id)));
                }

                cubyCollection.DataForXXX.MyEvents.RemoveAt(cubyCollection.DataForXXX.MyEvents
                   .IndexOf(cubyCollection.DataForXXX.MyEvents.FirstOrDefault(p => p.ApponentID == user.Id))); //Удаляю из списка событий

                LikeStyle = Icon;
            }
        }
               
        //Method: Создание иконки лайка
        private void CreateIcon(Guid myId, MainDataJson user)
        {
            //Находим события связанные с текущим пользователем в событиях аппонента
            var apponentEvent = user.EventUserJs?.ApponentID == myId;
            LikeStyle = apponentEvent ? "heart-double-like" : "heart-like";
        }

        //EVENT: Удаление пользователя из контактов
        public void DeleteContact(MainDataJson user)
        {
            cubyCollection.DataForXXX.MyContacts.RemoveAt(cubyCollection.DataForXXX.MyContacts //Delete user from contacts
                .IndexOf(cubyCollection.DataForXXX.MyContacts.FirstOrDefault(p => p.Id == user.Id)));

            int index = cubyCollection.DataForXXX.MyEvents.IndexOf(cubyCollection.DataForXXX.MyEvents.FirstOrDefault(p => p.ApponentID == user.Id)); //Удаляю из списка событий
            if (index != -1) cubyCollection.DataForXXX.MyEvents.RemoveAt(index);

            iHttpClientService.GetStringAsync(ChatAndEvent.Controller, ChatAndEvent.AddDeletionLabel,     //add to DB
                cubyCollection.DataForXXX.ApponentDataJson.Id.ToString(), user.Id.ToString(), cubyCollection.DataForXXX.ApponentDataJson.MyGender);

            LikeStyle = Icon;
        }
    }
}
