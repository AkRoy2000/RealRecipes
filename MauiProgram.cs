using CommunityToolkit.Maui;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Hosting;
using The49.Maui.BottomSheet;
using Microcharts.Maui;

namespace RealRecipes;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder

            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMicrocharts()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Inter-Regular.ttf", "InterRegular");
                fonts.AddFont("Inter-SemiBold.ttf", "InterSemiBold");
                fonts.AddFont("Inter-Bold.ttf", "InterBold");
            });
        AndroidHandlers.Apply();

        return builder.Build();
    }
}