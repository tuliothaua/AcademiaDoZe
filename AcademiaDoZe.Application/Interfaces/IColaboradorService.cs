using AcademiaDoZe.Application.DTOs;

namespace AcademiaDoZe.Application.Interfaces;

public interface IColaboradorService
{
    Task<ColaboradorDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ColaboradorDto>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<ColaboradorDto> AdicionarAsync(ColaboradorDto dto, CancellationToken cancellationToken = default);
    Task<ColaboradorDto> AtualizarAsync(ColaboradorDto dto, CancellationToken cancellationToken = default);
    Task<bool> RemoverAsync(int id, CancellationToken cancellationToken = default);
    Task<ColaboradorDto?> ObterPorCpfAsync(string cpf, CancellationToken cancellationToken = default);
}
