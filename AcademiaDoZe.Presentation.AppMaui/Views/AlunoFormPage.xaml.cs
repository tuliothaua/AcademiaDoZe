using AcademiaDoZe.Presentation.AppMaui.ViewModels;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class AlunoFormPage : ContentPage, IQueryAttributable
{
    private readonly AlunoFormViewModel _viewModel;
    private int _alunoId;
    private bool _initialized;

    public AlunoFormPage(AlunoFormViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        _alunoId = 0;
        if (query.TryGetValue("id", out var value))
            int.TryParse(value?.ToString(), out _alunoId);
        _initialized = false;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_initialized)
        {
            try { await _viewModel.ReloadAddressesAsync(); }
            catch (Exception ex) { await DisplayAlertAsync("Endereços", $"Não foi possível atualizar a lista de logradouros. {ex.Message}", "OK"); }
            return;
        }
        _initialized = true;
        await _viewModel.InitializeAsync(_alunoId);
    }
}
//Aluno: Túlio Thauã Dutra
