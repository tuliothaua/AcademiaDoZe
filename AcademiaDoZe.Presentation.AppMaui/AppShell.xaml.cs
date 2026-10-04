using AcademiaDoZe.Presentation.AppMaui.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui;

public partial class AppShell : Shell
{
    public AppShell(IServiceProvider services)
    {
        InitializeComponent();
        Title = "Academia do Zé";
        FlyoutBehavior = FlyoutBehavior.Flyout;
        FlyoutHeader = CriarCabecalho();

        Items.Add(CriarItem("Dashboard", "dashboard", "dashboard.png",
            () => services.GetRequiredService<DashboardPage>()));
        Items.Add(CriarItem("Logradouros", "logradouros", "location.png",
            () => services.GetRequiredService<LogradouroListPage>()));
        Items.Add(CriarItem("Configurações", "configuracoes", "settings.png",
            () => services.GetRequiredService<ConfigPage>()));

        Routing.RegisterRoute("logradouro-form", typeof(LogradouroFormPage));
    }

    private static FlyoutItem CriarItem(string title, string route, string icon, Func<Page> pageFactory)
    {
        var content = new ShellContent
        {
            Title = title,
            Route = route,
            Icon = ImageSource.FromFile(icon),
            ContentTemplate = new DataTemplate(() => pageFactory())
        };

        var item = new FlyoutItem { Title = title };
        item.Items.Add(content);
        return item;
    }

    private static View CriarCabecalho() => new Grid
    {
        Padding = new Thickness(20, 34, 20, 18),
        BackgroundColor = Color.FromArgb("#173653"),
        Children =
        {
            new VerticalStackLayout
            {
                Spacing = 4,
                Children =
                {
                    new Label { Text = "ACADEMIA DO ZÉ", TextColor = Colors.White, FontSize = 18, FontAttributes = FontAttributes.Bold },
                    new Label { Text = "Gestão de academia", TextColor = Color.FromArgb("#A8DADC"), FontSize = 13 }
                }
            }
        }
    };
}
