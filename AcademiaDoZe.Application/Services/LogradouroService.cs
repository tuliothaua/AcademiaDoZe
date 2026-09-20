using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Application.Services;

public sealed class LogradouroService : ILogradouroService
{
    private readonly ILogradouroRepository _repository;
    public LogradouroService(ILogradouroRepository repository) => _repository = repository;

    public async Task<LogradouroDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default) =>
        (await _repository.ObterPorIdAsync(id, cancellationToken))?.ToDto();

    public async Task<IEnumerable<LogradouroDto>> ObterTodosAsync(CancellationToken cancellationToken = default) =>
        (await _repository.ObterTodosAsync(cancellationToken)).Select(x => x.ToDto()).ToList();

    public async Task<LogradouroDto> AdicionarAsync(LogradouroDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var entity = dto.ToEntity();
        await _repository.AdicionarAsync(entity, cancellationToken);
        return entity.ToDto();
    }

    public async Task<LogradouroDto> AtualizarAsync(LogradouroDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var entity = dto.ToEntity();
        await _repository.AtualizarAsync(entity, cancellationToken);
        return entity.ToDto();
    }

    public Task<bool> RemoverAsync(int id, CancellationToken cancellationToken = default) => RemoverCoreAsync(id, cancellationToken);
    private async Task<bool> RemoverCoreAsync(int id, CancellationToken ct)
    {
        var entity = await _repository.ObterPorIdAsync(id, ct);
        if (entity is null) return false;
        await _repository.RemoverAsync(entity, ct);
        return true;
    }

    public async Task<LogradouroDto?> ObterPorCepAsync(string cep, CancellationToken cancellationToken = default) =>
        (await _repository.ObterPorCepAsync(cep, cancellationToken))?.ToDto();
}
