using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Exceptions;
using System.Data;
using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Repositories;

public class AlunoRepository : BaseRepository, IAlunoRepository
{
    public AlunoRepository(string connectionString, DatabaseType databaseType)
        : base(connectionString, databaseType)
    {
    }

    public AlunoRepository(Func<(string ConnectionString, DatabaseType DatabaseType)> configurationProvider)
        : base(configurationProvider)
    {
    }

    private static string BaseSelectQuery => @"
        SELECT
            a.id_aluno, a.cpf, a.nome, a.nascimento, a.telefone, a.email,
            a.logradouro_id, a.numero, a.complemento, a.senha, a.foto,
            l.id_logradouro, l.cep, l.nome AS logradouro_nome,
            l.bairro, l.cidade, l.estado, l.pais
        FROM tb_aluno a
        INNER JOIN tb_logradouro l ON a.logradouro_id = l.id_logradouro";

    public async Task<Aluno?> ObterPorId(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            string query = $"{BaseSelectQuery} WHERE a.id_aluno = @Id";
            await using var command = await CreateCommandAsync(query, cancellationToken);
            command.AddParameter("@Id", id, DbType.Int32);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_OBTER_ALUNO_POR_ID", $"Erro ao obter aluno por ID {id}: {ex.Message}", ex);
        }
    }

    public async Task<IReadOnlyCollection<Aluno>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            string query = $"{BaseSelectQuery} ORDER BY a.nome";
            await using var command = await CreateCommandAsync(query, cancellationToken);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            var alunos = new List<Aluno>();
            while (await reader.ReadAsync(cancellationToken))
                alunos.Add(Map(reader));
            return alunos.AsReadOnly();
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_OBTER_TODOS_ALUNOS", $"Erro ao obter todos os alunos: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<Aluno>> ObterTodos(CancellationToken cancellationToken = default)
    {
        return await ObterTodosAsync(cancellationToken);
    }

    public static Aluno Map(DbDataReader reader, string nomeColumn = "nome")
    {
        try
        {
            int id = reader.GetInt32Value("id_aluno");
            string cpf = reader.GetStringValue("cpf");
            string nome = reader.GetStringValue(nomeColumn);
            DateOnly nascimento = reader.GetDateOnlyValue("nascimento");
            string telefone = reader.GetStringValue("telefone");
            string email = reader.GetStringValue("email");
            string numero = reader.GetStringValue("numero");
            string complemento = reader.GetNullableString("complemento");
            string senha = reader.GetStringValue("senha");
            byte[]? fotoBytes = reader.GetNullableBytes("foto");
            Arquivo foto = fotoBytes is null
                ? Arquivo.Criar(Array.Empty<byte>()).Value!
                : Arquivo.Criar(fotoBytes).Value!;
            var logradouro = LogradouroRepository.Map(reader, "logradouro_nome");

            var result = Aluno.Criar(
                id, nome, cpf, nascimento, telefone, email, senha, foto,
                logradouro, numero, complemento);

            if (result.IsFailure)
            {
                throw new InfrastructureException(
                    "ERRO_DOMINIO_MAPEAMENTO_ALUNO",
                    $"Erro de domínio ao mapear aluno ID {id}: " +
                    string.Join(", ", result.Notifications.Select(n => n.Mensagem)));
            }

            return result.Value!;
        }
        catch (Exception ex) when (ex is not InfrastructureException)
        {
            throw new InfrastructureException(
                "ERRO_MAPEAMENTO_ALUNO",
                $"Erro ao mapear dados do aluno: {ex.Message}", ex);
        }
    }

    public async Task<Aluno> Adicionar(Aluno entidade, CancellationToken cancellationToken = default)
    {
        try
        {
            string query = FormatInsertQuery(@"
                INSERT INTO tb_aluno
                    (cpf, nome, nascimento, telefone, email, logradouro_id,
                     numero, complemento, senha, foto)
                VALUES
                    (@Cpf, @Nome, @Nascimento, @Telefone, @Email, @LogradouroId,
                     @Numero, @Complemento, @Senha, @Foto)");

            await using var command = await CreateCommandAsync(query, cancellationToken);
            AddEntityParameters(command, entidade);
            int id = await command.ExecuteScalarIdAsync(
                "ERRO_ADICIONAR_ALUNO",
                "Falha ao obter ID inserido para o aluno.", cancellationToken);
            typeof(Entity).GetProperty("Id")?.SetValue(entidade, id);
            return entidade;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_ADICIONAR_ALUNO", $"Erro ao adicionar aluno: {ex.Message}", ex);
        }
    }

    public async Task AdicionarAsync(Aluno entidade, CancellationToken cancellationToken = default)
    {
        await Adicionar(entidade, cancellationToken);
    }

    public async Task AtualizarAsync(Aluno entidade, CancellationToken cancellationToken = default)
    {
        try
        {
            const string query = @"
                UPDATE tb_aluno SET
                    cpf = @Cpf, nome = @Nome, nascimento = @Nascimento,
                    telefone = @Telefone, email = @Email, logradouro_id = @LogradouroId,
                    numero = @Numero, complemento = @Complemento, senha = @Senha,
                    foto = @Foto
                WHERE id_aluno = @Id";

            await using var command = await CreateCommandAsync(query, cancellationToken);
            command.AddParameter("@Id", entidade.Id, DbType.Int32);
            AddEntityParameters(command, entidade);
            int rows = await command.ExecuteNonQueryAsync(cancellationToken);
            if (rows == 0)
                throw new InfrastructureException("REGISTRO_ALUNO_NAO_ENCONTRADO", $"Nenhum aluno encontrado com ID {entidade.Id} para atualização.");
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_ATUALIZAR_ALUNO", $"Erro ao atualizar aluno ID {entidade.Id}: {ex.Message}", ex);
        }
    }

    public async Task<Aluno> Atualizar(Aluno entidade, CancellationToken cancellationToken = default)
    {
        await AtualizarAsync(entidade, cancellationToken);
        return entidade;
    }

    public async Task<Aluno?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await ObterPorId(id, cancellationToken);
    }

    public async Task RemoverAsync(Aluno entidade, CancellationToken cancellationToken = default)
    {
        await Remover(entidade.Id, cancellationToken);
    }

    public async Task<bool> Remover(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            const string query = "DELETE FROM tb_aluno WHERE id_aluno = @Id";
            await using var command = await CreateCommandAsync(query, cancellationToken);
            command.AddParameter("@Id", id, DbType.Int32);
            return await command.ExecuteNonQueryAsync(cancellationToken) > 0;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_REMOVER_ALUNO", $"Erro ao remover aluno ID {id}: {ex.Message}", ex);
        }
    }

    public Task<Aluno?> ObterPorCpf(Cpf cpf, CancellationToken cancellationToken = default) =>
        ObterUm($"{BaseSelectQuery} WHERE a.cpf = @Cpf", "@Cpf", cpf.Valor, "ERRO_OBTER_ALUNO_POR_CPF", cancellationToken);

    public async Task<Aluno?> ObterPorCpfAsync(string cpf, CancellationToken cancellationToken = default)
    {
        var cpfResult = Cpf.Criar(cpf);
        if (cpfResult.IsFailure)
            return null;
        return await ObterPorCpf(cpfResult.Value!, cancellationToken);
    }

    private static void AddEntityParameters(DbCommand command, Aluno aluno)
    {
        command.AddParameter("@Cpf", aluno.Cpf.Valor, DbType.String);
        command.AddParameter("@Nome", aluno.NomeCompleto, DbType.String);
        command.AddParameter("@Nascimento", aluno.DataNascimento, DbType.Date);
        command.AddParameter("@Telefone", aluno.Telefone.Numero, DbType.String);
        command.AddParameter("@Email", aluno.Email.Endereco, DbType.String);
        command.AddParameter("@LogradouroId", aluno.Logradouro.Id, DbType.Int32);
        command.AddParameter("@Numero", aluno.Numero, DbType.String);
        command.AddParameter("@Complemento", (object?)aluno.Complemento ?? DBNull.Value, DbType.String);
        command.AddParameter("@Senha", aluno.Senha.Valor, DbType.String);
        command.AddParameter("@Foto", (object?)aluno.Foto?.Conteudo ?? DBNull.Value, DbType.Binary);
    }

    private async Task<Aluno?> ObterUm(string query, string parameter, object value, string errorCode, CancellationToken cancellationToken)
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
            throw new InfrastructureException(errorCode, $"Erro ao consultar aluno: {ex.Message}", ex);
        }
    }
}
