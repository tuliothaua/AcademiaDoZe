using AcademiaDoZe.Presentation.AppMaui.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Hosting;
#if WINDOWS
using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml;
using Windows.UI.Core;
#endif

namespace AcademiaDoZe.Presentation.AppMaui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();
        ConfigurationHelper.ConfigureServices(builder.Services);

#if DEBUG
        builder.Logging.AddDebug();
#endif

#if WINDOWS
        // Mapeia o cursor de ponteiro "mão" para todos os Buttons no Windows
        ButtonHandler.Mapper.AppendToMapping("HandCursor", (handler, view) =>
        {
            // Ao entrar/saír do botão, ajusta o cursor da janela para mão/seta.
            if (handler.PlatformView is Microsoft.UI.Xaml.Controls.Button)
            {
                void Entered(object s, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
                {
                    try
                    {
                        var w = Microsoft.UI.Xaml.Window.Current;
                        if (w?.CoreWindow != null)
                            w.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Hand, 1);
                    }
                    catch { }
                }

                void Exited(object s, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
                {
                    try
                    {
                        var w = Microsoft.UI.Xaml.Window.Current;
                        if (w?.CoreWindow != null)
                            w.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
                    }
                    catch { }
                }

                // Remap eventos no handler.PlatformView (adiciona duplicidade mínima)
                if (handler.PlatformView is Microsoft.UI.Xaml.Controls.Button nativeButton)
                {
                    nativeButton.PointerEntered += Entered;
                    nativeButton.PointerExited += Exited;
                }
            }
        });
#endif

        return builder.Build();
    }
}
