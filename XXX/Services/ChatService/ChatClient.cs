using Microsoft.AspNetCore.SignalR.Client;
using Models.DataModel.ChatEventModel;
using Models.DataModels.ChatMessage;
using Models.JsonTpansportModel;
using Models.ModelVM;
using Services.HttpClientService;
using Services.HttpClientService.AddressesControllersAndActions;

namespace Services.ChatService
{
    public class ChatClient : IChatClient
    {
        private const string DOM = "https://localhost:44309";
        private readonly IHttpClientService httpClientService;
        public DataForCuby DataForXXX { get; set; } //Для изменения чата в коллециях             
        public string _message { get; set; } // on-screen message
        public string _newMessage { get; set; } // new message input
        private bool Crutch { get; set; }  //Правлю баг фреймворка     
        public LinkedList<Message> _messages { get; set; } = new LinkedList<Message>(); // list of messages in chat                                                           
        private HubConnection _hubConnection { get; set; }
        public bool OnChat { get; set; }
        public bool OnChatCrutch { get; set; }
        private  string ApponentId { get; set; }
        public ChatClient(IHttpClientService httpClientService)
        {
            this.httpClientService = httpClientService;
        }

        //Method:Регистрация пользователя в чате
        public void Chat(string JWT)
        {
            try
            {
                var _hubUrl = DOM + "/chat?access_token=" + JWT;//подрубаюсь к хабу
                _hubConnection = new HubConnectionBuilder()
                .WithUrl(_hubUrl)
                .Build();

                _hubConnection.On<string, string, string>("Receive", BroadcastMessage);  //Регистрация метода  который будет получать данные от сервера(он ниже)                  
                _hubConnection.StartAsync().Wait();  //Начало соединения с сервером                    
            }
            catch (Exception e)
            {
                _message = $"ERROR: Failed to start chat client: {e.Message}";
            }
        }

        //Method: Получаю чат c текущем аппонентом
        public void EnterChat(List<ChatMessage> chatMessages, string apponentId)
        {
            _messages = new LinkedList<Message>();  //делаю именно так из за того что диспонс включаеться после инициализации  если выходить с включенным чатом из  списка контактов

            OnChat = true; //оповещатель что пользователь в чате
            ApponentId = apponentId; // инициализирую id собеседника

            if (chatMessages is not null) //инициализирую чат
                foreach (var message in chatMessages)
                    _messages.AddLast(new Message(message.UserName, message.Message));
        }

        //Method: Отправляет сообщение на сервер //ВАЖНО ДАННЫЙ МЕТОД ДОЛЖЕН ЗАКОНЧИТЬСЯ ПОСЛЕ МЕТОДА ОТПРАВКИ С СЕРВЕРА     
        public void SendMessage()
        {
            if (!string.IsNullOrWhiteSpace(_newMessage))
            {
                Crutch = false; //костыль против бага  Отключаю потому что  при полученном сообщении от аппонента  он будет включен

                _hubConnection.SendAsync("Broadcast", _newMessage, ApponentId).Wait(); //Сообщение на серв

                AddMessageToDb(_newMessage); //save message to db
                _newMessage = string.Empty;

                while (!Crutch) { }  //Костыль   почемуто метод выше не ожидаеться
                Crutch = false; //снова отключаю
            }
        }

        //Method: Получает сообщения от сервера
        private void BroadcastMessage(string message, string mainName, string secondName)
        {
            if (OnChat && mainName == ApponentId || OnChat && secondName == ApponentId) //добавляю сообщение  если чат открыт и осообщение от одного из собеседников            
                _messages.AddLast(new Message(mainName, message));  //добавляю сообщение  на страницу
            
            if (mainName == DataForXXX.ApponentDataJson.Id.ToString()) // выбор от кого сообщение
                AddMessageToCollections(secondName, new Message(mainName, message), true); //Current user
            else
                AddMessageToCollections(mainName, new Message(mainName, message)); //Apponent

            Crutch = true;  //Заглушка от бага           
        }

        //Method: Сохраняю по коллекциям сообщение
        private void AddMessageToCollections(string ApponentName, Message message, bool currentUserMessage = false)
        {
            MainDataJson userFromContacts = default;  //добавляю сообщение или контакт
            MainDataJson userFromxxx = default;   //получаю пользователя из коллекции

            Parallel.Invoke(
                () => userFromContacts = AddNewChatOrContact(ApponentName, message),
                () => userFromxxx = AddNewChatToXXX(ApponentName, message));
            
            if (currentUserMessage && userFromContacts is null) // добавляю новый контакт сообщение было от текущего пользователя
                DataForXXX.MyContacts.Add(userFromxxx);
            else if(userFromxxx is not null && userFromContacts is null)   // add user to apponent contacts
                DataForXXX.MyContacts.Add(userFromxxx);
        }

        private MainDataJson AddNewChatOrContact(string name, Message message)
        {
            MainDataJson userFromContacts = DataForXXX.MyContacts.FirstOrDefault(p => p.Id == new Guid(name)); //получаю пользователя из контактов

            if (userFromContacts is not null)        //добавляю сообщение
            {
                userFromContacts.Chat?.ChatMessages.Add(new ChatMessage { UserName = message.Username, Message = message.Body }); // добавляю только сообщение                
                if(userFromContacts.Chat is null)                                               //Создаю новый чат  и добавляю сообщение
                {
                    userFromContacts.Chat = new ChatDTO();
                    userFromContacts.Chat.ChatMessages.Add(new ChatMessage { UserName = message.Username, Message = message.Body });
                }
            }
            return userFromContacts;
        }
        //Method: добавляю чат в коллекции 
        private MainDataJson AddNewChatToXXX(string name, Message message)
        {
            MainDataJson userFromXXX = DataForXXX.ManListJson.FirstOrDefault(p => p.Id == new Guid(name));    //получаю пользователя из коллекции XXX                   
            userFromXXX = userFromXXX ?? DataForXXX.WomanListJson.FirstOrDefault(p => p.Id == new Guid(name));

            if (userFromXXX is not null) //check there is user in a Cu
            {
                userFromXXX.Chat?.ChatMessages.Add(new ChatMessage { UserName = message.Username, Message = message.Body });  // добавляю только сообщение
                if (userFromXXX.Chat is null)     //Создаю новый чат  и добавляю сообщение
                {
                    userFromXXX.Chat = new ChatDTO();
                    userFromXXX.Chat.ChatMessages.Add(new ChatMessage { UserName = message.Username, Message = message.Body });
                }
            }

            return userFromXXX;
        }

        //Method:сохраняю сообщение в базу и коллекции
        private void AddMessageToDb(string newMess)
        {
            Task.Run(() =>
            {
                ChatJson message = new ChatJson(DataForXXX.ApponentDataJson.Id, new Guid(ApponentId), newMess);  //initializing Chat Json for request
                httpClientService.PostJsonGetStringAsync(message, ChatAndEvent.Controller, ChatAndEvent.AddChatMessage); // sending to server
            });
        }

        //Method: выход из чата
        public void OutChat()
        {
            if (!OnChatCrutch)
            {
                OnChat = false;
                ApponentId = default;
            }

            OnChatCrutch = false;  //Костыль   Disponce срабатывает позже че инициальзатор компонента
        }

        public class Message
        {
            public Message(string username, string body)
            {
                Username = username;
                Body = body;
            }

            public string Username { get; set; }
            public string Body { get; set; }
            public bool IsNotice => Body.StartsWith("[Notice]");
        }
    }
}

