using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Presentation.AppMaui.ViewModels;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class ColaboradorListPage : ContentPage
{
    private readonly ColaboradorListViewModel _viewModel;

    public ColaboradorListPage(ColaboradorListViewModel viewModel)
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
        if (sender is Button { BindingContext: ColaboradorDto colaborador })
            await _viewModel.EditCommand.ExecuteAsync(colaborador);
    }

    private async void OnDeleteButtonClicked(object? sender, EventArgs e)
    {
        if (sender is Button { BindingContext: ColaboradorDto colaborador })
            await _viewModel.DeleteCommand.ExecuteAsync(colaborador);
    }
}
//Aluno: Túlio Thauã Dutra
