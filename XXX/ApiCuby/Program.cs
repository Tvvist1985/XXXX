using ApiCuby;
using ApiCuby.JwtToken;
using APIService.GetUsersServices;
using ApiServices.DataDB.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models.ModelVM;
using Services.DataDB.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>(); // Устанавливаем сервис для получения Id пользователя
builder.Services.AddAuthorization();//Авторизация и аунтефикация JWTToken
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {            
            ValidateIssuer = true,// указывает, будет ли валидироваться издатель при валидации токена           
            ValidIssuer = AddJwtToken.ISSUER, // строка, представляющая издателя          
            ValidateAudience = true,  // будет ли валидироваться потребитель токена            
            ValidAudience = AddJwtToken.AUDIENCE,// установка потребителя токена           
            ValidateLifetime = true, // будет ли валидироваться время существования          
            IssuerSigningKey = AddJwtToken.GetSymmetricSecurityKey(),  // установка ключа безопасности          
            ValidateIssuerSigningKey = true          // валидация ключа безопасности   
        };
        options.Events = new JwtBearerEvents //JWT token для чата
        {
            OnMessageReceived = context =>
            {               
                var accessToken = context.Request.Query["access_token"]; //получаю токен из коллекции значений запроса               
                var path = context.HttpContext.Request.Path; // если запрос направлен хабу
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chat"))                                    
                    context.Token = accessToken;// получаем токен из строки запроса
                
                return Task.CompletedTask;
            }
        };
    });
//////////////////////////////////////////////////////////////////////////

//Конфигурация ХАБА (чата)
builder.Services.AddSignalR().AddHubOptions<ChatHub>(options =>
{
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(20);
    options.MaximumParallelInvocationsPerClient = 2;
});
//////////////////////////////////////////////////////////////////

builder.Services.AddControllers();// Add services to the container.

var connect = builder.Configuration.GetConnectionString("DBConnection");// подключение к My SQL
builder.Services.AddDbContextPool<AppDbContext>(options => options.UseMySql(connect
    , ServerVersion.AutoDetect(connect)));

//репозиторий
builder.Services.AddScoped<IStoreDbRepository, SQLStoreDbRepository>();
builder.Services.AddTransient<DataForCuby>();
builder.Services.AddTransient<IGetUsers, GetUsers>();

var app = builder.Build();

app.UseHttpsRedirection();  

//их подрубать после Роутенга   и до маршрутов
//Добавление мидл вее аунтефикации и авторизации (обязательны)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/chat");  //Адрес чата

app.Run();
