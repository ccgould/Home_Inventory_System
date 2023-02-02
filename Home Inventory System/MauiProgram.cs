using CommunityToolkit.Maui;
using Home_Inventory_System.ViewModels;
using Home_Inventory_System.Views;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;


namespace Home_Inventory_System;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .ConfigureSyncfusionCore()
            // Initialize the .NET MAUI Community Toolkit by adding the below line of code
            .UseMauiCommunityToolkit()
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Font Awesome 6 Brands-Regular-400.otf", "FAB");
                fonts.AddFont("Font Awesome 6 Free-Regular-400.otf", "FAR");
                fonts.AddFont("Font Awesome 6 Free-Solid-900.otf", "FAS");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif


        var services = builder.Services;

        services.AddSingleton<MainPage>();
        services.AddSingleton<MainPageViewModel>();

        return builder.Build();


    }
}
