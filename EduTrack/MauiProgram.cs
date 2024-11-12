using EduTrack.DB_Models;
//using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;

namespace EduTrack
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "EduTrack.db");
            AppConfig.DbPath = dbPath;
            builder.Services.AddSingleton(dbPath);
            builder.Services.AddSingleton<App>();
            builder.Services.AddTransient<TermListPage>();

            builder
                
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .UseLocalNotification();

            return builder.Build();
        }
    }
}
