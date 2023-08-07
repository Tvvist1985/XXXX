using System.ComponentModel.DataAnnotations;
using Models.DataModel.MainDataUserModel;
using Models.DataModels.ChatMessage;
using System.Text.Json.Serialization;

namespace Models.DataModel.ChatEventModel
{
    public class ChatDTO
    {
        [Key]
        [JsonIgnore]
        public Guid Id { get; set; } = new();
        [JsonIgnore]
        public List<MainUserDTO> MainUserDTO { get; set; }
        public List<ChatMessage>? ChatMessages { get; set; } = new();
    }
}
