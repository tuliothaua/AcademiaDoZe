// Nome: Túlio Thauã Dutra
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;
using AcademiaDoZe.Domain.ValueObjects;
using System.Collections.Generic;
namespace AcademiaDoZe.Domain.Entities;

public class Logradouro : Entity, IAggregateRoot
{
    public Cep Cep { get; private set; }
    public string Pais { get; private set; }
    public string Estado { get; private set; }
    public string Cidade { get; private set; }
    public string Bairro { get; private set; }
    public string Nome { get; private set; }

    private Logradouro(int id, Cep cep, string pais, string estado, string cidade, string bairro, string nome)
        : base(id)
    {
        Cep = cep;
        Pais = pais;
        Estado = estado;
        Cidade = cidade;
        Bairro = bairro;
        Nome = nome;
    }

    public static Result<Logradouro> Criar(int id, string cepStr, string pais, string estado, string cidade, string bairro, string nome)
    {
        NormalizadoService.ParaMaiusculo(estado);
        var notifications = new List<Notification>();

        var cepResult = Cep.Criar(cepStr);
        if (cepResult.IsFailure)
        {
            notifications.AddRange(cepResult.Notifications);
        }

        if (string.IsNullOrWhiteSpace(pais))
            notifications.Add(new Notification("Pais", "PAIS_OBRIGATORIO"));
        if (string.IsNullOrWhiteSpace(estado))
            notifications.Add(new Notification("Estado", "ESTADO_OBRIGATORIO"));
        if (string.IsNullOrWhiteSpace(cidade))
            notifications.Add(new Notification("Cidade", "CIDADE_OBRIGATORIA"));
        if (string.IsNullOrWhiteSpace(nome))
            notifications.Add(new Notification("Nome", "NOME_LOGRADOURO_OBRIGATORIO"));

        if (notifications.Count > 0)
            return Result<Logradouro>.Failure(notifications);

        var logradouro = new Logradouro(id, cepResult.Value!, pais.Trim(), estado.Trim(), cidade.Trim(), bairro?.Trim() ?? string.Empty, nome.Trim());

        return Result<Logradouro>.Success(logradouro);
    }
}