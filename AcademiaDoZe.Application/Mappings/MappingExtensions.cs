using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Application.Mappings;

public static class MappingExtensions
{
    public static LogradouroDto ToDto(this Logradouro entity) => new()
    {
        Id = entity.Id, Cep = entity.Cep.Valor, Pais = entity.Pais, Estado = entity.Estado,
        Cidade = entity.Cidade, Bairro = entity.Bairro, Nome = entity.Nome
    };

    public static Logradouro ToEntity(this LogradouroDto dto)
    {
        var result = Logradouro.Criar(dto.Id, dto.Cep, dto.Pais, dto.Estado, dto.Cidade, dto.Bairro, dto.Nome);
        return result.IsSuccess ? result.Value! : throw new ArgumentException(string.Join(", ", result.Notifications.Select(x => x.Mensagem)));
    }

    public static Arquivo? ToEntity(this ArquivoDto? dto)
    {
        if (dto?.Conteudo is not { Length: > 0 }) return null;
        var result = Arquivo.Criar(dto.Conteudo);
        return result.IsSuccess ? result.Value : null;
    }

    private static Arquivo ArquivoObrigatorio(ArquivoDto? dto) => dto.ToEntity() ?? Arquivo.Criar(new byte[] { 0 }).Value!;

    public static AlunoDto ToDto(this Aluno entity) => new()
    {
        Id = entity.Id, Nome = entity.Nome, Cpf = entity.Cpf.Valor, DataNascimento = entity.DataNascimento,
        Telefone = entity.Telefone.Numero, Email = entity.Email.Endereco, Senha = entity.Senha.Valor,
        Foto = new ArquivoDto { Conteudo = entity.Foto.Conteudo }, Endereco = entity.Logradouro.ToDto(),
        Numero = entity.Numero, Complemento = entity.Complemento
    };

    public static Aluno ToEntity(this AlunoDto dto, Logradouro logradouro)
    {
        var result = Aluno.Criar(dto.Id, dto.Nome, dto.Cpf, dto.DataNascimento, dto.Telefone, dto.Email ?? string.Empty,
            dto.Senha ?? "senha-temporaria", ArquivoObrigatorio(dto.Foto), logradouro, dto.Numero, dto.Complemento ?? string.Empty);
        return result.IsSuccess ? result.Value! : throw new ArgumentException(string.Join(", ", result.Notifications.Select(x => x.Mensagem)));
    }

    public static ColaboradorDto ToDto(this Colaborador entity) => new()
    {
        Id = entity.Id, Nome = entity.Nome, Cpf = entity.Cpf.Valor, DataNascimento = entity.DataNascimento,
        Telefone = entity.Telefone.Numero, Email = entity.Email.Endereco, Senha = entity.Senha.Valor,
        Foto = entity.Foto is null ? null : new ArquivoDto { Conteudo = entity.Foto.Conteudo }, Endereco = entity.Logradouro.ToDto(),
        Numero = entity.Numero, Complemento = entity.Complemento, DataAdmissao = entity.DataAdmissao,
        Tipo = (AppColaboradorTipo)entity.Tipo, Vinculo = (AppColaboradorVinculo)entity.Vinculo
    };

    public static Colaborador ToEntity(this ColaboradorDto dto, Logradouro logradouro)
    {
        var result = Colaborador.Criar(dto.Id, dto.Nome, dto.Cpf, dto.DataNascimento, dto.Telefone, dto.Email ?? string.Empty,
            dto.Senha ?? "senha-temporaria", ArquivoObrigatorio(dto.Foto), logradouro, dto.Numero, dto.Complemento ?? string.Empty,
            dto.DataAdmissao, (Domain.Enums.ColaboradorTipo)dto.Tipo, (Domain.Enums.ColaboradorVinculo)dto.Vinculo);
        return result.IsSuccess ? result.Value! : throw new ArgumentException(string.Join(", ", result.Notifications.Select(x => x.Mensagem)));
    }

    public static MatriculaDto ToDto(this Matricula entity) => new()
    {
        Id = entity.Id, Plano = (AppMatriculaPlano)entity.Plano, DataInicio = entity.DataInicio, DataFim = entity.DataFim,
        Objetivo = entity.Objetivo, RestricoesMedicas = (AppMatriculaRestricoes)entity.RestricoesMedicas,
        ObservacoesRestricoes = entity.ObservacoesRestricoes,
        LaudoMedico = entity.LaudoMedico is null ? null : new ArquivoDto { Conteudo = entity.LaudoMedico.Conteudo },
        AlunoMatricula = entity.AlunoMatricula.ToDto()
    };

    public static Matricula ToEntity(this MatriculaDto dto, Aluno aluno)
    {
        var laudo = dto.LaudoMedico.ToEntity();
        var result = Matricula.Criar(dto.Id, aluno, (Domain.Enums.MatriculaPlano)dto.Plano, dto.DataInicio, dto.DataFim,
            dto.Objetivo, (Domain.Enums.MatriculaRestricoes)dto.RestricoesMedicas, dto.ObservacoesRestricoes ?? string.Empty, laudo);
        return result.IsSuccess ? result.Value! : throw new ArgumentException(string.Join(", ", result.Notifications.Select(x => x.Mensagem)));
    }
}
