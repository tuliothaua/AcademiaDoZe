using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public sealed class AlunoFormViewModel : BaseViewModel
{
    private readonly IAlunoService _alunoService;
    private readonly ILogradouroService _logradouroService;
    private int _id;
    private string _nome = string.Empty;
    private string _cpf = string.Empty;
    private DateTime _dataNascimento = DateTime.Today.AddYears(-18);
    private string _telefone = string.Empty;
    private string _email = string.Empty;
    private string _numero = string.Empty;
    private string _complemento = string.Empty;
    private string _senha = string.Empty;
    private string _confirmarSenha = string.Empty;
    private LogradouroDto? _enderecoSelecionado;
    private ArquivoDto? _foto;
    private ImageSource? _fotoPreview;

    public ObservableCollection<LogradouroDto> Enderecos { get; } = [];
    public int Id { get => _id; private set => SetProperty(ref _id, value); }
    public bool IsEditMode => Id > 0;
    public string Nome { get => _nome; set => SetProperty(ref _nome, value); }
    public string Cpf { get => _cpf; set => SetProperty(ref _cpf, value); }
    public DateTime DataNascimento { get => _dataNascimento; set => SetProperty(ref _dataNascimento, value); }
    public DateTime DataMaximaNascimento => DateTime.Today.AddYears(-12);
    public string Telefone { get => _telefone; set => SetProperty(ref _telefone, value); }
    public string Email { get => _email; set => SetProperty(ref _email, value); }
    public string Numero { get => _numero; set => SetProperty(ref _numero, value); }
    public string Complemento { get => _complemento; set => SetProperty(ref _complemento, value); }
    public string Senha { get => _senha; set => SetProperty(ref _senha, value); }
    public string ConfirmarSenha { get => _confirmarSenha; set => SetProperty(ref _confirmarSenha, value); }
    public LogradouroDto? EnderecoSelecionado { get => _enderecoSelecionado; set => SetProperty(ref _enderecoSelecionado, value); }
    public ImageSource? FotoPreview { get => _fotoPreview; private set => SetProperty(ref _fotoPreview, value); }

    public AsyncCommand SalvarCommand { get; }
    public AsyncCommand CancelarCommand { get; }
    public AsyncCommand SelecionarFotoCommand { get; }
    public AsyncCommand AbrirLogradourosCommand { get; }

    public AlunoFormViewModel(IAlunoService alunoService, ILogradouroService logradouroService)
    {
        _alunoService = alunoService;
        _logradouroService = logradouroService;
        Title = "Novo aluno";
        SalvarCommand = new AsyncCommand(_ => SalvarAsync());
        CancelarCommand = new AsyncCommand(_ => Shell.Current.GoToAsync(".."));
        SelecionarFotoCommand = new AsyncCommand(_ => SelecionarFotoAsync());
        AbrirLogradourosCommand = new AsyncCommand(_ => Shell.Current.GoToAsync("logradouro-form"));
    }

    public async Task ReloadAddressesAsync()
    {
        var selectedId = EnderecoSelecionado?.Id ?? 0;
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        var items = await _logradouroService.ObterTodosAsync(cts.Token);
        Enderecos.Clear();
        foreach (var endereco in items) Enderecos.Add(endereco);
        EnderecoSelecionado = Enderecos.FirstOrDefault(x => x.Id == selectedId) ?? Enderecos.FirstOrDefault();
    }

    public async Task InitializeAsync(int id)
    {
        if (IsBusy) return;
        Id = id;
        Title = IsEditMode ? "Editar aluno" : "Novo aluno";
        Senha = string.Empty;
        ConfirmarSenha = string.Empty;
        try
        {
            IsBusy = true;
            Enderecos.Clear();
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            foreach (var endereco in await _logradouroService.ObterTodosAsync(cts.Token)) Enderecos.Add(endereco);

            if (!IsEditMode)
            {
                Nome = Cpf = Telefone = Email = Numero = Complemento = string.Empty;
                DataNascimento = DateTime.Today.AddYears(-18);
                _foto = null;
                AtualizarPreview();
                EnderecoSelecionado = Enderecos.FirstOrDefault();
                return;
            }

            var dto = await _alunoService.ObterPorIdAsync(id, cts.Token);
            if (dto is null)
            {
                await Shell.Current.DisplayAlertAsync("Aluno não encontrado", "O registro pode ter sido removido. A lista será atualizada.", "OK");
                await Shell.Current.GoToAsync("..");
                return;
            }
            Nome = dto.Nome;
            Cpf = dto.Cpf;
            DataNascimento = dto.DataNascimento == default ? DateTime.Today.AddYears(-18) : dto.DataNascimento.ToDateTime(TimeOnly.MinValue);
            Telefone = dto.Telefone;
            Email = dto.Email ?? string.Empty;
            Numero = dto.Numero;
            Complemento = dto.Complemento ?? string.Empty;
            _foto = dto.Foto;
            AtualizarPreview();
            EnderecoSelecionado = Enderecos.FirstOrDefault(x => x.Id == dto.Endereco?.Id) ?? dto.Endereco;
            if (EnderecoSelecionado is not null && Enderecos.All(x => x.Id != EnderecoSelecionado.Id)) Enderecos.Add(EnderecoSelecionado);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Erro", $"Não foi possível carregar os dados do aluno. {ex.Message}", "OK");
        }
        finally { IsBusy = false; }
    }

    private async Task SelecionarFotoAsync()
    {
        try
        {
            var resultado = await FotoPicker.SelecionarAsync();
            if (!resultado.Alterada) return;
            _foto = resultado.Foto;
            AtualizarPreview();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Foto", $"Não foi possível obter a imagem. {ex.Message}", "OK");
        }
    }

    private void AtualizarPreview()
    {
        FotoPreview = _foto?.Conteudo is { Length: > 0 } bytes
            ? ImageSource.FromStream(() => new MemoryStream(bytes, writable: false))
            : null;
    }

    private async Task SalvarAsync()
    {
        if (IsBusy) return;
        var cpfLimpo = new string(Cpf.Where(char.IsDigit).ToArray());
        var telefoneLimpo = new string(Telefone.Where(char.IsDigit).ToArray());
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(Nome)) errors.Add("Informe o nome completo.");
        if (cpfLimpo.Length != 11) errors.Add("O CPF deve conter 11 dígitos.");
        if (DataNascimento.Date > DataMaximaNascimento.Date) errors.Add("O aluno deve ter pelo menos 12 anos.");
        if (telefoneLimpo.Length is not (10 or 11)) errors.Add("Informe um telefone com DDD (10 ou 11 dígitos).");
        if (!Regex.IsMatch(Email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) errors.Add("Informe um e-mail válido.");
        if (EnderecoSelecionado is null || EnderecoSelecionado.Id <= 0) errors.Add("Cadastre e selecione um logradouro antes de salvar.");
        if (string.IsNullOrWhiteSpace(Numero)) errors.Add("Informe o número do endereço.");
        if (_foto?.Conteudo is not { Length: > 0 }) errors.Add("Selecione ou tire uma foto para o cadastro.");
        if (!IsEditMode && string.IsNullOrWhiteSpace(Senha)) errors.Add("A senha é obrigatória para um novo aluno.");
        if (!string.IsNullOrWhiteSpace(Senha))
        {
            if (Senha.Trim().Length < 6) errors.Add("A senha deve ter pelo menos 6 caracteres.");
            if (Senha != ConfirmarSenha) errors.Add("A confirmação da senha não corresponde.");
        }
        if (errors.Count > 0)
        {
            await Shell.Current.DisplayAlertAsync("Revise o formulário", string.Join("\n", errors), "OK");
            return;
        }

        var dto = new AlunoDto
        {
            Id = Id,
            Nome = Nome.Trim(),
            Cpf = cpfLimpo,
            DataNascimento = DateOnly.FromDateTime(DataNascimento),
            Telefone = telefoneLimpo,
            Email = Email.Trim(),
            Endereco = EnderecoSelecionado,
            Numero = Numero.Trim(),
            Complemento = string.IsNullOrWhiteSpace(Complemento) ? null : Complemento.Trim(),
            Senha = string.IsNullOrWhiteSpace(Senha) ? null : Senha,
            Foto = _foto
        };
        try
        {
            IsBusy = true;
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
            if (IsEditMode)
                await _alunoService.AtualizarAsync(dto, cts.Token);
            else
                await _alunoService.AdicionarAsync(dto, cts.Token);
            await Shell.Current.DisplayAlertAsync("Salvo", IsEditMode ? "Aluno atualizado." : "Aluno cadastrado.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Erro ao salvar", ex.Message, "OK");
        }
        finally { IsBusy = false; }
    }
}
//Aluno: Túlio Thauã Dutra
