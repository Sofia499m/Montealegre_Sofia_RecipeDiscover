using Microsoft.Extensions.Logging;
using Montealegre_Sofia_RecipeDiscover.Services;

namespace Montealegre_Sofia_RecipeDiscover
{
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
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
					fonts.AddFont("fa-solid-900.ttf", "FontAwesomeSolid");
					fonts.AddFont("fa-regular-400.ttf", "FontAwesomeRegular");
				});
			builder.Services.AddSingleton<RecipeStoreService>();
			builder.Services.AddSingleton<SettingsService>();

#if DEBUG
			builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
