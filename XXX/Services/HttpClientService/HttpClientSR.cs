using Services.HttpClientService.AddressesControllersAndActions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Services.HttpClientService
{
    public class HttpClientSR : IHttpClientService, IDisposable
    {       
        private const string DOM  = "https://localhost:44309";   
        public string? JwtToken { get; set; }
        private CancellationTokenSource CancelTokenSource { get; set; } = new CancellationTokenSource();
        
        private readonly IHttpClientFactory httpClientFactory;   
        public HttpClientSR(IHttpClientFactory httpClientFactory) { this.httpClientFactory = httpClientFactory; }
        
        //Method: Генерация ссылки
        private string GetURL(string controller = null, string action = null, string[] data = default)
        {
            string Url = DOM;            
            Url += controller;
            Url += action;
            //Добавляю параметры если они есть
            if (data != null)
                foreach (var item in data)
                    Url += "/" + item;

            return Url;
        }
       
        //Method: GET STRING (получаем просто строку с запроса) - Чтобы получить HTML код  нужжен просто метод GetAsync()
        public async Task<string> GetStringAsync(string controller = null, string action = null, params string[] data)
        {
            var httpClient = httpClientFactory.CreateClient();
            //Установка заголовков ВАЖНО    в "Bearer "   ПРОБЕЛ ОБЯЗАТЕЛЕН
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + JwtToken);
            var url = GetURL(controller, action, data);
            
            using var response = await httpClient.GetAsync(url);
            return  await response.Content.ReadAsStringAsync() ;         
        }

        //Method: Получение Json 
        public async Task<(Entity, string)> GetJSONAsync<Entity>(string controller = null, string action = null, params string[] data) 
        {           
            var httpClient = httpClientFactory.CreateClient();
            //Установка заголовков ВАЖНО    в "Bearer "   ПРОБЕЛ ОБЯЗАТЕЛЕН
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + JwtToken);
            var url = GetURL(controller, action, data);                 

            using var response = await httpClient.GetAsync(url);               
            return response.StatusCode == HttpStatusCode.OK ? (await response.Content.ReadFromJsonAsync<Entity>(), default)
                : (default, await response.Content.ReadAsStringAsync());
        }
        
        //Method: ПОСT JSON Get Json
        public async Task<(GetEntity, string)> PostJsonGetJsonAsync<Entity,GetEntity>(Entity data, string controller = null, string action = null)
        {            
            var httpClient = httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + JwtToken);
            var url = GetURL(controller, action);
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };

            using var response = await httpClient.PostAsJsonAsync(url, data, options);
            return response.StatusCode == HttpStatusCode.OK ? (await response.Content.ReadFromJsonAsync<GetEntity>(), default)
                : (default, await response.Content.ReadAsStringAsync());
        }

        //Method: ПОСT JSON Get string
        public async Task<string> PostJsonGetStringAsync<Entity>(Entity data, string controller = null, string action = null)
        {
            var httpClient = httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + JwtToken);
            var url = GetURL(controller, action);
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };

            using var response = await httpClient.PostAsJsonAsync(url, data, options);          
            return await response.Content.ReadAsStringAsync();
        }
        //Method: Получение токена в фоновой Самодельной задичи
        public async Task GetJWTTokenAsync(string email, string id)
        {
            await Task.Run(async () =>
            {               
                CancellationToken token = CancelTokenSource.Token; //создаю токен для выхода               
                while (true) //сдесь 2 цикла чтобы  http удалялся чере фабрику
                {
                    Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                    Thread.Sleep(TimeSpan.FromMinutes(29.3));

                    if (!token.IsCancellationRequested)
                        JwtToken = await GetStringAsync(AboutUsers.Controller, AboutUsers.GetJwtToken, email, id);
                    else break;
                }
            });
        }
        
        public void Dispose()
        {
            //Закрываю фоновую задачу
            CancelTokenSource.Cancel();
            CancelTokenSource.Dispose();
            GC.SuppressFinalize(this);
        }
    }   
}
