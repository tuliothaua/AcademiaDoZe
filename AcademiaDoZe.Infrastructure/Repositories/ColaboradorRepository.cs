using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Exceptions;
using System.Data;
using System.Data.Common;
using System.Threading;

namespace AcademiaDoZe.Infrastructure.Repositories;

public class ColaboradorRepository : BaseRepository, IColaboradorRepository
{
    public ColaboradorRepository(string connectionString, DatabaseType databaseType)
        : base(connectionString, databaseType)
    {
    }

    private static string BaseSelectQuery => @"
        SELECT
            c.id_colaborador, c.cpf, c.nome, c.nascimento, c.telefone, c.email,
            c.logradouro_id, c.numero, c.complemento, c.senha, c.foto,
            c.admissao, c.tipo, c.vinculo,
            l.id_logradouro, l.cep, l.nome AS logradouro_nome,
            l.bairro, l.cidade, l.estado, l.pais
        FROM tb_colaborador c
        INNER JOIN tb_logradouro l ON c.logradouro_id = l.id_logradouro";

    public async Task<Colaborador?> ObterPorId(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            string query = $"{BaseSelectQuery} WHERE c.id_colaborador = @Id";
            await using var command = await CreateCommandAsync(query, cancellationToken);
            command.AddParameter("@Id", id, DbType.Int32);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_OBTER_POR_ID", $"Erro ao obter colaborador por ID {id}: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<Colaborador>> ObterTodos(CancellationToken cancellationToken = default)
    {
        try
        {
            string query = $"{BaseSelectQuery} ORDER BY c.nome";
            await using var command = await CreateCommandAsync(query, cancellationToken);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            var colaboradores = new List<Colaborador>();
            while (await reader.ReadAsync(cancellationToken))
                colaboradores.Add(Map(reader));
            return colaboradores;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_OBTER_TODOS", $"Erro ao obter todos os colaboradores: {ex.Message}", ex);
        }
    }

    public static Colaborador Map(DbDataReader reader, string nomeColumn = "nome")
    {
        try
        {
            int id = reader.GetInt32Value("id_colaborador");
            string cpf = reader.GetStringValue("cpf");
            string nome = reader.GetStringValue(nomeColumn);
            DateOnly nascimento = reader.GetDateOnlyValue("nascimento");
            string telefone = reader.GetStringValue("telefone");
            string email = reader.GetStringValue("email");
            string numero = reader.GetStringValue("numero");
            string complemento = reader.GetNullableString("complemento");
            string senha = reader.GetStringValue("senha");
            DateOnly admissao = reader.GetDateOnlyValue("admissao");
            var tipo = (ColaboradorTipo)reader.GetInt32Value("tipo");
            var vinculo = (ColaboradorVinculo)reader.GetInt32Value("vinculo");

            byte[]? fotoBytes = reader.GetNullableBytes("foto");
            Arquivo? foto = fotoBytes is null ? null : Arquivo.Criar(fotoBytes).Value;
            var logradouro = LogradouroRepository.Map(reader, "logradouro_nome");

            var result = Colaborador.Criar(
                id, nome, cpf, nascimento, telefone, email, senha, foto!, logradouro,
                numero, complemento, admissao, tipo, vinculo);

            if (result.IsFailure)
            {
                throw new InfrastructureException(
                    "ERRO_DOMINIO_MAPEAMENTO",
                    $"Erro de domínio ao mapear colaborador ID {id}: " +
                    string.Join(", ", result.Notifications.Select(n => n.Mensagem)));
            }

            return result.Value!;
        }
        catch (Exception ex) when (ex is not InfrastructureException)
        {
            throw new InfrastructureException(
                "ERRO_MAPEAMENTO_COLABORADOR",
                $"Erro ao mapear dados do colaborador: {ex.Message}", ex);
        }
    }

    public async Task<Colaborador> Adicionar(Colaborador entity, CancellationToken cancellationToken = default)
    {
        try
        {
            string query = FormatInsertQuery(@"
                INSERT INTO tb_colaborador
                    (cpf, nome, nascimento, telefone, email, logradouro_id,
                     numero, complemento, senha, foto, admissao, tipo, vinculo)
                VALUES
                    (@Cpf, @Nome, @Nascimento, @Telefone, @Email, @LogradouroId,
                     @Numero, @Complemento, @Senha, @Foto, @Admissao, @Tipo, @Vinculo)");

            await using var command = await CreateCommandAsync(query, cancellationToken);
            AddEntityParameters(command, entity);
            int id = await command.ExecuteScalarIdAsync(
                "ERRO_ADICIONAR_COLABORADOR",
                "Falha ao obter ID inserido para o colaborador.", cancellationToken);
            typeof(Entity).GetProperty("Id")?.SetValue(entity, id);
            return entity;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_ADICIONAR_COLABORADOR", $"Erro ao adicionar colaborador: {ex.Message}", ex);
        }
    }

    public async Task<Colaborador> Atualizar(Colaborador entity, CancellationToken cancellationToken = default)
    {
        try
        {
            const string query = @"
                UPDATE tb_colaborador SET
                    cpf = @Cpf, nome = @Nome, nascimento = @Nascimento,
                    telefone = @Telefone, email = @Email, logradouro_id = @LogradouroId,
                    numero = @Numero, complemento = @Complemento, senha = @Senha,
                    foto = @Foto, admissao = @Admissao, tipo = @Tipo, vinculo = @Vinculo
                WHERE id_colaborador = @Id";

            await using var command = await CreateCommandAsync(query, cancellationToken);
            command.AddParameter("@Id", entity.Id, DbType.Int32);
            AddEntityParameters(command, entity);
            int rows = await command.ExecuteNonQueryAsync(cancellationToken);
            if (rows == 0)
                throw new InfrastructureException("REGISTRO_NAO_ENCONTRADO", $"Nenhum colaborador encontrado com ID {entity.Id} para atualização.");
            return entity;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_ATUALIZAR_COLABORADOR", $"Erro ao atualizar colaborador ID {entity.Id}: {ex.Message}", ex);
        }
    }

    public async Task<bool> Remover(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            const string query = "DELETE FROM tb_colaborador WHERE id_colaborador = @Id";
            await using var command = await CreateCommandAsync(query, cancellationToken);
            command.AddParameter("@Id", id, DbType.Int32);
            return await command.ExecuteNonQueryAsync(cancellationToken) > 0;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_REMOVER_COLABORADOR", $"Erro ao remover colaborador ID {id}: {ex.Message}", ex);
        }
    }

    public Task<Colaborador?> ObterPorCpf(Cpf cpf, CancellationToken cancellationToken = default) =>
        ObterUm($"{BaseSelectQuery} WHERE c.cpf = @Cpf", "@Cpf", cpf.Valor, "ERRO_OBTER_POR_CPF", cancellationToken);

    // Implementação exigida pela interface IColaboradorRepository que recebe CPF como string
    public async Task<Colaborador?> ObterPorCpfAsync(string cpf, CancellationToken cancellationToken = default)
    {
        var cpfResult = Cpf.Criar(cpf);
        if (cpfResult.IsFailure) return null;
        return await ObterPorCpf(cpfResult.Value, cancellationToken);
    }

    public Task<Colaborador?> ObterPorEmail(Email email, CancellationToken cancellationToken = default) =>
        ObterUm($"{BaseSelectQuery} WHERE c.email = @Email", "@Email", email.Endereco, "ERRO_OBTER_POR_EMAIL", cancellationToken);

    public async Task<bool> CpfJaExiste(Cpf cpf, int? id = null, CancellationToken cancellationToken = default)
    {
        return await Existe("cpf", cpf.Valor, id, "ERRO_VERIFICAR_CPF", cancellationToken);
    }

    public async Task<bool> EmailJaExiste(Email email, int? id = null, CancellationToken cancellationToken = default)
    {
        return await Existe("email", email.Endereco, id, "ERRO_VERIFICAR_EMAIL", cancellationToken);
    }

    public Task<IEnumerable<Colaborador>> ObterPorTipo(ColaboradorTipo tipo, CancellationToken cancellationToken = default) =>
        ObterLista($"{BaseSelectQuery} WHERE c.tipo = @Tipo ORDER BY c.nome", "@Tipo", (int)tipo, "ERRO_OBTER_POR_TIPO", cancellationToken);

    public Task<IEnumerable<Colaborador>> ObterPorVinculo(ColaboradorVinculo vinculo, CancellationToken cancellationToken = default) =>
        ObterLista($"{BaseSelectQuery} WHERE c.vinculo = @Vinculo ORDER BY c.nome", "@Vinculo", (int)vinculo, "ERRO_OBTER_POR_VINCULO", cancellationToken);

    public async Task<bool> TrocarSenha(int id, Senha novaSenha, CancellationToken cancellationToken = default)
    {
        try
        {
            const string query = "UPDATE tb_colaborador SET senha = @Senha WHERE id_colaborador = @Id";
            await using var command = await CreateCommandAsync(query, cancellationToken);
            command.AddParameter("@Id", id, DbType.Int32);
            command.AddParameter("@Senha", novaSenha.Valor, DbType.String);
            return await command.ExecuteNonQueryAsync(cancellationToken) > 0;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_TROCAR_SENHA", $"Erro ao trocar senha do colaborador ID {id}: {ex.Message}", ex);
        }
    }

    // Implementação dos métodos do IRepository<T> herdados via IColaboradorRepository
    public Task<Colaborador?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default) =>
        ObterPorId(id, cancellationToken);

    public async Task<System.Collections.Generic.IReadOnlyCollection<Colaborador>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        var list = await ObterTodos(cancellationToken);
        return new System.Collections.Generic.List<Colaborador>(list).AsReadOnly();
    }

    public async Task AdicionarAsync(Colaborador entidade, CancellationToken cancellationToken = default)
    {
        await Adicionar(entidade, cancellationToken);
    }

    public async Task AtualizarAsync(Colaborador entidade, CancellationToken cancellationToken = default)
    {
        await Atualizar(entidade, cancellationToken);
    }

    public async Task RemoverAsync(Colaborador entidade, CancellationToken cancellationToken = default)
    {
        await Remover(entidade.Id, cancellationToken);
    }

    private static void AddEntityParameters(DbCommand command, Colaborador entity)
    {
        command.AddParameter("@Cpf", entity.Cpf.Valor, DbType.String);
        command.AddParameter("@Nome", entity.NomeCompleto, DbType.String);
        command.AddParameter("@Nascimento", entity.DataNascimento, DbType.Date);
        command.AddParameter("@Telefone", entity.Telefone.Numero, DbType.String);
        command.AddParameter("@Email", entity.Email.Endereco, DbType.String);
        command.AddParameter("@LogradouroId", entity.Logradouro.Id, DbType.Int32);
        command.AddParameter("@Numero", entity.Numero, DbType.String);
        command.AddParameter("@Complemento", (object?)entity.Complemento ?? DBNull.Value, DbType.String);
        command.AddParameter("@Senha", entity.Senha.Valor, DbType.String);
        command.AddParameter("@Foto", (object?)entity.Foto?.Conteudo ?? DBNull.Value, DbType.Binary);
        command.AddParameter("@Admissao", entity.DataAdmissao, DbType.Date);
        command.AddParameter("@Tipo", (int)entity.Tipo, DbType.Int32);
        command.AddParameter("@Vinculo", (int)entity.Vinculo, DbType.Int32);
    }

    private async Task<Colaborador?> ObterUm(string query, string parameter, object value, string errorCode, CancellationToken cancellationToken)
    {
        try
        {
            await using var command = await CreateCommandAsync(query, cancellationToken);
            command.AddParameter(parameter, value, DbType.String);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException(errorCode, $"Erro ao consultar colaborador: {ex.Message}", ex);
        }
    }

    private async Task<IEnumerable<Colaborador>> ObterLista(string query, string parameter, object value, string errorCode, CancellationToken cancellationToken)
    {
        try
        {
            await using var command = await CreateCommandAsync(query, cancellationToken);
            command.AddParameter(parameter, value, DbType.Int32);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            var lista = new List<Colaborador>();
            while (await reader.ReadAsync(cancellationToken)) lista.Add(Map(reader));
            return lista;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException(errorCode, $"Erro ao consultar colaboradores: {ex.Message}", ex);
        }
    }

    private async Task<bool> Existe(string column, string value, int? id, string errorCode, CancellationToken cancellationToken)
    {
        try
        {
            string query = $"SELECT COUNT(1) FROM tb_colaborador WHERE {column} = @Value AND (@Id IS NULL OR id_colaborador <> @Id)";
            await using var command = await CreateCommandAsync(query, cancellationToken);
            command.AddParameter("@Value", value, DbType.String);
            command.AddParameter("@Id", (object?)id ?? DBNull.Value, DbType.Int32);
            return Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken)) > 0;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException(errorCode, $"Erro ao verificar existência: {ex.Message}", ex);
        }
    }
}
