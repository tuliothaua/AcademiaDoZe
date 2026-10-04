using AcademiaDoZe.Application.DependencyInjection;
using AcademiaDoZe.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace AcademiaDoZe.Presentation.AppMaui;

public partial class App : Microsoft.Maui.Controls.Application
{
    private readonly IServiceProvider _services;
    private readonly RepositoryConfig _repositoryConfig;

    public App(IServiceProvider services, RepositoryConfig repositoryConfig)
    {
        InitializeComponent();
        _services = services;
        _repositoryConfig = repositoryConfig;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(CreateLoadingPage());
        _ = InitializeAndOpenAsync(window);
        return window;
    }

    private static ContentPage CreateLoadingPage() => new()
    {
        BackgroundColor = Color.FromArgb("#F4F7FB"),
        Content = new VerticalStackLayout
        {
            Spacing = 16,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Children =
            {
                new ActivityIndicator
                {
                    IsRunning = true,
                    Color = Color.FromArgb("#149A98"),
                    WidthRequest = 44,
                    HeightRequest = 44
                },
                new Label
                {
                    Text = "Preparando a Academia do Zé...",
                    TextColor = Color.FromArgb("#173653"),
                    FontSize = 18,
                    HorizontalTextAlignment = TextAlignment.Center
                }
            }
        }
    };

    private async Task InitializeAndOpenAsync(Window window)
    {
        try
        {
            await DbInitializer.InicializarAsync(
                _repositoryConfig.ConnectionString,
                _repositoryConfig.DatabaseType);

            await MainThread.InvokeOnMainThreadAsync(() =>
                window.Page = _services.GetRequiredService<AppShell>());
        }
        catch (Exception ex)
        {
            await MainThread.InvokeOnMainThreadAsync(() => window.Page = CreateErrorPage(window, ex));
        }
    }

    private ContentPage CreateErrorPage(Window window, Exception exception)
    {
        var retryButton = new Button
        {
            Text = "Tentar novamente",
            BackgroundColor = Color.FromArgb("#149A98"),
            TextColor = Colors.White,
            CornerRadius = 12
        };
        retryButton.Clicked += async (_, _) =>
        {
            window.Page = CreateLoadingPage();
            await InitializeAndOpenAsync(window);
        };

        return new ContentPage
        {
            BackgroundColor = Color.FromArgb("#F4F7FB"),
            Content = new VerticalStackLayout
            {
                Padding = 28,
                Spacing = 14,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    new Label
                    {
                        Text = "Não foi possível preparar o banco de dados.",
                        FontSize = 22,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.FromArgb("#173653")
                    },
                    new Label
                    {
                        Text = exception.Message,
                        FontSize = 14,
                        TextColor = Color.FromArgb("#667085")
                    },
                    retryButton
                }
            }
        };
    }
}
