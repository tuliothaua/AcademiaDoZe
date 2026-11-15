using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Application.Services;

public sealed class ControleAcessoService : IControleAcessoService
{
    private readonly IAlunoRepository _alunos;
    private readonly IColaboradorRepository _colaboradores;
    private readonly IMatriculaRepository _matriculas;
    private readonly IAcessoAlunoRepository _acessosAluno;
    private readonly IAcessoColaboradorRepository _acessosColaborador;

    public ControleAcessoService(
        IAlunoRepository alunos,
        IColaboradorRepository colaboradores,
        IMatriculaRepository matriculas,
        IAcessoAlunoRepository acessosAluno,
        IAcessoColaboradorRepository acessosColaborador)
    {
        _alunos = alunos;
        _colaboradores = colaboradores;
        _matriculas = matriculas;
        _acessosAluno = acessosAluno;
        _acessosColaborador = acessosColaborador;
    }

    public async Task<string> RegistrarEntradaAlunoAsync(int alunoId, CancellationToken cancellationToken = default)
    {
        var aluno = await _alunos.ObterPorIdAsync(alunoId, cancellationToken)
            ?? throw new KeyNotFoundException("Aluno não encontrado.");
        var eventos = (await _acessosAluno.ObterPorAlunoAsync(alunoId, DateOnly.FromDateTime(DateTime.Today), cancellationToken))
            .OrderBy(x => x.DataHora).ThenBy(x => x.Id).ToList();
        if (eventos.Count % 2 != 0)
            throw new InvalidOperationException($"{aluno.NomeCompleto} já possui uma entrada sem saída registrada hoje.");

        var hoje = DateOnly.FromDateTime(DateTime.Today);
        var matricula = (await _matriculas.ObterPorAlunoAsync(alunoId, cancellationToken))
            .Where(m => m.DataInicio <= hoje && (m.DataFim is null || m.DataFim.Value >= hoje))
            .OrderByDescending(m => m.DataFim).FirstOrDefault();
        if (matricula is null)
            throw new InvalidOperationException($"Entrada negada: {aluno.NomeCompleto} não possui matrícula ativa.");

        var agora = DateTime.Now;
        var acesso = Domain.Entities.AcessoAluno.Criar(0, aluno, agora);
        if (acesso.IsFailure) throw new InvalidOperationException(string.Join("; ", acesso.Notifications.Select(n => n.Mensagem)));
        await _acessosAluno.AdicionarAsync(acesso.Value!, cancellationToken);
        var dias = matricula.DataFim is null ? (int?)null : Math.Max(0, matricula.DataFim.Value.DayNumber - hoje.DayNumber);
        var prazo = dias is null ? "sem data de vencimento" : $"restam {dias} dia(s) de plano (vence em {matricula.DataFim:dd/MM/yyyy})";
        return $"Entrada registrada para {aluno.NomeCompleto} às {agora:HH:mm}. Matrícula ativa; {prazo}.";
    }

    public async Task<string> RegistrarSaidaAlunoAsync(int alunoId, CancellationToken cancellationToken = default)
    {
        var aluno = await _alunos.ObterPorIdAsync(alunoId, cancellationToken)
            ?? throw new KeyNotFoundException("Aluno não encontrado.");
        var eventos = (await _acessosAluno.ObterPorAlunoAsync(alunoId, DateOnly.FromDateTime(DateTime.Today), cancellationToken))
            .OrderBy(x => x.DataHora).ThenBy(x => x.Id).ToList();
        if (eventos.Count == 0 || eventos.Count % 2 == 0)
            throw new InvalidOperationException($"Não há entrada em aberto hoje para {aluno.NomeCompleto}.");
        var agora = DateTime.Now;
        var acesso = Domain.Entities.AcessoAluno.Criar(0, aluno, agora);
        if (acesso.IsFailure) throw new InvalidOperationException(string.Join("; ", acesso.Notifications.Select(n => n.Mensagem)));
        await _acessosAluno.AdicionarAsync(acesso.Value!, cancellationToken);
        var tempo = agora - eventos[^1].DataHora;
        return $"Saída registrada para {aluno.NomeCompleto} às {agora:HH:mm}. Permanência: {FormatarDuracao(tempo)}.";
    }

    public async Task<string> RegistrarEntradaColaboradorAsync(int colaboradorId, CancellationToken cancellationToken = default)
    {
        var colaborador = await _colaboradores.ObterPorIdAsync(colaboradorId, cancellationToken)
            ?? throw new KeyNotFoundException("Colaborador não encontrado.");
        var eventos = (await _acessosColaborador.ObterPorColaboradorAsync(colaboradorId, DateOnly.FromDateTime(DateTime.Today), cancellationToken))
            .OrderBy(x => x.DataHora).ThenBy(x => x.Id).ToList();
        if (eventos.Count % 2 != 0)
            throw new InvalidOperationException($"{colaborador.NomeCompleto} já possui uma entrada sem saída registrada hoje.");

        var limite = colaborador.Vinculo == ColaboradorVinculo.CLT ? TimeSpan.FromHours(8) : TimeSpan.FromHours(6);
        var total = SomarPares(eventos.Select(x => x.DataHora).ToList(), DateTime.Now);
        if (total >= limite)
            throw new InvalidOperationException($"Entrada negada: {colaborador.NomeCompleto} já atingiu o limite diário de {FormatarDuracao(limite)} ({colaborador.Vinculo}).");

        var agora = DateTime.Now;
        var acesso = Domain.Entities.AcessoColaborador.Criar(0, colaborador, agora);
        if (acesso.IsFailure) throw new InvalidOperationException(string.Join("; ", acesso.Notifications.Select(n => n.Mensagem)));
        await _acessosColaborador.AdicionarAsync(acesso.Value!, cancellationToken);
        return $"Entrada registrada para {colaborador.NomeCompleto} às {agora:HH:mm}. Limite diário: {FormatarDuracao(limite)} ({colaborador.Vinculo}); total computado hoje: {FormatarDuracao(total)}.";
    }

    public async Task<string> RegistrarSaidaColaboradorAsync(int colaboradorId, CancellationToken cancellationToken = default)
    {
        var colaborador = await _colaboradores.ObterPorIdAsync(colaboradorId, cancellationToken)
            ?? throw new KeyNotFoundException("Colaborador não encontrado.");
        var eventos = (await _acessosColaborador.ObterPorColaboradorAsync(colaboradorId, DateOnly.FromDateTime(DateTime.Today), cancellationToken))
            .OrderBy(x => x.DataHora).ThenBy(x => x.Id).ToList();
        if (eventos.Count == 0 || eventos.Count % 2 == 0)
            throw new InvalidOperationException($"Não há entrada em aberto hoje para {colaborador.NomeCompleto}.");
        var agora = DateTime.Now;
        var acesso = Domain.Entities.AcessoColaborador.Criar(0, colaborador, agora);
        if (acesso.IsFailure) throw new InvalidOperationException(string.Join("; ", acesso.Notifications.Select(n => n.Mensagem)));
        await _acessosColaborador.AdicionarAsync(acesso.Value!, cancellationToken);
        var total = SomarPares(eventos.Select(x => x.DataHora).Append(agora).ToList(), agora);
        var limite = colaborador.Vinculo == ColaboradorVinculo.CLT ? TimeSpan.FromHours(8) : TimeSpan.FromHours(6);
        var aviso = total > limite ? $" Atenção: o total excedeu o limite de {FormatarDuracao(limite)}." : string.Empty;
        return $"Saída registrada para {colaborador.NomeCompleto} às {agora:HH:mm}. Total de permanência hoje (somando os períodos): {FormatarDuracao(total)}.{aviso}";
    }

    private static TimeSpan SomarPares(IReadOnlyList<DateTime> eventos, DateTime agora)
    {
        var total = TimeSpan.Zero;
        for (var i = 0; i < eventos.Count; i += 2)
        {
            var inicio = eventos[i];
            var fim = i + 1 < eventos.Count ? eventos[i + 1] : agora;
            if (fim > inicio) total += fim - inicio;
        }
        return total;
    }

    private static string FormatarDuracao(TimeSpan valor) =>
        $"{(int)valor.TotalHours}h {valor.Minutes:D2}min";
}
