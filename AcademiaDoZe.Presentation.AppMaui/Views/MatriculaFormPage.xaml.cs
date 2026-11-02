using AcademiaDoZe.Presentation.AppMaui.ViewModels;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

[QueryProperty(nameof(MatriculaId), "id")]
public partial class MatriculaFormPage : ContentPage
{
    private readonly MatriculaFormViewModel _viewModel;
    private int _matriculaId;
    public string MatriculaId { get => _matriculaId.ToString(); set { if (int.TryParse(value, out var id)) _matriculaId = id; } }
    public MatriculaFormPage(MatriculaFormViewModel viewModel)
    {
        InitializeComponent(); _viewModel = viewModel; BindingContext = viewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing(); await _viewModel.InitializeAsync(_matriculaId);
    }
}
