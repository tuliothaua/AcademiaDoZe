// Nome: Túlio Thauã Dutra
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Domain.Enums;

namespace AcademiaDoZe.Domain.Entities;

public class Colaborador : Pessoa
{
    public DateOnly DataAdmissao { get; private set; }
    public ColaboradorTipo Tipo { get; private set; }
    public ColaboradorVinculo Vinculo { get; private set; }

    public Colaborador(int id, string nome, Cpf cpf, DateOnly dataNascimento, Telefone telefone, Email email, Senha senha, Arquivo foto, Logradouro logradouro, string numero, string complemento, DateOnly dataAdmissao, ColaboradorTipo tipo, ColaboradorVinculo vinculo)
        : base(id, nome, cpf, dataNascimento, telefone, email, senha, foto, logradouro, numero, complemento)
    {
        DataAdmissao = dataAdmissao;
        Tipo = tipo;
        Vinculo = vinculo;
    }
}