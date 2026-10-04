using AcademiaDoZe.Application.DependencyInjection;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Presentation.AppMaui.Configuration;
using AcademiaDoZe.Presentation.AppMaui.Message;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Storage;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class ConfigPage : ContentPage
{
    private readonly RepositoryConfig _repositoryConfig;

    public ConfigPage(RepositoryConfig repositoryConfig)
    {
        InitializeComponent();
        _repositoryConfig = repositoryConfig;

        TemaPicker.ItemsSource = new[] { "Claro", "Escuro", "Usar tema do sistema" };
        BancoPicker.ItemsSource = new[] { "SQLite (local)", "SQL Server", "MySQL" };
        CarregarPreferencias();
    }

    private void CarregarPreferencias()
    {
        TemaPicker.SelectedIndex = Preferences.Default.Get("Tema", "system") switch
        {
            "light" => 0,
            "dark" => 1,
            _ => 2
        };

        var savedType = _repositoryConfig.DatabaseType.ToApplication();
        BancoPicker.SelectedIndex = savedType switch
        {
            AppDatabaseType.SqlServer => 1,
            AppDatabaseType.MySql => 2,
            _ => 0
        };

        SqlitePathEntry.Text = Preferences.Default.Get(
            "Sqlite_Caminho",
            Path.Combine(FileSystem.AppDataDirectory, "db_academia_do_ze.db"));
        ServidorEntry.Text = Preferences.Default.Get("Banco_Servidor", string.Empty);
        PortaEntry.Text = Preferences.Default.Get("Banco_Porta", string.Empty);
        NomeBancoEntry.Text = Preferences.Default.Get("Banco_Nome", "db_academia_do_ze");
        UsuarioEntry.Text = Preferences.Default.Get("Banco_Usuario", string.Empty);
        SenhaEntry.Text = Preferences.Default.Get("Banco_Senha", string.Empty);
        AtualizarCamposBanco();
    }

    private void OnTipoBancoChanged(object? sender, EventArgs e) => AtualizarCamposBanco();

    private void AtualizarCamposBanco()
    {
        var sqliteSelected = BancoPicker.SelectedIndex == 0;
        SqliteInfo.IsVisible = sqliteSelected;
        SqlitePathEntry.IsVisible = sqliteSelected;
        BancoRemotoFields.IsVisible = BancoPicker.SelectedIndex is 1 or 2;
    }

    private async void OnSalvarTemaClicked(object? sender, EventArgs e)
    {
        if (TemaPicker.SelectedIndex < 0)
        {
            await DisplayAlert("Tema", "Selecione uma opção de tema.", "OK");
            return;
        }

        var theme = TemaPicker.SelectedIndex switch
        {
            0 => "light",
            1 => "dark",
            _ => "system"
        };

        Preferences.Default.Set("Tema", theme);
        WeakReferenceMessenger.Default.Send(new TemaPreferencesUpdatedMessage(theme));
        await DisplayAlert("Tema atualizado", "A preferência foi salva e aplicada agora.", "OK");
    }

    private async void OnSalvarBancoClicked(object? sender, EventArgs e)
    {
        if (BancoPicker.SelectedIndex < 0)
        {
            await DisplayAlert("Banco de dados", "Selecione um tipo de banco.", "OK");
            return;
        }

        var type = BancoPicker.SelectedIndex switch
        {
            1 => AppDatabaseType.SqlServer,
            2 => AppDatabaseType.MySql,
            _ => AppDatabaseType.Sqlite
        };

        try
        {
            var candidate = ConfigurationHelper.BuildRepositoryConfig(
                type,
                ServidorEntry.Text ?? string.Empty,
                PortaEntry.Text ?? string.Empty,
                NomeBancoEntry.Text ?? string.Empty,
                UsuarioEntry.Text ?? string.Empty,
                SenhaEntry.Text ?? string.Empty,
                SqlitePathEntry.Text ?? string.Empty);

            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(20));
            await DbInitializer.InicializarAsync(
                candidate.ConnectionString,
                candidate.DatabaseType,
                timeout.Token);

            Preferences.Default.Set("Banco_Tipo", type.ToString());
            Preferences.Default.Set("Sqlite_Caminho", SqlitePathEntry.Text?.Trim() ?? string.Empty);
            Preferences.Default.Set("Banco_Servidor", ServidorEntry.Text?.Trim() ?? string.Empty);
            Preferences.Default.Set("Banco_Porta", PortaEntry.Text?.Trim() ?? string.Empty);
            Preferences.Default.Set("Banco_Nome", NomeBancoEntry.Text?.Trim() ?? string.Empty);
            Preferences.Default.Set("Banco_Usuario", UsuarioEntry.Text?.Trim() ?? string.Empty);
            Preferences.Default.Set("Banco_Senha", SenhaEntry.Text ?? string.Empty);

            // O singleton lido pelos repositórios é atualizado; eles descartam conexões antigas
            // e abrem a nova conexão na próxima operação, sem reiniciar o aplicativo.
            _repositoryConfig.ConnectionString = candidate.ConnectionString;
            _repositoryConfig.DatabaseType = candidate.DatabaseType;
            WeakReferenceMessenger.Default.Send(new BancoPreferencesUpdatedMessage(type.ToString()));

            await DisplayAlert("Conexão atualizada", "O banco foi validado e a nova configuração já está ativa.", "OK");
            await Shell.Current.GoToAsync("//dashboard");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Falha ao validar o banco configurado: {ex}");
            await DisplayAlert(
                "Não foi possível salvar",
                "A conexão não foi validada. Revise servidor, porta, banco e credenciais e tente novamente. Os detalhes técnicos foram enviados à saída de depuração.",
                "OK");
        }
    }

    private async void OnCancelarClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//dashboard");
    }
}
