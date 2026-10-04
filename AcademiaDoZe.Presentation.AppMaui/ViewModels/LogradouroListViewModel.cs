using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public sealed class LogradouroListViewModel : BaseViewModel
{
    private readonly ILogradouroService _logradouroService;
    private List<LogradouroDto> _todos = [];
    private string _searchText = string.Empty;

    public ObservableCollection<LogradouroDto> Logradouros { get; } = [];

    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    public AsyncCommand LoadCommand { get; }
    public AsyncCommand SearchCommand { get; }
    public AsyncCommand ClearSearchCommand { get; }
    public AsyncCommand AddCommand { get; }
    public AsyncCommand EditCommand { get; }
    public AsyncCommand DeleteCommand { get; }

    public LogradouroListViewModel(ILogradouroService logradouroService)
    {
        _logradouroService = logradouroService;
        Title = "Logradouros";
        LoadCommand = new AsyncCommand(_ => LoadAsync());
        SearchCommand = new AsyncCommand(_ => ApplySearchAsync());
        ClearSearchCommand = new AsyncCommand(_ => ClearSearchAsync());
        AddCommand = new AsyncCommand(_ => NavigateToFormAsync());
        EditCommand = new AsyncCommand(EditAsync);
        DeleteCommand = new AsyncCommand(DeleteAsync);
    }

    private async Task LoadAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            _todos = (await _logradouroService.ObterTodosAsync(timeout.Token)).ToList();
            UpdateVisibleItems();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Erro", $"Não foi possível carregar os Logradouros. {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private Task ApplySearchAsync()
    {
        UpdateVisibleItems();
        return Task.CompletedTask;
    }

    private Task ClearSearchAsync()
    {
        SearchText = string.Empty;
        UpdateVisibleItems();
        return Task.CompletedTask;
    }

    private void UpdateVisibleItems()
    {
        var query = SearchText.Trim();
        var filtered = string.IsNullOrWhiteSpace(query)
            ? _todos
            : _todos.Where(item =>
                item.Nome.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                item.Bairro.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                item.Cidade.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                item.Cep.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                item.Estado.Contains(query, StringComparison.CurrentCultureIgnoreCase)).ToList();

        Logradouros.Clear();
        foreach (var item in filtered)
            Logradouros.Add(item);
    }

    private static async Task NavigateToFormAsync() => await Shell.Current.GoToAsync("logradouro-form");

    private async Task EditAsync(object? parameter)
    {
        if (parameter is not LogradouroDto item)
            return;

        await Shell.Current.GoToAsync($"logradouro-form?id={item.Id}");
    }

    private async Task DeleteAsync(object? parameter)
    {
        if (parameter is not LogradouroDto item)
            return;

        var confirmed = await Shell.Current.DisplayAlertAsync(
            "Confirmar exclusão",
            $"Deseja excluir o Logradouro ‘{item.Nome}’?",
            "Excluir",
            "Cancelar");
        if (!confirmed)
            return;

        try
        {
            IsBusy = true;
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var removed = await _logradouroService.RemoverAsync(item.Id, timeout.Token);
            if (!removed)
            {
                await Shell.Current.DisplayAlertAsync("Aviso", "O registro não foi encontrado para exclusão.", "OK");
                return;
            }

            _todos.RemoveAll(x => x.Id == item.Id);
            UpdateVisibleItems();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Não foi possível excluir",
                $"O Logradouro pode estar vinculado a um aluno ou colaborador. {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
