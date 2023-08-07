using Services.ChatService;
using Services.DateTimeService;
using Services.HttpClientService;
using Services.OtherActionWithCubySR;

namespace Cuby;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
		#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
        
		
        builder.Services.AddHttpClient(); //Фабрика HTTPClient
		//Моисервисы	
		builder.Services.AddScoped<IHttpClientService, HttpClientSR>();
		builder.Services.AddTransient<BirthdayValidationService>();
        builder.Services.AddScoped<IChatClient, ChatClient>();          
        builder.Services.AddScoped<IOtherActionWithCuby, OtherActionWithCuby>();
#endif

        return builder.Build();
	}
}
