using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using System.Collections.ObjectModel;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public sealed class AlunoListViewModel : BaseViewModel
{
    private readonly IAlunoService _service;
    private string _cpfBusca = string.Empty;
    private List<AlunoDto> _todos = [];

    public ObservableCollection<AlunoDto> Alunos { get; } = [];
    public string CpfBusca { get => _cpfBusca; set => SetProperty(ref _cpfBusca, value); }
    public AsyncCommand LoadCommand { get; }
    public AsyncCommand SearchCommand { get; }
    public AsyncCommand ClearCommand { get; }
    public AsyncCommand AddCommand { get; }
    public AsyncCommand EditCommand { get; }
    public AsyncCommand DeleteCommand { get; }

    public AlunoListViewModel(IAlunoService service)
    {
        _service = service;
        Title = "Alunos";
        LoadCommand = new AsyncCommand(_ => LoadAsync());
        SearchCommand = new AsyncCommand(_ => SearchAsync());
        ClearCommand = new AsyncCommand(_ => ClearAsync());
        AddCommand = new AsyncCommand(_ => Shell.Current.GoToAsync("aluno-form"));
        EditCommand = new AsyncCommand(EditAsync);
        DeleteCommand = new AsyncCommand(DeleteAsync);
    }

    private async Task LoadAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            _todos = (await _service.ObterTodosAsync(cts.Token)).ToList();
            CpfBusca = string.Empty;
            Show(_todos);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Erro", $"Não foi possível carregar os alunos. {ex.Message}", "OK");
        }
        finally { IsBusy = false; }
    }

    private async Task SearchAsync()
    {
        if (IsBusy) return;
        var digits = new string(CpfBusca.Where(char.IsDigit).ToArray());
        if (string.IsNullOrWhiteSpace(CpfBusca))
        {
            Show(_todos);
            return;
        }
        if (digits.Length != 11)
        {
            await Shell.Current.DisplayAlertAsync("CPF inválido", "Digite os 11 dígitos do CPF para filtrar a lista.", "OK");
            return;
        }
        try
        {
            IsBusy = true;
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            var aluno = await _service.ObterPorCpfAsync(digits, cts.Token);
            Show(aluno is null ? Enumerable.Empty<AlunoDto>() : new[] { aluno });
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Erro na busca", ex.Message, "OK");
        }
        finally { IsBusy = false; }
    }

    private Task ClearAsync()
    {
        CpfBusca = string.Empty;
        Show(_todos);
        return Task.CompletedTask;
    }

    private void Show(IEnumerable<AlunoDto> items)
    {
        Alunos.Clear();
        foreach (var item in items) Alunos.Add(item);
    }

    private async Task EditAsync(object? parameter)
    {
        if (parameter is AlunoDto aluno)
            await Shell.Current.GoToAsync($"aluno-form?id={aluno.Id}");
    }

    private async Task DeleteAsync(object? parameter)
    {
        if (parameter is not AlunoDto aluno) return;
        var confirmed = await Shell.Current.DisplayAlertAsync("Confirmar exclusão", $"Excluir o cadastro de {aluno.Nome}?", "Excluir", "Cancelar");
        if (!confirmed) return;
        try
        {
            IsBusy = true;
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            if (!await _service.RemoverAsync(aluno.Id, cts.Token))
            {
                await Shell.Current.DisplayAlertAsync("Aviso", "O aluno não foi encontrado para exclusão.", "OK");
                return;
            }
            _todos.RemoveAll(x => x.Id == aluno.Id);
            Show(_todos);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Não foi possível excluir", ex.Message, "OK");
        }
        finally { IsBusy = false; }
    }
}
//Aluno: Túlio Thauã Dutra
