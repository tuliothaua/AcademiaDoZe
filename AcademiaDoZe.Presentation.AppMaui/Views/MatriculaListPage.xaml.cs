using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Presentation.AppMaui.ViewModels;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class MatriculaListPage : ContentPage
{
    private readonly MatriculaListViewModel _viewModel;
    public MatriculaListPage(MatriculaListViewModel viewModel)
    {
        InitializeComponent(); _viewModel = viewModel; BindingContext = viewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing(); await _viewModel.LoadCommand.ExecuteAsync();
    }
    private async void OnEditClicked(object? sender, EventArgs e)
    {
        if (sender is Button { BindingContext: MatriculaDto dto }) await _viewModel.EditCommand.ExecuteAsync(dto);
    }
    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (sender is Button { BindingContext: MatriculaDto dto }) await _viewModel.DeleteCommand.ExecuteAsync(dto);
    }
}
