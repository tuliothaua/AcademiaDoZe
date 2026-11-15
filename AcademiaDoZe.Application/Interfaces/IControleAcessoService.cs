namespace AcademiaDoZe.Application.Interfaces;

public interface IControleAcessoService
{
    Task<string> RegistrarEntradaAlunoAsync(int alunoId, CancellationToken cancellationToken = default);
    Task<string> RegistrarSaidaAlunoAsync(int alunoId, CancellationToken cancellationToken = default);
    Task<string> RegistrarEntradaColaboradorAsync(int colaboradorId, CancellationToken cancellationToken = default);
    Task<string> RegistrarSaidaColaboradorAsync(int colaboradorId, CancellationToken cancellationToken = default);
}
