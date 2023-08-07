using Models.DataModel.ChatEventModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Models.DataModels.ChatMessage
{
    public class ChatMessage
    {
        [Key]
        [JsonIgnore]
        public Guid Id { get; set; } = new();
        public string UserName { get; set; }
        public string Message { get; set; }
        [JsonIgnore]
        public Guid ChatDTOId { get; set; }
        [JsonIgnore]
        public ChatDTO? Chat { get; set; }
    }
}
