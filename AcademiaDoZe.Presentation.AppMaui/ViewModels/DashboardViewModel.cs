using AcademiaDoZe.Application.Interfaces;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public sealed class DashboardViewModel : BaseViewModel
{
    private readonly ILogradouroService _logradouroService;
    private readonly IAlunoService _alunoService;
    private readonly IColaboradorService _colaboradorService;
    private readonly IMatriculaService _matriculaService;

    private int _totalLogradouros;
    private int _totalAlunos;
    private int _totalColaboradores;
    private int _totalMatriculas;

    public int TotalLogradouros { get => _totalLogradouros; set => SetProperty(ref _totalLogradouros, value); }
    public int TotalAlunos { get => _totalAlunos; set => SetProperty(ref _totalAlunos, value); }
    public int TotalColaboradores { get => _totalColaboradores; set => SetProperty(ref _totalColaboradores, value); }
    public int TotalMatriculas { get => _totalMatriculas; set => SetProperty(ref _totalMatriculas, value); }

    public AsyncCommand LoadCommand { get; }
    public AsyncCommand OpenLogradourosCommand { get; }

    public DashboardViewModel(
        ILogradouroService logradouroService,
        IAlunoService alunoService,
        IColaboradorService colaboradorService,
        IMatriculaService matriculaService)
    {
        _logradouroService = logradouroService;
        _alunoService = alunoService;
        _colaboradorService = colaboradorService;
        _matriculaService = matriculaService;
        Title = "Dashboard";
        LoadCommand = new AsyncCommand(_ => LoadAsync());
        OpenLogradourosCommand = new AsyncCommand(_ => OpenLogradourosAsync());
    }

    private async Task LoadAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(8));

            var logradourosTask = _logradouroService.ObterTodosAsync(timeout.Token);
            var alunosTask = _alunoService.ObterTodosAsync(timeout.Token);
            var colaboradoresTask = _colaboradorService.ObterTodosAsync(timeout.Token);
            var matriculasTask = _matriculaService.ObterTodosAsync(timeout.Token);
            await Task.WhenAll(logradourosTask, alunosTask, colaboradoresTask, matriculasTask);

            TotalLogradouros = (await logradourosTask).Count();
            TotalAlunos = (await alunosTask).Count();
            TotalColaboradores = (await colaboradoresTask).Count();
            TotalMatriculas = (await matriculasTask).Count();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Não foi possível carregar", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static async Task OpenLogradourosAsync() =>
        await Shell.Current.GoToAsync("//logradouros");
}
