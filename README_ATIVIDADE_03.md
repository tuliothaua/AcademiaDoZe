# DevAval03 — Entrada e saída da academia

Implementação do controle de acesso do AcademiaDoZe em .NET MAUI, seguindo as camadas de domínio, aplicação, infraestrutura e apresentação já usadas no projeto.

## Funcionalidades

- Menu **Entrada e saída** e atalho na Dashboard.
- Eventos de acesso persistidos com pessoa, data e hora na tabela `tb_acesso` existente.
- Seleção de aluno/colaborador pelo nome completo.
- Entrada de aluno negada quando não existe matrícula vigente; em entrada autorizada, a tela informa quantos dias restam e a data de vencimento do plano.
- Saída de aluno vinculada a uma entrada aberta do mesmo aluno no dia; exibe o tempo da visita.
- Entrada e saída de colaborador registradas para a mesma pessoa; CLT tem limite diário de 8 horas e estágio de 6 horas.
- Saída de colaborador informa a soma dos períodos registrados no dia. A entrada é bloqueada se o limite diário já tiver sido atingido; se a saída confirmar que o total passou do limite, o sistema registra a saída e alerta, para não deixar a pessoa presa com uma entrada aberta.
- A logo com o nome completo **Túlio Thauã Dutra** já está exibida na Dashboard.

## Preparar e testar

1. Abra `AcademiaDoZe.slnx` no Visual Studio com a carga de trabalho .NET MAUI e selecione o alvo Windows ou Android.
2. Inicie a aplicação com o banco configurado. O padrão é SQLite local; a tabela de acessos faz parte do script de banco existente e é criada pela inicialização da infraestrutura.
3. Cadastre um logradouro, um aluno e um colaborador com nomes completos. Configure o vínculo do colaborador como CLT ou estágio.
4. Para testar a rejeição do aluno, deixe esse aluno sem matrícula ativa e tente registrar entrada: o sistema deve negar e explicar o motivo.
5. Cadastre uma matrícula ativa para outro aluno (ou para o mesmo depois do teste de rejeição). Registre a entrada e confirme a mensagem com os dias restantes do plano. Selecione exatamente o mesmo aluno para registrar a saída e confira o tempo de permanência.
6. Selecione um colaborador. Registre entrada e saída com o mesmo nome. Confira o horário e o total diário; o total considera todos os períodos registrados naquele dia.
7. Confira a opção de controle também na Dashboard e no menu lateral.

## Roteiro recomendado para o vídeo do professor

1. Mostre a Dashboard e a imagem da logo contendo **Túlio Thauã Dutra**.
2. No controle de acesso, selecione um aluno **sem matrícula ativa** e tente a entrada para demonstrar a validação/negação.
3. Selecione um aluno com matrícula ativa e nome completo. Registre a entrada, mostre a mensagem de plano restante e, ainda no mesmo dia, registre a saída para esse mesmo aluno, evidenciando o tempo de permanência.
4. Selecione um colaborador com nome completo. Registre a entrada e a saída para esse mesmo colaborador e mostre o total do dia e o limite correspondente ao vínculo.
5. Grave a tela com resolução e texto legíveis, evitando exibir dados pessoais de terceiros ou credenciais do banco.

## Verificação técnica e pendências de entrega

- A compilação da solução `AcademiaDoZe.slnx` passou para os alvos MAUI configurados no Windows; os testes existentes de domínio (164) e infraestrutura (44) passaram, totalizando 208 testes.
- Os avisos atuais de `DisplayAlert` obsoleto são preexistentes na tela de Configurações.
- Faça a execução manual no Visual Studio com o banco de desenvolvimento e percorra os cenários acima antes de gravar; não foi criado cadastro de teste nem alterado o banco local do usuário.
- A avaliação exige publicar/atualizar o repositório Git próprio, criar uma release no GitHub e anexar três itens no Classroom: link Git, ZIP da release e vídeo. Esses passos não são feitos automaticamente por esta implementação; especialmente não publiquei alterações nem criei release ou postagem.
- O cálculo da jornada é feito para os registros do dia corrente; conclua a saída no mesmo dia em que registrar a entrada.
