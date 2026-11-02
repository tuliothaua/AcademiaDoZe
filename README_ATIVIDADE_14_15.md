# Atividade 14/15 — Matrículas

Implementação do CRUD de matrículas para o projeto AcademiaDoZe, seguindo o material da aula e as regras do enunciado.

## Funcionalidades

- Menu **Matrículas**, com listagem, busca por nome/CPF/ID, inclusão, edição e exclusão com confirmação.
- Seleção de aluno cadastrado e resumo de nome, CPF, nascimento e miniatura da foto.
- Planos mensal, trimestral, semestral e anual; a data final é calculada como início + duração do plano − 1 dia.
- Seleção de múltiplas restrições: diabetes, pressão alta, labirintite, alergias e problemas respiratórios.
- Anexo do laudo/parecer médico, armazenado junto à matrícula (limite de 10 MB).
- Exigência de laudo para aluno com idade entre 12 e 16 anos na data de início e para matrícula com qualquer restrição médica, validada na interface e no domínio.
- Bloqueio de novo cadastro e de troca do aluno em edição quando já existe outra matrícula vigente para esse aluno.

## Executar e verificar

1. Abra `AcademiaDoZe.slnx` no Visual Studio com a carga de trabalho .NET MAUI instalada; restaure os pacotes e compile para Windows ou Android.
2. Inicie a aplicação com o banco configurado. Cadastre alunos com foto; para os testes de matrícula, confirme que há ao menos um aluno e endereço cadastrados.
3. Abra **Matrículas** e demonstre seleção do aluno, preenchimento do plano/data, cálculo automático da data final e seleção de restrições múltiplas.
4. Anexe um arquivo de teste como laudo quando a idade ou as restrições exigirem; salve e confirme a matrícula na listagem.
5. Edite a matrícula e altere as restrições; volte à listagem, busque o aluno e exclua o registro.
6. Faça testes adicionais: tentativa de cadastrar outra matrícula vigente para o mesmo aluno, aluno entre 12–16 sem laudo e restrição sem laudo devem ser bloqueados.

## Testes e limitações

A compilação validada nesta entrega foi para `net10.0-windows10.0.19041.0`; os testes unitários de domínio verificam também as duas regras de laudo. O banco local precisa conter o schema de matrícula e alunos. Ainda é necessário executar o fluxo no dispositivo/emulador usado para gravar o vídeo, conferir a configuração do banco nesse ambiente e validar a leitura/gravação do anexo no sistema operacional escolhido.

## Entrega do professor

O roteiro de vídeo deve mostrar o aluno que tem o nome do estudante, incluindo nome, CPF, nascimento e miniatura da foto; a data final calculada, seleção de múltiplas restrições, cadastro e listagem; depois editar as restrições, localizar novamente e excluir. O enunciado também pede link do repositório Git e ZIP da release do GitHub. A gravação, publicação da release e postagem no Classroom são etapas manuais que ainda precisam ser realizadas pelo estudante.
