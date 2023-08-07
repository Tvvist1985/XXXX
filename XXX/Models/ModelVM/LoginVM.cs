using System.Text.Json.Serialization;

namespace Models.ModelVM
{
    public class LoginVM
    {        
        public string Id { get; set; }                   
        public string? Email { get; set; }       
        public string? Gender { get; set; }
        [JsonIgnore]
        public string? Password { get; set; }
        [JsonIgnore]
        public string? ErrorMessageMailChld { get; set; }
        [JsonIgnore]
        public string? ErrorMessagePasswordChld { get; set; }
    }
}
