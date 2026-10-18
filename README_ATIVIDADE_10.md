# Atividade 10 — Preferências e identidade visual

Implementação acadêmica para o projeto **Academia do Zé**, alinhada ao material da aula 10.

## Requisitos implementados

- Página **Configurações** no menu do aplicativo.
- Preferência de tema **Claro**, **Escuro** ou **Usar tema do sistema**, salva em `Preferences` e aplicada imediatamente por `WeakReferenceMessenger`.
- Configuração de **SQLite**, **SQL Server** ou **MySQL**. A conexão é validada/inicializada antes de persistir; o tipo e as credenciais ficam em `Preferences`.
- Mensagem de alteração do banco e leitura dinâmica do `RepositoryConfig` pelos repositórios, descartando a conexão anterior e usando a nova na operação seguinte.
- Menu real para **Dashboard**, **Logradouros** e **Configurações**, com ícones.
- Ícone do aplicativo e título de exibição personalizados.
- Dashboard exibe a logo com o nome **Túlio Thauã Dutra**.
- Cores principais das páginas adaptadas aos temas claro e escuro.

## Como abrir e executar

1. Abra `AcademiaDoZe.slnx` no Visual Studio em um ambiente com suporte ao .NET MAUI.
2. Restaure os pacotes NuGet e selecione um destino instalado, por exemplo Windows ou Android.
3. Execute o aplicativo. Por padrão, a configuração mantém o banco SQLite local no diretório de dados do app.
4. Abra **Configurações** pelo menu.
5. Troque o tema e confirme que a tela muda sem reiniciar.
6. Para demonstrar a mudança de credenciais, configure um SQL Server ou MySQL acessível no ambiente de execução; informe servidor, porta, nome do banco, usuário e senha e use **Testar e salvar conexão**. A nova conexão só é aplicada se a inicialização for validada.
7. Volte ao Dashboard e confira que a navegação, a logo, o título e os ícones aparecem corretamente.

> A troca para um banco remoto depende de um servidor alcançável pelo dispositivo/emulador e das permissões do usuário configurado. Para a demonstração, não grave a senha no vídeo.

## Checklist para o vídeo

Grave a tela com resolução legível e áudio opcional. Mostre, nesta ordem:

1. Título/ícone personalizados e menu com ícones.
2. Dashboard com a logo que contém o nome do aluno.
3. Abrir **Configurações**, alternar Claro/Escuro e voltar ao app para mostrar o efeito imediato.
4. Informar/validar as credenciais de um banco remoto de demonstração e salvar; ocultar a senha durante a gravação.
5. Voltar ao Dashboard e mostrar a aplicação usando a conexão alterada.

## Preparação da entrega

O material da disciplina solicita o endereço do repositório Git, um ZIP/release do projeto, um vídeo, a imagem da logo e um texto com o link do Git. Este pacote contém o projeto e a logo existente (`AcademiaDoZe.Presentation.AppMaui/Resources/Images/tuliologo.png`). Ainda é necessário, no seu computador/conta:

- publicar as alterações no seu repositório Git e criar a release/ZIP exigida;
- gravar o vídeo demonstrativo, pois ele precisa mostrar a execução no seu ambiente e credenciais reais não devem ser compartilhadas;
- anexar o vídeo, a logo e o texto com o link no Classroom.

## Observação de segurança

A disciplina pede o uso de `Preferences` e este exercício persiste as credenciais conforme o material. `Preferences` não é um cofre de segredos: em uma aplicação de produção, prefira `SecureStorage` para senhas e use permissões/gestão segura de credenciais no servidor.
