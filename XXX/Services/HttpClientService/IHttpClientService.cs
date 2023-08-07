namespace Services.HttpClientService
{
    public interface IHttpClientService
    {
        public string? JwtToken { get; set; }
        public Task GetJWTTokenAsync(string email, string id);
        public Task<string> GetStringAsync(string controller = null, string action = null, params string[] data);
        public Task<(Entity,string)> GetJSONAsync<Entity>(string controller = null, string action = null, params string[] data);       
        public Task<string> PostJsonGetStringAsync<Entity>(Entity data, string? controller = null, string action = null);       
        public Task<(GetEntity, string)> PostJsonGetJsonAsync<Entity,GetEntity>(Entity data, string controller = null, string action = null);        
    }
}
