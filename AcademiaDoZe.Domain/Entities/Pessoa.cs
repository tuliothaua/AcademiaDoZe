// Nome: Túlio Thauã Dutra
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities;

public abstract class Pessoa : Entity
{
    public string NomeCompleto { get; protected set; }
    public Cpf Cpf { get; protected set; }
    public DateOnly DataNascimento { get; protected set; }
    public Telefone Telefone { get; protected set; }
    public Email Email { get; protected set; }
    public Senha Senha { get; protected set; }
    public Arquivo Foto { get; protected set; }
    public Logradouro Logradouro { get; protected set; }
    public string Numero { get; protected set; }
    public string Complemento { get; protected set; }

    // Construtor que recebe todos os dados obrigatórios
    protected Pessoa(int id, string nomeCompleto, Cpf cpf, DateOnly dataNascimento, Telefone telefone, Email email, Senha senha, Arquivo foto, Logradouro logradouro, string numero, string complemento)
        : base(id)
    {
        NomeCompleto = nomeCompleto;
        Cpf = cpf;
        DataNascimento = dataNascimento;
        Telefone = telefone;
        Email = email;
        Senha = senha;
        Foto = foto;
        Logradouro = logradouro;
        Numero = numero;
        Complemento = complemento;
    }
}