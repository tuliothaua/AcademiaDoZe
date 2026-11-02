using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Interfaces;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System.Collections.ObjectModel;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public sealed class MatriculaFormViewModel : BaseViewModel
{
    private readonly IMatriculaService _matriculaService;
    private readonly IAlunoService _alunoService;
    private int _id;
    private AlunoDto? _alunoSelecionado;
    private AppMatriculaPlano _plano;
    private DateTime _dataInicio = DateTime.Today;
    private string _objetivo = string.Empty;
    private string _observacoes = string.Empty;
    private ArquivoDto? _laudo;
    private string _laudoNome = "Nenhum laudo selecionado";
    private bool _diabetes, _pressaoAlta, _labirintite, _alergias, _problemasRespiratorios;

    public ObservableCollection<AlunoDto> Alunos { get; } = [];
    public IReadOnlyList<AppMatriculaPlano> Planos { get; } = Enum.GetValues<AppMatriculaPlano>();
    public int Id { get => _id; private set => SetProperty(ref _id, value); }
    public bool IsEditMode => Id > 0;
    public AlunoDto? AlunoSelecionado { get => _alunoSelecionado; set => SetProperty(ref _alunoSelecionado, value); }
    public AppMatriculaPlano Plano { get => _plano; set { if (SetProperty(ref _plano, value)) OnPropertyChanged(nameof(DataFim)); } }
    public DateTime DataInicio { get => _dataInicio; set { if (SetProperty(ref _dataInicio, value)) OnPropertyChanged(nameof(DataFim)); } }
    public DateTime DataFim => DataInicio.Date.AddMonths(Meses(Plano)).AddDays(-1);
    public string Objetivo { get => _objetivo; set => SetProperty(ref _objetivo, value); }
    public string Observacoes { get => _observacoes; set => SetProperty(ref _observacoes, value); }
    public string LaudoNome { get => _laudoNome; private set => SetProperty(ref _laudoNome, value); }
    public bool Diabetes { get => _diabetes; set { SetProperty(ref _diabetes, value); OnPropertyChanged(nameof(RestricoesSelecionadas)); } }
    public bool PressaoAlta { get => _pressaoAlta; set { SetProperty(ref _pressaoAlta, value); OnPropertyChanged(nameof(RestricoesSelecionadas)); } }
    public bool Labirintite { get => _labirintite; set { SetProperty(ref _labirintite, value); OnPropertyChanged(nameof(RestricoesSelecionadas)); } }
    public bool Alergias { get => _alergias; set { SetProperty(ref _alergias, value); OnPropertyChanged(nameof(RestricoesSelecionadas)); } }
    public bool ProblemasRespiratorios { get => _problemasRespiratorios; set { SetProperty(ref _problemasRespiratorios, value); OnPropertyChanged(nameof(RestricoesSelecionadas)); } }
    public bool RestricoesSelecionadas => Diabetes || PressaoAlta || Labirintite || Alergias || ProblemasRespiratorios;
    public AsyncCommand SaveCommand { get; }
    public AsyncCommand CancelCommand { get; }
    public AsyncCommand SelectLaudoCommand { get; }

    public MatriculaFormViewModel(IMatriculaService matriculaService, IAlunoService alunoService)
    {
        _matriculaService = matriculaService; _alunoService = alunoService;
        SaveCommand = new AsyncCommand(_ => SaveAsync());
        CancelCommand = new AsyncCommand(_ => Shell.Current.GoToAsync(".."));
        SelectLaudoCommand = new AsyncCommand(_ => SelectLaudoAsync());
    }

    public async Task InitializeAsync(int id)
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true; Id = id;
            Title = IsEditMode ? "Editar matrícula" : "Nova matrícula";
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
            Alunos.Clear();
            foreach (var aluno in await _alunoService.ObterTodosAsync(cts.Token)) Alunos.Add(aluno);
            if (!IsEditMode) { Reset(); return; }
            var dto = await _matriculaService.ObterPorIdAsync(id, cts.Token);
            if (dto is null) { await Shell.Current.DisplayAlertAsync("Matrícula não encontrada", "O registro não existe mais.", "OK"); await Shell.Current.GoToAsync(".."); return; }
            AlunoSelecionado = Alunos.FirstOrDefault(x => x.Id == dto.AlunoMatricula?.Id) ?? dto.AlunoMatricula;
            if (AlunoSelecionado is not null && Alunos.All(x => x.Id != AlunoSelecionado.Id)) Alunos.Add(AlunoSelecionado);
            Plano = dto.Plano; DataInicio = dto.DataInicio.ToDateTime(TimeOnly.MinValue);
            Objetivo = dto.Objetivo; Observacoes = dto.ObservacoesRestricoes ?? string.Empty; _laudo = dto.LaudoMedico;
            LaudoNome = _laudo is null ? "Nenhum laudo selecionado" : "Laudo médico anexado";
            Diabetes = dto.RestricoesMedicas.HasFlag(AppMatriculaRestricoes.Diabetes);
            PressaoAlta = dto.RestricoesMedicas.HasFlag(AppMatriculaRestricoes.PressaoAlta);
            Labirintite = dto.RestricoesMedicas.HasFlag(AppMatriculaRestricoes.Labirintite);
            Alergias = dto.RestricoesMedicas.HasFlag(AppMatriculaRestricoes.Alergias);
            ProblemasRespiratorios = dto.RestricoesMedicas.HasFlag(AppMatriculaRestricoes.ProblemasRespiratorios);
        }
        catch (Exception ex) { await Shell.Current.DisplayAlertAsync("Erro", $"Não foi possível abrir a matrícula. {ex.Message}", "OK"); }
        finally { IsBusy = false; }
    }

    private void Reset()
    {
        AlunoSelecionado = null; Plano = AppMatriculaPlano.Mensal; DataInicio = DateTime.Today;
        Objetivo = Observacoes = string.Empty; _laudo = null; LaudoNome = "Nenhum laudo selecionado";
        Diabetes = PressaoAlta = Labirintite = Alergias = ProblemasRespiratorios = false;
    }

    private async Task SelectLaudoAsync()
    {
        try
        {
            var file = await FilePicker.Default.PickAsync(new PickOptions { PickerTitle = "Selecione o laudo médico" });
            if (file is null) return;
            if (file.FileName.Length == 0) return;
            await using var stream = await file.OpenReadAsync();
            using var memory = new MemoryStream(); await stream.CopyToAsync(memory);
            if (memory.Length == 0 || memory.Length > 10 * 1024 * 1024) { await Shell.Current.DisplayAlertAsync("Laudo inválido", "O arquivo deve ter conteúdo e no máximo 10 MB.", "OK"); return; }
            _laudo = new ArquivoDto { Conteudo = memory.ToArray() }; LaudoNome = file.FileName;
        }
        catch (Exception ex) { await Shell.Current.DisplayAlertAsync("Erro", $"Não foi possível anexar o laudo. {ex.Message}", "OK"); }
    }

    private async Task SaveAsync()
    {
        if (IsBusy) return;
        var errors = new List<string>();
        if (AlunoSelecionado is null) errors.Add("Selecione um aluno.");
        if (string.IsNullOrWhiteSpace(Objetivo)) errors.Add("Informe o objetivo da matrícula.");
        if (AlunoSelecionado is not null)
        {
            var nascimento = AlunoSelecionado.DataNascimento;
            var idade = DataInicio.Year - nascimento.Year - (DataInicio.Date < nascimento.ToDateTime(TimeOnly.MinValue).AddYears(DataInicio.Year - nascimento.Year) ? 1 : 0);
            if (idade is >= 12 and <= 16 && _laudo is null) errors.Add("Alunos de 12 a 16 anos precisam de laudo médico.");
        }
        if (RestricoesSelecionadas && _laudo is null) errors.Add("Para restrições de saúde, anexe o parecer médico autorizando atividade física.");
        if (errors.Count > 0) { await Shell.Current.DisplayAlertAsync("Revise os dados", string.Join("\n", errors), "OK"); return; }
        var flags = AppMatriculaRestricoes.None;
        if (Diabetes) flags |= AppMatriculaRestricoes.Diabetes;
        if (PressaoAlta) flags |= AppMatriculaRestricoes.PressaoAlta;
        if (Labirintite) flags |= AppMatriculaRestricoes.Labirintite;
        if (Alergias) flags |= AppMatriculaRestricoes.Alergias;
        if (ProblemasRespiratorios) flags |= AppMatriculaRestricoes.ProblemasRespiratorios;
        var dto = new MatriculaDto { Id = Id, AlunoMatricula = AlunoSelecionado, Plano = Plano, DataInicio = DateOnly.FromDateTime(DataInicio), DataFim = DateOnly.FromDateTime(DataFim), Objetivo = Objetivo.Trim(), RestricoesMedicas = flags, ObservacoesRestricoes = Observacoes.Trim(), LaudoMedico = _laudo };
        try
        {
            IsBusy = true; using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
            if (IsEditMode) await _matriculaService.AtualizarAsync(dto, cts.Token); else await _matriculaService.AdicionarAsync(dto, cts.Token);
            await Shell.Current.DisplayAlertAsync("Salvo", IsEditMode ? "Matrícula atualizada." : "Matrícula cadastrada.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex) { await Shell.Current.DisplayAlertAsync("Não foi possível salvar", ex.Message, "OK"); }
        finally { IsBusy = false; }
    }

    private static int Meses(AppMatriculaPlano plano) => plano switch { AppMatriculaPlano.Mensal => 1, AppMatriculaPlano.Trimestral => 3, AppMatriculaPlano.Semestral => 6, AppMatriculaPlano.Anual => 12, _ => 1 };
}
