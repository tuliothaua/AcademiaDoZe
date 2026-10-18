# Atividade 11 — CRUD de alunos e colaboradores

Implementação sobre o projeto atualizado na atividade 10, conforme o material da aula 11 (`dev11de20.pdf`). O enunciado pede CRUD completo de **Aluno** e **Colaborador**, Dashboard com a logo que contém o nome do aluno e um vídeo demonstrativo de cadastro, busca por CPF, edição com troca de foto e exclusão para os dois tipos de pessoa.

## O que foi implementado

- Novas áreas **Alunos** e **Colaboradores** no menu do Shell, com listas responsivas, fotos, busca por CPF, cadastro, edição e exclusão com confirmação.
- Formulários com dados pessoais, contato, endereço já cadastrado, senha e foto; para colaborador também existem data de admissão, cargo e vínculo. Os formulários permitem cadastrar um logradouro sem perder o rascunho da pessoa.
- Seleção de imagem por galeria/arquivos ou captura pela câmera (`FilePicker` e `MediaPicker`), com solicitação de permissão de câmera e limite de 8 MB para a imagem.
- A foto é convertida para bytes e gravada no campo binário do banco já usado pela infraestrutura. A edição pode trocar a imagem pela galeria ou câmera.
- A busca da listagem chama `ObterPorCpfAsync` após validar 11 dígitos. A exclusão pede confirmação.
- As regras de domínio são preservadas; ao editar, deixar a senha em branco mantém a senha atual. Uma nova senha só é gravada quando preenchida e confirmada.
- Permissões para câmera adicionadas ao Android, iOS, MacCatalyst e Windows.
- A Dashboard já entregue na atividade 10 foi mantida: a imagem `tuliologo.png` contém o nome **Túlio Thauã Dutra**.

## Como executar e preparar os registros

1. Abra `AcademiaDoZe.slnx` no Visual Studio com a carga de trabalho **.NET MAUI** instalada, restaure os pacotes e selecione um dispositivo/emulador compatível.
2. Inicie com SQLite local (padrão) ou use **Configurações** para selecionar um servidor configurado no ambiente de execução.
3. Pelo menu, abra **Logradouros** e cadastre pelo menos um endereço se a base estiver vazia; aluno e colaborador precisam selecionar um logradouro existente.
4. Abra **Alunos** ou **Colaboradores** e cadastre todos os campos obrigatórios. Para a evidência, use um CPF de teste com 11 dígitos não cadastrado e uma foto sua/autorizada. A câmera pode ser negada pelo sistema; nesse caso, permita o acesso ou escolha uma imagem na galeria.
5. O campo de busca da listagem exige 11 dígitos de CPF (com ou sem pontuação). Use **Limpar** para voltar à lista completa.

## Roteiro do vídeo exigido

Faça uma demonstração legível e com a tela inteira visível. Não grave senhas, credenciais de banco, tokens ou dados pessoais de terceiros.

1. Mostre a Dashboard e a imagem com o nome do aluno; abra o menu.
2. **Colaborador:** cadastre um novo colaborador com foto, preferencialmente tirada na hora; volte à listagem, filtre pelo CPF, edite o registro, altere o nome para **“zé dos testes”** e substitua a foto escolhendo uma imagem da galeria; filtre novamente e exclua o registro.
3. **Aluno:** repita o mesmo fluxo: cadastro com foto, busca por CPF, edição do nome para **“zé dos testes”**, troca da foto pela galeria e exclusão.
4. Mostre as confirmações e que cada registro desaparece da lista após a exclusão.

## Entrega no Classroom

O PDF da aula especifica **três arquivos**: (1) ZIP/release do projeto completo; (2) vídeo da execução; (3) comentário ou arquivo de texto contendo o link do repositório Git de sua propriedade. Para concluir, ainda será necessário publicar este projeto no seu GitHub, criar a release/ZIP conforme a orientação da turma, gravar o vídeo no seu computador e anexar os três itens no Classroom. O ZIP preparado com este trabalho contém o código-fonte e esta documentação, mas não contém uma gravação de tela nem um repositório publicado.

> Não consegui executar o aplicativo MAUI Android neste ambiente sem o Android SDK configurado; valide no Visual Studio/emulador do seu computador antes da gravação. As camadas de domínio, aplicação e infraestrutura devem ser testadas conforme o checklist acima.


## Validação técnica executada

- `dotnet build AcademiaDoZe.Application/AcademiaDoZe.Application.csproj`: sucesso.
- Testes de domínio: 162 aprovados, 0 falhas.
- Testes de infraestrutura: 44 aprovados, 0 falhas.
- Os XMLs dos XAML, projetos e manifests foram parseados sem erros.
- O build Android do MAUI não foi concluído neste sandbox porque falta instalar/configurar o Android SDK. Restaure os pacotes e compile no Visual Studio/Android Studio do computador onde fará a gravação. A restauração também reportou avisos existentes de vulnerabilidade para `System.Security.Cryptography.Pkcs` 9.0.13; não são falhas dos testes desta atividade e merecem atualização de dependência antes de uso em produção.
