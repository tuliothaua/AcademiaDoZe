using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Application.Security;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Application.Services;

public sealed class AlunoService : IAlunoService
{
    private readonly IAlunoRepository _repository;
    public AlunoService(IAlunoRepository repository) => _repository = repository;

    public async Task<AlunoDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default) =>
        (await _repository.ObterPorIdAsync(id, cancellationToken))?.ToDto();

    public async Task<IEnumerable<AlunoDto>> ObterTodosAsync(CancellationToken cancellationToken = default) =>
        (await _repository.ObterTodosAsync(cancellationToken)).Select(x => x.ToDto()).ToList();

    public async Task<AlunoDto> AdicionarAsync(AlunoDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (dto.Endereco is null) throw new ArgumentException("O endereço é obrigatório.", nameof(dto));
        if (string.IsNullOrWhiteSpace(dto.Senha)) throw new ArgumentException("A senha é obrigatória.", nameof(dto));
        dto.Senha = PasswordHasher.Hash(dto.Senha);
        var logradouro = dto.Endereco.ToEntity();
        var entity = dto.ToEntity(logradouro);
        await _repository.AdicionarAsync(entity, cancellationToken);
        return entity.ToDto();
    }

    public async Task<AlunoDto> AtualizarAsync(AlunoDto dto, CancellationToken cancellationToken = default)
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

    public async Task<AlunoDto?> ObterPorCpfAsync(string cpf, CancellationToken cancellationToken = default) =>
        (await _repository.ObterPorCpfAsync(cpf, cancellationToken))?.ToDto();
}
