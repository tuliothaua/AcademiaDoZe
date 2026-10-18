using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Presentation.AppMaui.ViewModels;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class AlunoListPage : ContentPage
{
    private readonly AlunoListViewModel _viewModel;

    public AlunoListPage(AlunoListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadCommand.ExecuteAsync();
    }

    private async void OnEditButtonClicked(object? sender, EventArgs e)
    {
        if (sender is Button { BindingContext: AlunoDto aluno })
            await _viewModel.EditCommand.ExecuteAsync(aluno);
    }

    private async void OnDeleteButtonClicked(object? sender, EventArgs e)
    {
        if (sender is Button { BindingContext: AlunoDto aluno })
            await _viewModel.DeleteCommand.ExecuteAsync(aluno);
    }
}
//Aluno: Túlio Thauã Dutra
