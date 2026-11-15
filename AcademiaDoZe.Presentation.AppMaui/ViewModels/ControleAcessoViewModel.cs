using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public sealed class ControleAcessoViewModel : BaseViewModel
{
    private readonly IAlunoService _alunoService;
    private readonly IColaboradorService _colaboradorService;
    private readonly IControleAcessoService _controleAcessoService;
    private AlunoDto? _alunoSelecionado;
    private ColaboradorDto? _colaboradorSelecionado;
    private string _mensagem = "Selecione uma pessoa e registre a entrada ou saída.";

    public ObservableCollection<AlunoDto> Alunos { get; } = new();
    public ObservableCollection<ColaboradorDto> Colaboradores { get; } = new();
    public AlunoDto? AlunoSelecionado { get => _alunoSelecionado; set => SetProperty(ref _alunoSelecionado, value); }
    public ColaboradorDto? ColaboradorSelecionado { get => _colaboradorSelecionado; set => SetProperty(ref _colaboradorSelecionado, value); }
    public string Mensagem { get => _mensagem; private set => SetProperty(ref _mensagem, value); }

    public Command AtualizarCommand { get; }
    public Command EntradaAlunoCommand { get; }
    public Command SaidaAlunoCommand { get; }
    public Command EntradaColaboradorCommand { get; }
    public Command SaidaColaboradorCommand { get; }

    public ControleAcessoViewModel(IAlunoService alunoService, IColaboradorService colaboradorService, IControleAcessoService controleAcessoService)
    {
        _alunoService = alunoService;
        _colaboradorService = colaboradorService;
        _controleAcessoService = controleAcessoService;
        Title = "Entrada e saída";
        AtualizarCommand = new Command(async () => await CarregarAsync());
        EntradaAlunoCommand = new Command(async () => await ExecutarAsync(() => _controleAcessoService.RegistrarEntradaAlunoAsync(AlunoSelecionado!.Id), AlunoSelecionado is not null));
        SaidaAlunoCommand = new Command(async () => await ExecutarAsync(() => _controleAcessoService.RegistrarSaidaAlunoAsync(AlunoSelecionado!.Id), AlunoSelecionado is not null));
        EntradaColaboradorCommand = new Command(async () => await ExecutarAsync(() => _controleAcessoService.RegistrarEntradaColaboradorAsync(ColaboradorSelecionado!.Id), ColaboradorSelecionado is not null));
        SaidaColaboradorCommand = new Command(async () => await ExecutarAsync(() => _controleAcessoService.RegistrarSaidaColaboradorAsync(ColaboradorSelecionado!.Id), ColaboradorSelecionado is not null));
    }

    public async Task CarregarAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            var alunosTask = _alunoService.ObterTodosAsync();
            var colaboradoresTask = _colaboradorService.ObterTodosAsync();
            await Task.WhenAll(alunosTask, colaboradoresTask);
            Alunos.Clear();
            foreach (var aluno in await alunosTask) Alunos.Add(aluno);
            Colaboradores.Clear();
            foreach (var colaborador in await colaboradoresTask) Colaboradores.Add(colaborador);
            AlunoSelecionado ??= Alunos.FirstOrDefault();
            ColaboradorSelecionado ??= Colaboradores.FirstOrDefault();
            if (Alunos.Count == 0 && Colaboradores.Count == 0)
                Mensagem = "Cadastre alunos e colaboradores antes de usar o controle de acesso.";
        }
        catch (Exception ex)
        {
            Mensagem = $"Falha ao carregar pessoas: {ex.Message}";
        }
        finally { IsBusy = false; }
    }

    private async Task ExecutarAsync(Func<Task<string>> acao, bool pessoaSelecionada)
    {
        if (!pessoaSelecionada)
        {
            Mensagem = "Selecione a pessoa antes de registrar o acesso.";
            return;
        }
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            Mensagem = await acao();
        }
        catch (Exception ex)
        {
            Mensagem = ex.Message;
        }
        finally { IsBusy = false; }
    }
}
