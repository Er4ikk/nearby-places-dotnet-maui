using Microsoft.Extensions.Logging;

namespace MauiProject;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
#if ANDROID || IOS
	.UseMauiMaps()
#elif WINDOWS
    .UseMauiCommunityToolkitMaps("BBUY6Nr3vvNNyTb79dK3LVXPVNmHlviTlm8qE1MfX2suFFsWPNoSJQQJ99BFACYeBjFnlIanAAAgAZMP3cFB")
#endif
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});


#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
