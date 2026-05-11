# CI/CD - Wex Purchases API

## Visao geral

Este documento descreve a estrategia de CI/CD do projeto `Wex Purchases API` com GitHub Actions, Amazon ECR e Amazon ECS Fargate.

A estrutura foi separada por branch para refletir o ciclo de vida de entrega:

- `develop` dispara validacoes completas e deploy em ambiente de desenvolvimento/homologacao.
- `main` dispara o pipeline de producao com cobertura minima, aprovacao por environment, health check e rollback automatico.

Arquivos criados:

- `.github/workflows/ci-develop.yml`
- `.github/workflows/ci-main.yml`

Os workflows de branch atuam como orquestradores e chamam arquivos reutilizaveis por job no padrao `.github/workflows/ci-shared-*.yml`.

## Arquitetura dos pipelines

Os workflows foram organizados em dois blocos claros:

- `CI`: checkout, setup do .NET, restore, build, testes, validacao de formatacao e warnings.
- `CD`: build da imagem, push para o Amazon ECR e deploy no Amazon ECS Fargate.

Principais decisoes tecnicas:

- `Ubuntu latest`: runner padrao moderno, estavel e com Docker, AWS CLI, `jq`, `curl` e Python disponiveis.
- `actions/checkout@v4`: versao atual oficial para obter o codigo no runner.
- `actions/setup-dotnet@v4`: instala o .NET 10 e habilita cache de dependencias NuGet.
- `docker/setup-buildx-action@v3`: habilita cache de layers e build moderno com BuildKit.
- `docker/build-push-action@v6`: build e push de imagem Docker sem scripts manuais extensos.
- `aws-actions/configure-aws-credentials@v4`: autenticacao com AWS via OIDC, sem access key hardcoded.
- `aws-actions/amazon-ecr-login@v2`: login padronizado e seguro no ECR.
- `permissions` minimas: por padrao `contents: read`; jobs AWS adicionam apenas `id-token: write`.
- `concurrency`: evita deploys simultaneos no mesmo ambiente.
- `environment`: integra aprovacao manual, auditoria e segregacao entre `development` e `production`.

## Fluxo da branch develop

Trigger:

- `push` em `develop`
- `pull_request` com destino para `develop`

Comportamento:

1. Faz checkout do codigo.
2. Instala o .NET 10 com cache de NuGet.
3. Executa restore da solucao e dos projetos de teste.
4. Compila a solucao em `Release` com `/warnaserror`.
5. Compila tambem os projetos de teste com `/warnaserror`.
6. Executa testes unitarios.
7. Executa testes de integracao.
8. Valida formatacao com `dotnet format --verify-no-changes`.
9. Em `push` para `develop`, autentica na AWS.
10. Faz build da imagem Docker com Buildx.
11. Publica duas tags no ECR:
12. `develop-latest`
13. `develop-<github.sha>`
14. Le a task definition atual.
15. Registra uma nova revisao com a imagem nova.
16. Forca novo deployment no ECS Service.
17. Aguarda estabilizacao do servico.
18. Publica um resumo com eventos recentes do ECS.

Observacao importante:

- Em `pull_request`, o workflow executa apenas a parte de CI.
- O deploy fica restrito a `push` em `develop`, o que evita publicar artefatos de branches temporarias ou forks.

## Fluxo da branch main

Trigger:

- `push` em `main`

Comportamento:

1. Faz checkout do codigo.
2. Instala o .NET 10 com cache de NuGet.
3. Executa restore da solucao e dos testes.
4. Faz build `Release` com warnings como erro.
5. Compila tambem os projetos de teste com `/warnaserror`.
6. Executa testes unitarios com coleta de cobertura.
7. Executa testes de integracao com coleta de cobertura.
8. Publica artefatos TRX e arquivos de cobertura.
9. Consolida a cobertura total e valida o limite minimo.
10. Valida formatacao com `dotnet format`.
11. Faz build e push da imagem Docker de producao.
12. Publica duas tags no ECR:
13. `latest`
14. `prod-<github.sha>`
15. Aguarda aprovacao do `GitHub Environment production`.
16. Atualiza a task definition do ECS.
17. Forca novo deployment do servico.
18. Aguarda estabilizacao do ECS.
19. Executa health check pos deploy.
20. Se o health check falhar, faz rollback automatico para a task definition anterior.
21. Publica logs resumidos do deploy.

## Estrategia de deploy

O deploy utiliza o padrao de `immutable image + mutable task definition`.

Fluxo tecnico:

1. O pipeline gera uma nova imagem Docker versionada por SHA.
2. A imagem e enviada ao ECR.
3. O workflow consulta a task definition atual no ECS.
4. O JSON da task definition e reprocessado com a nova imagem.
5. Uma nova revisao e registrada no ECS.
6. O ECS Service passa a apontar para a nova revisao.
7. O deploy usa `--force-new-deployment`.
8. O pipeline espera `services-stable` para confirmar estabilizacao.

Vantagens:

- versionamento rastreavel por commit
- rollback simples
- auditoria clara
- baixa chance de drift manual

## Estrategia de rollback

O rollback automatico foi implementado apenas no pipeline de `main`, pois producao exige protecao adicional.

Funcionamento:

1. Antes do deploy, o workflow armazena a task definition atual do servico.
2. A nova revisao e implantada.
3. Um health check HTTP e executado apos a estabilizacao do ECS.
4. Se o health check falhar, o workflow chama novo `update-service` apontando para a revisao anterior.
5. O pipeline aguarda a estabilizacao do rollback.
6. O job falha ao final para deixar o incidente visivel no GitHub Actions.

## Versionamento das imagens

Desenvolvimento:

- `develop-latest`
- `develop-<github.sha>`

Producao:

- `latest`
- `prod-<github.sha>`

Motivo da estrategia:

- tags `latest` e `develop-latest` facilitam consultas rapidas.
- tags com SHA garantem imutabilidade e rastreabilidade.
- a revisao implantada pode ser associada diretamente ao commit correspondente.

## Variaveis e secrets no GitHub

### Repository Variables

Configure em `Settings > Secrets and variables > Actions > Variables`:

- `AWS_REGION`
- `AWS_ACCOUNT_ID`
- `ECR_REPOSITORY`
- `ECS_CLUSTER_DEVELOP`
- `ECS_SERVICE_DEVELOP`
- `ECS_TASK_DEFINITION_DEVELOP`
- `ECS_CLUSTER_PRODUCTION`
- `ECS_SERVICE_PRODUCTION`
- `ECS_TASK_DEFINITION_PRODUCTION`
- `ECS_CONTAINER_NAME`
- `MINIMUM_COVERAGE`

Descricao recomendada:

- `AWS_REGION`: regiao AWS, por exemplo `us-east-1`
- `AWS_ACCOUNT_ID`: conta AWS dona do ECR/ECS
- `ECR_REPOSITORY`: nome do repositorio ECR, por exemplo `wex-purchases-api`
- `ECS_CLUSTER_DEVELOP`: nome do cluster ECS de desenvolvimento
- `ECS_SERVICE_DEVELOP`: nome do servico ECS de desenvolvimento
- `ECS_TASK_DEFINITION_DEVELOP`: family, ARN ou nome base da task definition de desenvolvimento
- `ECS_CLUSTER_PRODUCTION`: nome do cluster ECS de producao
- `ECS_SERVICE_PRODUCTION`: nome do servico ECS de producao
- `ECS_TASK_DEFINITION_PRODUCTION`: family, ARN ou nome base da task definition de producao
- `ECS_CONTAINER_NAME`: nome do container da API dentro da task definition
- `MINIMUM_COVERAGE`: percentual minimo de cobertura exigido em `main`, por exemplo `80`

`ECS_CONTAINER_NAME` e opcional se a task definition tiver apenas um container principal, mas e recomendado para evitar ambiguidade.

### Repository Secrets

Configure em `Settings > Secrets and variables > Actions > Secrets`:

- `AWS_ROLE_TO_ASSUME_DEVELOP`
- `AWS_ROLE_TO_ASSUME_PRODUCTION`
- `APP_HEALTHCHECK_URL`

Descricao recomendada:

- `AWS_ROLE_TO_ASSUME_DEVELOP`: ARN da role IAM assumida no deploy de `develop`
- `AWS_ROLE_TO_ASSUME_PRODUCTION`: ARN da role IAM assumida no deploy de `main`
- `APP_HEALTHCHECK_URL`: URL completa do health check de producao, por exemplo `https://api.seudominio.com/health`

Observacao:

- `APP_HEALTHCHECK_URL` foi colocado como secret para evitar exposicao desnecessaria de endpoint operacional.
- Se preferir, ele pode ser movido para `Environment Secret` do ambiente `production`.

## Configurando GitHub Environments

Crie dois environments em `Settings > Environments`:

- `development`
- `production`

### Environment development

Recomendacoes:

- sem aprovacao obrigatoria
- opcionalmente com regras de branch para `develop`
- usado para auditoria e segregacao do deploy de homologacao

### Environment production

Recomendacoes:

- habilitar `Required reviewers`
- restringir deploy apenas para `main`
- opcionalmente adicionar tempo de espera
- usar approvals antes da etapa de deploy

No workflow `ci-main.yml`, o job `deploy` usa `environment: production`. Isso faz o GitHub pausar a execucao ate a aprovacao dos revisores do environment.

## Como conectar com AWS ECS e ECR

### 1. Criar ou validar o repositorio ECR

Exemplo:

```bash
aws ecr create-repository --repository-name wex-purchases-api --region us-east-1
```

### 2. Criar a task definition no ECS

A task definition deve apontar para:

- compatibilidade com Fargate
- CPU e memoria compatíveis
- container da API
- log driver `awslogs`
- porta da aplicacao
- variaveis de ambiente e secrets

### 3. Criar ECS Cluster e ECS Service

O ECS Service deve estar configurado para:

- Fargate
- subnets privadas ou conforme sua arquitetura
- security groups corretos
- desired count
- load balancer, se houver

### 4. Configurar autenticacao OIDC do GitHub na AWS

Recomendacao enterprise:

- nao usar `AWS_ACCESS_KEY_ID` e `AWS_SECRET_ACCESS_KEY`
- usar `GitHub OIDC + IAM Role`

Passos:

1. Criar o provider OIDC do GitHub na conta AWS.
2. Criar uma IAM Role para `develop` e outra para `production`.
3. Permitir `sts:AssumeRoleWithWebIdentity`.
4. Restringir o trust policy para o repositorio e branch desejados.
5. Conceder apenas as permissoes necessarias para ECR e ECS.

Permissoes minimas tipicas da role:

- `ecr:GetAuthorizationToken`
- `ecr:BatchCheckLayerAvailability`
- `ecr:CompleteLayerUpload`
- `ecr:InitiateLayerUpload`
- `ecr:PutImage`
- `ecr:UploadLayerPart`
- `ecr:BatchGetImage`
- `ecs:DescribeServices`
- `ecs:DescribeTaskDefinition`
- `ecs:RegisterTaskDefinition`
- `ecs:UpdateService`
- `ecs:ListTasks`
- `iam:PassRole`

Dependendo da configuracao, voce tambem pode precisar de:

- `logs:DescribeLogStreams`
- `logs:GetLogEvents`

## Exemplo de trust policy OIDC

Ajuste `ACCOUNT_ID`, `ORG`, `REPO` e branch conforme seu caso:

```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Principal": {
        "Federated": "arn:aws:iam::ACCOUNT_ID:oidc-provider/token.actions.githubusercontent.com"
      },
      "Action": "sts:AssumeRoleWithWebIdentity",
      "Condition": {
        "StringEquals": {
          "token.actions.githubusercontent.com:aud": "sts.amazonaws.com"
        },
        "StringLike": {
          "token.actions.githubusercontent.com:sub": [
            "repo:ORG/REPO:ref:refs/heads/develop",
            "repo:ORG/REPO:ref:refs/heads/main"
          ]
        }
      }
    }
  ]
}
```

Para maior seguranca, use uma role por ambiente e restrinja cada uma a apenas uma branch.

## Explicacao tecnica de cada job

### `build`

- faz `restore` da aplicacao e dos projetos de teste
- compila em `Release`
- usa warnings como erro
- falha cedo quando a base nao esta consistente

### `tests`

- executa unitarios e integracao em paralelo com matrix
- usa `fail-fast`
- publica resultados em artefatos
- em `main`, tambem coleta cobertura

### `coverage`

- baixa os artefatos gerados pelos testes
- calcula cobertura total a partir dos arquivos Cobertura
- aplica o threshold configurado em `MINIMUM_COVERAGE`

### `format`

- garante padrao de formatacao antes do build de imagem
- protege a qualidade e reduz diffs desnecessarios

### `package`

- autentica na AWS
- faz login no ECR
- builda a imagem com Buildx
- publica tags rastreaveis

### `deploy`

- consulta a task definition atual
- cria nova revisao com a imagem publicada
- atualiza o ECS Service
- aguarda estabilizacao
- registra resumo do deploy
- em producao, ainda executa health check e rollback se necessario

## Troubleshooting comum

### 1. Falha no `configure-aws-credentials`

Possiveis causas:

- role ARN incorreta
- trust policy OIDC incompleta
- branch nao permitida no `sub`
- falta de `id-token: write`

Validacoes:

- confirme os secrets `AWS_ROLE_TO_ASSUME_DEVELOP` e `AWS_ROLE_TO_ASSUME_PRODUCTION`
- revise a trust policy
- confira se o workflow esta rodando na branch esperada

### 2. Falha no push para o ECR

Possiveis causas:

- repositorio inexistente
- role sem permissoes de `PutImage`
- `AWS_REGION` ou `AWS_ACCOUNT_ID` incorretos

Validacoes:

- verifique a existencia do repositorio ECR
- confira se `ECR_REPOSITORY` bate com o nome real
- revise a policy IAM da role

### 3. Falha ao registrar task definition

Possiveis causas:

- role sem `ecs:RegisterTaskDefinition`
- role sem `iam:PassRole`
- `ECS_TASK_DEFINITION_*` apontando para family errada
- `ECS_CONTAINER_NAME` divergente do container real

Validacoes:

- confira o nome do container na task definition
- confirme se a role consegue passar `executionRoleArn` e `taskRoleArn`

### 4. ECS nao estabiliza

Possiveis causas:

- health check do target group falhando
- variaveis de ambiente ausentes
- imagem com erro de startup
- task sem acesso a banco, Redis ou secrets

Validacoes:

- verifique eventos do ECS no resumo do workflow
- consulte logs do CloudWatch
- revise configuracao de rede, security groups e roles

### 5. Health check pos deploy falha em producao

Possiveis causas:

- endpoint `/health` indisponivel externamente
- ALB ainda propagando target healthy
- aplicacao subiu, mas nao ficou operacional

Validacoes:

- confira o valor de `APP_HEALTHCHECK_URL`
- valide se a URL responde de fora do cluster
- revise readiness e health checks do ALB

## Observacoes finais

- Os workflows foram mantidos em arquivos separados para facilitar governanca por branch.
- O deploy de `main` foi endurecido com approval, coverage gate e rollback automatico.
- O deploy de `develop` foi mantido mais agil, mas ainda com controles de qualidade, rastreabilidade e estabilidade.
- Toda a estrategia evita dependencias locais manuais: o runner instala o .NET e usa cache automaticamente.
