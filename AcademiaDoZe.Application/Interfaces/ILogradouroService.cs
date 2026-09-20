using AcademiaDoZe.Application.DTOs;

namespace AcademiaDoZe.Application.Interfaces;

public interface ILogradouroService
{
    Task<LogradouroDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<LogradouroDto>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<LogradouroDto> AdicionarAsync(LogradouroDto dto, CancellationToken cancellationToken = default);
    Task<LogradouroDto> AtualizarAsync(LogradouroDto dto, CancellationToken cancellationToken = default);
    Task<bool> RemoverAsync(int id, CancellationToken cancellationToken = default);
    Task<LogradouroDto?> ObterPorCepAsync(string cep, CancellationToken cancellationToken = default);
}
