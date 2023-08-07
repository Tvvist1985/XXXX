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

builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>(); // ������������� ������ ��� ��������� Id ������������
builder.Services.AddAuthorization();//����������� � ������������ JWTToken
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {            
            ValidateIssuer = true,// ���������, ����� �� �������������� �������� ��� ��������� ������           
            ValidIssuer = AddJwtToken.ISSUER, // ������, �������������� ��������          
            ValidateAudience = true,  // ����� �� �������������� ����������� ������            
            ValidAudience = AddJwtToken.AUDIENCE,// ��������� ����������� ������           
            ValidateLifetime = true, // ����� �� �������������� ����� �������������          
            IssuerSigningKey = AddJwtToken.GetSymmetricSecurityKey(),  // ��������� ����� ������������          
            ValidateIssuerSigningKey = true          // ��������� ����� ������������   
        };
        options.Events = new JwtBearerEvents //JWT token ��� ����
        {
            OnMessageReceived = context =>
            {               
                var accessToken = context.Request.Query["access_token"]; //������� ����� �� ��������� �������� �������               
                var path = context.HttpContext.Request.Path; // ���� ������ ��������� ����
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chat"))                                    
                    context.Token = accessToken;// �������� ����� �� ������ �������
                
                return Task.CompletedTask;
            }
        };
    });
//////////////////////////////////////////////////////////////////////////

//������������ ���� (����)
builder.Services.AddSignalR().AddHubOptions<ChatHub>(options =>
{
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(20);
    options.MaximumParallelInvocationsPerClient = 2;
});
//////////////////////////////////////////////////////////////////

builder.Services.AddControllers();// Add services to the container.

var connect = builder.Configuration.GetConnectionString("DBConnection");// ����������� � My SQL
builder.Services.AddDbContextPool<AppDbContext>(options => options.UseMySql(connect
    , ServerVersion.AutoDetect(connect)));

//�����������
builder.Services.AddScoped<IStoreDbRepository, SQLStoreDbRepository>();
builder.Services.AddTransient<DataForCuby>();
builder.Services.AddTransient<IGetUsers, GetUsers>();

var app = builder.Build();

app.UseHttpsRedirection();  

//�� ��������� ����� ��������   � �� ���������
//���������� ���� ��� ������������ � ����������� (�����������)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/chat");  //����� ����

app.Run();
