// Nome: Túlio Thauã Dutra
namespace AcademiaDoZe.Domain.Entities;

public class AcessoAluno : Entity
{
    public Aluno Aluno { get; private set; }
    public DateTime HorarioEntrada { get; private set; }
    public DateTime? HorarioSaida { get; private set; }

    public AcessoAluno(int id, Aluno aluno, DateTime horarioEntrada, DateTime? horarioSaida = null) : base(id)
    {
        Aluno = aluno;
        HorarioEntrada = horarioEntrada;
        HorarioSaida = horarioSaida;
    }
}