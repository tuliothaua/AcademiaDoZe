using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public sealed class LogradouroFormViewModel : BaseViewModel
{
    private readonly ILogradouroService _logradouroService;
    private string _cep = string.Empty;
    private string _nome = string.Empty;
    private string _bairro = string.Empty;
    private string _cidade = string.Empty;
    private string _estado = string.Empty;
    private string _pais = "Brasil";
    private int _logradouroId;

    public string Cep { get => _cep; set => SetProperty(ref _cep, value); }
    public string Nome { get => _nome; set => SetProperty(ref _nome, value); }
    public string Bairro { get => _bairro; set => SetProperty(ref _bairro, value); }
    public string Cidade { get => _cidade; set => SetProperty(ref _cidade, value); }
    public string Estado { get => _estado; set => SetProperty(ref _estado, value); }
    public string Pais { get => _pais; set => SetProperty(ref _pais, value); }
    public bool IsEditMode => _logradouroId > 0;

    public AsyncCommand SaveCommand { get; }
    public AsyncCommand CancelCommand { get; }

    public LogradouroFormViewModel(ILogradouroService logradouroService)
    {
        _logradouroService = logradouroService;
        Title = "Novo Logradouro";
        SaveCommand = new AsyncCommand(_ => SaveAsync());
        CancelCommand = new AsyncCommand(_ => CancelAsync());
    }

    public async Task InitializeAsync(int id)
    {
        _logradouroId = id;
        OnPropertyChanged(nameof(IsEditMode));

        if (id <= 0)
        {
            Title = "Novo Logradouro";
            Cep = string.Empty;
            Nome = "Rua Túlio Thauã Dutra";
            Bairro = string.Empty;
            Cidade = string.Empty;
            Estado = string.Empty;
            Pais = "Brasil";
            return;
        }

        try
        {
            IsBusy = true;
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var item = await _logradouroService.ObterPorIdAsync(id, timeout.Token);
            if (item is null)
            {
                await Shell.Current.DisplayAlertAsync("Aviso", "O Logradouro solicitado não foi encontrado.", "OK");
                await Shell.Current.GoToAsync("..");
                return;
            }

            Title = "Editar Logradouro";
            Cep = item.Cep;
            Nome = item.Nome;
            Bairro = item.Bairro;
            Cidade = item.Cidade;
            Estado = item.Estado;
            Pais = item.Pais;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Erro", $"Não foi possível carregar o Logradouro. {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task CancelAsync() => await Shell.Current.GoToAsync("..");

    private async Task SaveAsync()
    {
        if (IsBusy)
            return;

        var cepDigits = new string(Cep.Where(char.IsDigit).ToArray());
        var errors = new List<string>();
        if (cepDigits.Length != 8) errors.Add("Informe um CEP com 8 dígitos.");
        if (string.IsNullOrWhiteSpace(Nome)) errors.Add("Informe o nome do Logradouro.");
        if (string.IsNullOrWhiteSpace(Bairro)) errors.Add("Informe o bairro.");
        if (string.IsNullOrWhiteSpace(Cidade)) errors.Add("Informe a cidade.");
        if (Estado.Trim().Length != 2 || !Estado.Trim().All(char.IsLetter)) errors.Add("Informe a UF com duas letras, por exemplo SC.");
        if (string.IsNullOrWhiteSpace(Pais)) errors.Add("Informe o país.");

        if (errors.Count > 0)
        {
            await Shell.Current.DisplayAlertAsync("Confira os campos", string.Join("\n", errors), "OK");
            return;
        }

        var dto = new LogradouroDto
        {
            Id = _logradouroId,
            Cep = cepDigits,
            Nome = Nome.Trim(),
            Bairro = Bairro.Trim(),
            Cidade = Cidade.Trim(),
            Estado = Estado.Trim().ToUpperInvariant(),
            Pais = Pais.Trim()
        };

        try
        {
            IsBusy = true;
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            if (IsEditMode)
                await _logradouroService.AtualizarAsync(dto, timeout.Token);
            else
                await _logradouroService.AdicionarAsync(dto, timeout.Token);

            await Shell.Current.DisplayAlertAsync("Salvo", "O Logradouro foi salvo com sucesso.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Erro ao salvar", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
