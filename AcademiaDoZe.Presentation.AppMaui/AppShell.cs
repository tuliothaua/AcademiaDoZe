using AcademiaDoZe.Presentation.AppMaui.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace AcademiaDoZe.Presentation.AppMaui;

public sealed partial class AppShell : Shell
{
    // Construtor movido para AppShell.xaml.cs para evitar duplicação de membros.
}

internal sealed class ServiceRouteFactory<TPage> : RouteFactory
    where TPage : Element
{
    private readonly IServiceProvider _services;
    public ServiceRouteFactory(IServiceProvider services) => _services = services;
    public override Element GetOrCreate() => _services.GetRequiredService<TPage>();

    public override Element GetOrCreate(IServiceProvider services) => (services ?? _services).GetRequiredService<TPage>();
}
