using AcademiaDoZe.Application.DTOs;

namespace AcademiaDoZe.Application.Interfaces;

public interface IMatriculaService
{
    Task<MatriculaDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MatriculaDto>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<MatriculaDto> AdicionarAsync(MatriculaDto dto, CancellationToken cancellationToken = default);
    Task<MatriculaDto> AtualizarAsync(MatriculaDto dto, CancellationToken cancellationToken = default);
    Task<bool> RemoverAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MatriculaDto>> ObterPorAlunoAsync(int alunoId, CancellationToken cancellationToken = default);
}
