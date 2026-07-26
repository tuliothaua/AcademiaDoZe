// Nome: Túlio Thauã Dutra
namespace AcademiaDoZe.Domain.Entities;

public class AcessoColaborador : Entity
{
    public Colaborador Colaborador { get; private set; }
    public DateTime HorarioEntrada { get; private set; }
    public DateTime? HorarioSaida { get; private set; }

    public AcessoColaborador(int id, Colaborador colaborador, DateTime horarioEntrada, DateTime? horarioSaida = null) : base(id)
    {
        Colaborador = colaborador;
        HorarioEntrada = horarioEntrada;
        HorarioSaida = horarioSaida;
    }
}