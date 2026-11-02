using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using System.Collections.ObjectModel;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public sealed class MatriculaListViewModel : BaseViewModel
{
    private readonly IMatriculaService _service;
    private List<MatriculaDto> _all = [];
    private string _busca = string.Empty;

    public ObservableCollection<MatriculaDto> Matriculas { get; } = [];
    public string Busca { get => _busca; set => SetProperty(ref _busca, value); }
    public AsyncCommand LoadCommand { get; }
    public AsyncCommand SearchCommand { get; }
    public AsyncCommand ClearCommand { get; }
    public AsyncCommand AddCommand { get; }
    public AsyncCommand EditCommand { get; }
    public AsyncCommand DeleteCommand { get; }

    public MatriculaListViewModel(IMatriculaService service)
    {
        _service = service;
        Title = "Matrículas";
        LoadCommand = new AsyncCommand(_ => LoadAsync());
        SearchCommand = new AsyncCommand(_ => Search());
        ClearCommand = new AsyncCommand(_ => { Busca = string.Empty; Show(_all); return Task.CompletedTask; });
        AddCommand = new AsyncCommand(_ => Shell.Current.GoToAsync("matricula-form"));
        EditCommand = new AsyncCommand(EditAsync);
        DeleteCommand = new AsyncCommand(DeleteAsync);
    }

    private async Task LoadAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
            _all = (await _service.ObterTodosAsync(cts.Token)).OrderByDescending(x => x.DataInicio).ToList();
            await Search();
        }
        catch (Exception ex) { await Shell.Current.DisplayAlertAsync("Erro", $"Não foi possível carregar as matrículas. {ex.Message}", "OK"); }
        finally { IsBusy = false; }
    }

    private Task Search()
    {
        var query = Busca.Trim();
        var digits = new string(query.Where(char.IsDigit).ToArray());
        var result = string.IsNullOrEmpty(query) ? _all : _all.Where(m =>
            (m.AlunoMatricula?.Nome.Contains(query, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
            (digits.Length > 0 && (m.AlunoMatricula?.Cpf.Contains(digits, StringComparison.Ordinal) ?? false)) ||
            m.Id.ToString() == query).ToList();
        Show(result);
        return Task.CompletedTask;
    }

    private void Show(IEnumerable<MatriculaDto> items)
    {
        Matriculas.Clear();
        foreach (var item in items) Matriculas.Add(item);
    }

    private async Task EditAsync(object? parameter)
    {
        if (parameter is MatriculaDto item) await Shell.Current.GoToAsync($"matricula-form?id={item.Id}");
    }

    private async Task DeleteAsync(object? parameter)
    {
        if (parameter is not MatriculaDto item) return;
        if (!await Shell.Current.DisplayAlertAsync("Confirmar exclusão", $"Excluir a matrícula de {item.AlunoMatricula?.Nome}?", "Excluir", "Cancelar")) return;
        try
        {
            IsBusy = true;
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
            if (await _service.RemoverAsync(item.Id, cts.Token)) _all.RemoveAll(x => x.Id == item.Id);
            Show(_all);
        }
        catch (Exception ex) { await Shell.Current.DisplayAlertAsync("Erro", $"Não foi possível excluir a matrícula. {ex.Message}", "OK"); }
        finally { IsBusy = false; }
    }
}
