// Nome: Túlio Thauã Dutra
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Exceptions;
using AcademiaDoZe.Infrastructure.Repositories;
using Xunit;

namespace AcademiaDoZe.Infrastructure.Tests;

public class LogradouroInfrastructureTests : TestBase
{
    private readonly LogradouroRepository _repository;

    public LogradouroInfrastructureTests()
    {
        _repository = new LogradouroRepository(ConnectionString, DatabaseType);
    }

    // Método auxiliar adaptado para preencher os dados exatamente com seus dados cadastrais obrigatórios
    internal static async Task<Logradouro> CriarEInserirLogradouroAsync(LogradouroRepository logradouroRepo, DatabaseType dbType)
    {
        var cep = GerarCep();

        // Define dinamicamente o nome da cidade de acordo com o SGBD que está sendo testado
        string cidadeExigida = dbType switch
        {
            DatabaseType.Sqlite => "SQLite",
            DatabaseType.SqlServer => "SQLServer",
            DatabaseType.MySql => "MySQL",
            _ => "SQLite"
        };

        // Cria o domínio rico de Logradouro respeitando as validações (Rua: Túlio, Bairro: Dutra, Cidade: conforme o banco)
        var logradouroResult = Logradouro.Criar(0, cep, "Brasil", "SC", cidadeExigida, "Dutra", "Túlio");
        if (logradouroResult.IsFailure)
        {
            throw new Exception($"Falha ao criar Logradouro para teste: {string.Join(", ", logradouroResult.Notifications.Select(n => n.Mensagem))}");
        }

        return await logradouroRepo.Adicionar(logradouroResult.Value!);
    }

    [Fact]
    public async Task Logradouro_Adicionar_E_ObterPorId_Sucesso()
    {
        // Arrange
        var cep = GerarCep();
        string cidadeExigida = DatabaseType switch
        {
            DatabaseType.Sqlite => "SQLite",
            DatabaseType.SqlServer => "SQLServer",
            DatabaseType.MySql => "MySQL",
            _ => "SQLite"
        };

        var logradouro = Logradouro.Criar(0, cep, "Brasil", "SC", cidadeExigida, "Dutra", "Túlio").Value!;

        // Act
        var inserido = await _repository.Adicionar(logradouro);

        // Assert
        Assert.NotNull(inserido);
        Assert.True(inserido.Id > 0);
        Assert.Equal(cep, inserido.Cep.Valor);
        Assert.Equal("Túlio", inserido.Nome);

        var obtido = await _repository.ObterPorId(inserido.Id);
        Assert.NotNull(obtido);
        Assert.Equal(inserido.Id, obtido.Id);
        Assert.Equal(cep, obtido.Cep.Valor);
    }

    [Fact]
    public async Task Logradouro_ObterPorId_RetornaNuloQuandoInexistente()
    {
        // Act
        var obtido = await _repository.ObterPorId(999999);

        // Assert
        Assert.Null(obtido);
    }

    [Fact]
    public async Task Logradouro_ObterTodos_Sucesso()
    {
        // Arrange
        await CriarEInserirLogradouroAsync(_repository, DatabaseType);

        // Act
        var todos = await _repository.ObterTodos();

        // Assert
        Assert.NotNull(todos);
        Assert.NotEmpty(todos);
    }

    [Fact]
    public async Task Logradouro_Atualizar_Sucesso()
    {
        // Arrange
        var logradouro = await CriarEInserirLogradouroAsync(_repository, DatabaseType);
        var novoCep = GerarCep();

        var logradouroAtualizado = Logradouro.Criar(logradouro.Id, novoCep, "Brasil", "SC", "Lages", "Dutra Novo", "Túlio").Value!;

        // Act
        var resultado = await _repository.Atualizar(logradouroAtualizado);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Túlio", resultado.Nome);
        Assert.Equal("Dutra Novo", resultado.Bairro);

        var noBanco = await _repository.ObterPorId(logradouro.Id);
        Assert.NotNull(noBanco);
        Assert.Equal("Dutra Novo", noBanco.Bairro);
    }

    [Fact]
    public async Task Logradouro_Atualizar_LancaExcecaoQuandoInexistente()
    {
        // Arrange
        var cep = GerarCep();
        var logradouroInexistente = Logradouro.Criar(999999, cep, "Brasil", "SC", "Cidade Fake", "Bairro Fake", "Rua Fake").Value!;

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InfrastructureException>(() => _repository.Atualizar(logradouroInexistente));
        Assert.Equal("REGISTRO_NAO_ENCONTRADO", ex.ErrorCode);
    }

    [Fact]
    public async Task Logradouro_Remover_Sucesso()
    {
        // Arrange
        var logradouro = await CriarEInserirLogradouroAsync(_repository, DatabaseType);

        // Act
        var removido = await _repository.Remover(logradouro.Id);

        // Assert
        Assert.True(removido);

        var noBanco = await _repository.ObterPorId(logradouro.Id);
        Assert.Null(noBanco);
    }

    [Fact]
    public async Task Logradouro_Remover_RetornaFalseQuandoInexistente()
    {
        // Act
        var removida = await _repository.Remover(999999);

        // Assert
        Assert.False(removida);
    }

    [Fact]
    public async Task Logradouro_ObterPorCep_SucessoENulo()
    {
        // Arrange
        var logradouro = await CriarEInserirLogradouroAsync(_repository, DatabaseType);

        // Act
        var obtido = await _repository.ObterPorCep(logradouro.Cep);

        // Assert
        Assert.NotNull(obtido);
        Assert.Equal(logradouro.Id, obtido.Id);

        var cepInexistente = Cep.Criar("99999999").Value!;
        var naoObtido = await _repository.ObterPorCep(cepInexistente);
        Assert.Null(naoObtido);
    }

    [Fact]
    public async Task Logradouro_CepJaExiste_ValidacaoCorreta()
    {
        // Arrange
        var logradouro = await CriarEInserirLogradouroAsync(_repository, DatabaseType);

        // Act & Assert
        var existe = await _repository.CepJaExiste(logradouro.Cep);
        Assert.True(existe);

        var existeMesmoId = await _repository.CepJaExiste(logradouro.Cep, logradouro.Id);
        Assert.False(existeMesmoId);

        var cepInedito = Cep.Criar(GerarCep()).Value!;
        var existeInedito = await _repository.CepJaExiste(cepInedito);
        Assert.False(existeInedito);
    }

    [Fact]
    public async Task Logradouro_ObterPorCidade_FiltragemCorreta()
    {
        // Arrange
        var cep = GerarCep();
        var cidadeUnica = "CidadeUnica_" + Guid.NewGuid().ToString("N")[..5];
        var logradouro = Logradouro.Criar(0, cep, "Brasil", "SC", cidadeUnica, "Dutra", "Túlio").Value!;
        await _repository.Adicionar(logradouro);

        // Act
        var resultados = await _repository.ObterPorCidade(cidadeUnica);

        // Assert
        Assert.NotNull(resultados);
        Assert.Single(resultados);
        Assert.Equal(cidadeUnica, resultados.First().Cidade);

        var resultadosVazio = await _repository.ObterPorCidade("CidadeInexistente_123");
        Assert.Empty(resultadosVazio);
    }

    [Fact]
    public async Task Logradouro_ObterPorBairro_FiltragemCorreta()
    {
        // Arrange
        var cep = GerarCep();
        var cidade = "Cidade_" + Guid.NewGuid().ToString("N")[..5];
        var bairro = "Bairro_" + Guid.NewGuid().ToString("N")[..5];
        var logradouro = Logradouro.Criar(0, cep, "Brasil", "SC", cidade, bairro, "Túlio").Value!;
        await _repository.Adicionar(logradouro);

        // Act
        var resultados = await _repository.ObterPorBairro(cidade, bairro);

        // Assert
        Assert.NotNull(resultados);
        Assert.Single(resultados);

        var resultadosVazio = await _repository.ObterPorBairro(cidade, "BairroInexistente");
        Assert.Empty(resultadosVazio);
    }
}