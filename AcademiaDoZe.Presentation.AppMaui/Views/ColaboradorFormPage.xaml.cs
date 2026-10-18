using AcademiaDoZe.Presentation.AppMaui.ViewModels;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class ColaboradorFormPage : ContentPage, IQueryAttributable
{
    private readonly ColaboradorFormViewModel _viewModel;
    private int _colaboradorId;
    private bool _initialized;

    public ColaboradorFormPage(ColaboradorFormViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        _colaboradorId = 0;
        if (query.TryGetValue("id", out var value))
            int.TryParse(value?.ToString(), out _colaboradorId);
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
        await _viewModel.InitializeAsync(_colaboradorId);
    }
}
//Aluno: Túlio Thauã Dutra
