using Models.DataModels.ChatMessage;
using Models.ModelVM;
using static Services.ChatService.ChatClient;

namespace Services.ChatService
{
    public interface IChatClient
    {
        public LinkedList<Message> _messages { get; set; }      
        public DataForCuby DataForXXX { get; set; }  
        public bool OnChat { get; set; }
        public bool OnChatCrutch { get; set; }
        public string _message { get; set; }
        public string _newMessage { get; set; }       
        public void Chat(string JWT);
        public void EnterChat(List<ChatMessage> chatMessages, string apponentId);
        public void SendMessage();
        public void OutChat();
    }
}
