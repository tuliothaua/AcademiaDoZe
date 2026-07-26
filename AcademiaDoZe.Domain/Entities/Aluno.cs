// Nome: Túlio Thauã Dutra
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities;

public class Aluno : Pessoa
{
    public Aluno(int id, string nome, Cpf cpf, DateOnly dataNascimento, Telefone telefone, Email email, Senha senha, Arquivo foto, Logradouro logradouro, string numero, string complemento)
        : base(id, nome, cpf, dataNascimento, telefone, email, senha, foto, logradouro, numero, complemento)
    {
    }
}