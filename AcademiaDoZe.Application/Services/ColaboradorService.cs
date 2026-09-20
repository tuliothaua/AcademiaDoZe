using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Application.Security;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Application.Services;

public sealed class ColaboradorService : IColaboradorService
{
    private readonly IColaboradorRepository _repository;
    public ColaboradorService(IColaboradorRepository repository) => _repository = repository;

    public async Task<ColaboradorDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default) =>
        (await _repository.ObterPorIdAsync(id, cancellationToken))?.ToDto();

    public async Task<IEnumerable<ColaboradorDto>> ObterTodosAsync(CancellationToken cancellationToken = default) =>
        (await _repository.ObterTodosAsync(cancellationToken)).Select(x => x.ToDto()).ToList();

    public async Task<ColaboradorDto> AdicionarAsync(ColaboradorDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (dto.Endereco is null) throw new ArgumentException("O endereço é obrigatório.", nameof(dto));
        if (string.IsNullOrWhiteSpace(dto.Senha)) throw new ArgumentException("A senha é obrigatória.", nameof(dto));
        dto.Senha = PasswordHasher.Hash(dto.Senha);
        var entity = dto.ToEntity(dto.Endereco.ToEntity());
        await _repository.AdicionarAsync(entity, cancellationToken);
        return entity.ToDto();
    }

    public async Task<ColaboradorDto> AtualizarAsync(ColaboradorDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (dto.Endereco is null) throw new ArgumentException("O endereço é obrigatório.", nameof(dto));
        if (!string.IsNullOrWhiteSpace(dto.Senha)) dto.Senha = PasswordHasher.Hash(dto.Senha);
        var entity = dto.ToEntity(dto.Endereco.ToEntity());
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

    public async Task<ColaboradorDto?> ObterPorCpfAsync(string cpf, CancellationToken cancellationToken = default) =>
        (await _repository.ObterPorCpfAsync(cpf, cancellationToken))?.ToDto();
}
