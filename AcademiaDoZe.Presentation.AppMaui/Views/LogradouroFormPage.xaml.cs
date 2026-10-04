using AcademiaDoZe.Presentation.AppMaui.ViewModels;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class LogradouroFormPage : ContentPage, IQueryAttributable
{
    private readonly LogradouroFormViewModel _viewModel;
    private int _logradouroId;
    private bool _initialized;

    public LogradouroFormPage(LogradouroFormViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        _logradouroId = 0;
        if (query.TryGetValue("id", out var value))
            int.TryParse(value?.ToString(), out _logradouroId);
        _initialized = false;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_initialized)
            return;

        _initialized = true;
        await _viewModel.InitializeAsync(_logradouroId);
    }
}
