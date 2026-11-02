using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Application.Services;

public sealed class MatriculaService : IMatriculaService
{
    private readonly IMatriculaRepository _repository;
    private readonly IAlunoRepository _alunoRepository;
    public MatriculaService(IMatriculaRepository repository, IAlunoRepository alunoRepository)
    {
        _repository = repository;
        _alunoRepository = alunoRepository;
    }

    public async Task<MatriculaDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default) =>
        (await _repository.ObterPorIdAsync(id, cancellationToken))?.ToDto();

    public async Task<IEnumerable<MatriculaDto>> ObterTodosAsync(CancellationToken cancellationToken = default) =>
        (await _repository.ObterTodosAsync(cancellationToken)).Select(x => x.ToDto()).ToList();

    public async Task<MatriculaDto> AdicionarAsync(MatriculaDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var alunoId = dto.AlunoMatricula?.Id ?? 0;
        var aluno = await _alunoRepository.ObterPorIdAsync(alunoId, cancellationToken)
            ?? throw new KeyNotFoundException($"Aluno {alunoId} não encontrado.");
        var hoje = DateOnly.FromDateTime(DateTime.Today);
        var possuiMatriculaAtiva = (await _repository.ObterPorAlunoAsync(alunoId, cancellationToken))
            .Any(m => m.DataFim is null || m.DataFim.Value >= hoje);
        if (possuiMatriculaAtiva)
            throw new InvalidOperationException("Este aluno já possui uma matrícula ativa. Aguarde o vencimento antes de cadastrar outra.");
        var entity = dto.ToEntity(aluno);
        await _repository.AdicionarAsync(entity, cancellationToken);
        return entity.ToDto();
    }

    public async Task<MatriculaDto> AtualizarAsync(MatriculaDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var alunoId = dto.AlunoMatricula?.Id ?? 0;
        var aluno = await _alunoRepository.ObterPorIdAsync(alunoId, cancellationToken)
            ?? throw new KeyNotFoundException($"Aluno {alunoId} não encontrado.");
        var hoje = DateOnly.FromDateTime(DateTime.Today);
        var conflito = (await _repository.ObterPorAlunoAsync(alunoId, cancellationToken))
            .Any(m => m.Id != dto.Id && (m.DataFim is null || m.DataFim.Value >= hoje));
        if (conflito)
            throw new InvalidOperationException("Este aluno já possui outra matrícula ativa.");
        var entity = dto.ToEntity(aluno);
        await _repository.AtualizarAsync(entity, cancellationToken);
        return entity.ToDto();
    }

    public async Task<bool> RemoverAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.ObterPorIdAsync(id, cancellationToken);
        if (entity is null) return false;
        await _repository.RemoverAsync(entity, cancellationToken);
        return true;
    }

    public async Task<IEnumerable<MatriculaDto>> ObterPorAlunoAsync(int alunoId, CancellationToken cancellationToken = default) =>
        (await _repository.ObterPorAlunoAsync(alunoId, cancellationToken)).Select(x => x.ToDto()).ToList();
}

//Aluno: Túlio Thauã Dutra
