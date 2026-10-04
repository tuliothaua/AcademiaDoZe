using AcademiaDoZe.Presentation.AppMaui.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace AcademiaDoZe.Presentation.AppMaui;

public sealed partial class AppShell : Shell
{
    public AppShell(IServiceProvider services)
    {
        Title = "Academia do Zé";
        FlyoutBehavior = FlyoutBehavior.Flyout;
        FlyoutBackgroundColor = Color.FromArgb("#173653");
        Shell.SetTitleColor(this, Colors.White);
        Shell.SetForegroundColor(this, Colors.White);

        var dashboardItem = new FlyoutItem
        {
            Title = "Dashboard",
            Route = "dashboard"
        };
        var dashboardTab = new Tab { Title = "Visão geral", Route = "dashboard-tab" };
        dashboardTab.Items.Add(new ShellContent
        {
            Title = "Dashboard",
            Route = "dashboard-page",
            ContentTemplate = new DataTemplate(() => services.GetRequiredService<DashboardPage>())
        });
        dashboardItem.Items.Add(dashboardTab);
        Items.Add(dashboardItem);

        var logradourosItem = new FlyoutItem
        {
            Title = "Logradouros",
            Route = "logradouros"
        };
        var logradourosTab = new Tab { Title = "Cadastro e consulta", Route = "logradouros-tab" };
        logradourosTab.Items.Add(new ShellContent
        {
            Title = "Logradouros",
            Route = "logradouros-page",
            ContentTemplate = new DataTemplate(() => services.GetRequiredService<LogradouroListPage>())
        });
        logradourosItem.Items.Add(logradourosTab);
        Items.Add(logradourosItem);

        Routing.RegisterRoute("logradouro-form", new ServiceRouteFactory<LogradouroFormPage>(services));
        CurrentItem = dashboardItem;
    }
}

internal sealed class ServiceRouteFactory<TPage> : RouteFactory
    where TPage : Element
{
    private readonly IServiceProvider _services;
    public ServiceRouteFactory(IServiceProvider services) => _services = services;
public override Element GetOrCreate() => _services.GetRequiredService<TPage>();

public override Element GetOrCreate(IServiceProvider services) => (services ?? _services).GetRequiredService<TPage>();
}
