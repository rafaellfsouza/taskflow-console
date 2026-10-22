# TaskFlow — Gerenciador de Tarefas via Console

Aplicação **console** em **.NET 10 / C#** para gerenciamento de uma lista de tarefas
(*To-Do List*), desenvolvida com foco em **Garantia da Qualidade de Software**:
código organizado em camadas, testes unitários em **xUnit** e histórico de commits
seguindo o padrão **Conventional Commits**.

---

## Sumário

- [Tecnologias](#tecnologias)
- [Estrutura do projeto](#estrutura-do-projeto)
- [Pré-requisitos](#pré-requisitos)
- [Como obter o código](#como-obter-o-código)
- [Como compilar (build)](#como-compilar-build)
- [Como rodar a aplicação console](#como-rodar-a-aplicação-console)
- [Como executar os testes unitários](#como-executar-os-testes-unitários)
- [Funcionalidades e menu](#funcionalidades-e-menu)
- [Arquitetura e regras de negócio](#arquitetura-e-regras-de-negócio)
- [Histórico de commits (Conventional Commits)](#histórico-de-commits-conventional-commits)
- [Solução de problemas](#solução-de-problemas)

---

## Tecnologias

| Item                | Versão / Tecnologia                     |
| ------------------- | --------------------------------------- |
| Runtime / SDK       | .NET 10 (`net10.0`)                     |
| Linguagem           | C# 14                                   |
| Tipo de aplicação   | Console App                             |
| Testes unitários    | xUnit 2.9.3                             |
| Runner de testes    | Microsoft.NET.Test.Sdk 17.14.1          |
| Cobertura de código | coverlet.collector 6.0.4                |
| Controle de versão  | Git + Conventional Commits              |

## Estrutura do projeto

```text
taskflow-console/
├── TaskFlow.sln                          # Solution (formato .sln clássico)
├── README.md
├── .gitignore
│
├── TaskFlow.Console/                     # Projeto Console (aplicação)
│   ├── TaskFlow.Console.csproj
│   ├── Program.cs                        # Camada de apresentação (menu interativo)
│   ├── Models/
│   │   └── TaskItem.cs                   # Entidade de domínio
│   └── Services/
│       └── TaskManager.cs                # Serviço de gerenciamento em memória
│
└── TaskFlow.Tests/                       # Projeto de testes unitários (xUnit)
    ├── TaskFlow.Tests.csproj
    └── Services/
        ├── TaskManagerTests.cs           # Regras de adição e validação de título
        └── TaskManagerGetByIdTests.cs    # Regressão da busca por Id
```

> **Observação técnica:** o `TaskFlow.Console.csproj` define
> `<RootNamespace>TaskFlow</RootNamespace>`. Isso é necessário porque o nome do
> projeto termina em `.Console`, o que faria o namespace padrão (`TaskFlow.Console`)
> entrar em conflito com a classe `System.Console` (erro **CS0118** ao usar
> `Console.WriteLine`). Com o *root namespace* `TaskFlow`, os namespaces usados são
> `TaskFlow.Models` e `TaskFlow.Services`.

## Pré-requisitos

- **.NET 10 SDK** instalado — [download oficial](https://dotnet.microsoft.com/download/dotnet/10.0)
- **Git** — [download oficial](https://git-scm.com/downloads)

Verifique a instalação:

```bash
dotnet --version    # esperado: 10.0.xxx
git --version
```

## Como obter o código

```bash
git clone https://github.com/rafaellfsouza/taskflow-console.git
cd taskflow-console
```

## Como compilar (build)

Compila a solution inteira (aplicação + testes):

```bash
dotnet build TaskFlow.sln
```

Compila em modo *Release* (otimizado):

```bash
dotnet build TaskFlow.sln -c Release
```

Restaurar apenas as dependências (NuGet), sem compilar:

```bash
dotnet restore TaskFlow.sln
```

Saída esperada:

```text
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

## Como rodar a aplicação console

```bash
dotnet run --project TaskFlow.Console
```

Alternativas equivalentes:

```bash
# entrar na pasta do projeto e executar
cd TaskFlow.Console
dotnet run

# executar o binário já compilado (Release)
dotnet build TaskFlow.sln -c Release
dotnet TaskFlow.Console/bin/Release/net10.0/TaskFlow.Console.dll

# gerar executável nativo da plataforma e rodar direto
dotnet publish TaskFlow.Console -c Release -o ./publish
./publish/TaskFlow.Console.exe        # Windows
./publish/TaskFlow.Console            # Linux / macOS
```

> As tarefas são mantidas **em memória**: ao encerrar a aplicação (opção `0`),
> a lista é descartada.

## Como executar os testes unitários

Executar todos os testes da solution:

```bash
dotnet test TaskFlow.sln
```

Com detalhamento de cada teste executado:

```bash
dotnet test TaskFlow.sln --logger "console;verbosity=detailed"
```

Gerar relatório de cobertura de código (formato *cobertura*):

```bash
dotnet test TaskFlow.sln --collect:"XPlat Code Coverage" --results-directory ./TestResults
```

Filtrar testes por nome:

```bash
dotnet test TaskFlow.sln --filter "FullyQualifiedName~TaskManagerTests"
```

Saída esperada:

```text
Passed!  - Failed: 0, Passed: 25, Skipped: 0, Total: 25 - TaskFlow.Tests.dll (net10.0)
```

## Funcionalidades e menu

Ao executar a aplicação, o menu interativo abaixo é exibido em *loop* até a opção `0`:

```text
╔══════════════════════════════════════════════╗
║                   TaskFlow                   ║
║      Gerenciador de Tarefas via Console      ║
╚══════════════════════════════════════════════╝
  Total: 2  |  Pendentes: 1  |  Concluídas: 1
┌────────────────── M E N U ──────────────────┐
  1. Adicionar tarefa
  2. Listar todas as tarefas
  3. Listar tarefas pendentes
  4. Listar tarefas concluídas
  5. Concluir tarefa
  6. Alternar status da tarefa
  7. Buscar tarefa por Id
  8. Remover tarefa
  0. Sair
└─────────────────────────────────────────────┘
```

Exemplo de listagem:

```text
── Todas as tarefas (2) ──

[x] #1   Estudar para a prova de GQS  (concluída em 19/10/2026 23:05)
[ ] #2   Entregar a lista de exercícios
```

Comandos atalho úteis:

| Atalho | Comando equivalente                     |
| ------ | --------------------------------------- |
| —      | `dotnet run --project TaskFlow.Console` |
| —      | `dotnet build TaskFlow.sln`             |
| —      | `dotnet test TaskFlow.sln`              |

## Arquitetura e regras de negócio

O projeto segue separação de responsabilidades em três camadas:

| Camada          | Arquivo                          | Responsabilidade                                                        |
| --------------- | -------------------------------- | ----------------------------------------------------------------------- |
| Domínio         | `Models/TaskItem.cs`             | Estado e comportamento da tarefa (`Id`, `Title`, `IsCompleted`, `ToggleCompletion`) |
| Serviço         | `Services/TaskManager.cs`        | Lista em memória: adicionar, listar, buscar, concluir, alternar e remover |
| Apresentação    | `Program.cs`                     | Menu interativo, leitura/validação de entrada e formatação da saída      |

### Entidade `TaskItem`

- `Id` (`int`) — identificador sequencial atribuído pelo `TaskManager`;
- `Title` (`string`) — obrigatório; é aplicado `Trim()` no valor recebido;
- `IsCompleted` (`bool`) — nasce sempre `false` (tarefa pendente);
- `CreatedAt` (`DateTime`) e `CompletedAt` (`DateTime?`) — auditoria simples;
- `MarkAsCompleted()`, `MarkAsPending()` e `ToggleCompletion()` — alternância de status.

### Regras de negócio validadas pelos testes

1. **Adição correta:** títulos válidos criam tarefas pendentes, com Id sequencial e único, armazenadas na lista.
2. **Título inválido:** `null`, `""`, `" "` ou somente espaços/tabulação/quebra de linha lançam `ArgumentException`
   (`ParamName = "title"`), **nenhuma** tarefa é armazenada e o próximo Id **não** é consumido.
3. **Busca por Id inexistente:** `GetTaskById(int id)` retorna `TaskItem?` (`null` quando não encontra),
   em vez de lançar `KeyNotFoundException`. `Complete`, `Toggle` e `Remove` retornam `false` nesse cenário.
4. **Listagens:** `List()`, `ListPending()` e `ListCompleted()` retornam as tarefas ordenadas por `Id`.

## Histórico de commits (Conventional Commits)

Todo o histórico segue o padrão `tipo(escopo): descrição curta`:

```text
docs(readme): add build, run and test instructions
refactor(console): implement interactive menu loop in Program.cs
fix(manager): handle missing task gracefully in search method
test(unit): add xUnit test suite for TaskManager validation rules
feat(services): implement TaskManager logic for in-memory task management
feat(domain): implement TaskItem entity with completion logic
chore(test): add xUnit test project and reference to console Project
chore(core): initialize .NET 10 solution and console project structure
```

Para visualizar o grafo de commits:

```bash
git log --oneline --graph
```

Tipos utilizados nesta lista: `chore` (estrutura/configuração), `feat` (nova funcionalidade),
`test` (testes), `fix` (correção de defeito), `refactor` (refatoração sem mudança de comportamento)
e `docs` (documentação).

## Solução de problemas

| Problema                                                              | Solução                                                                                                     |
| --------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------- |
| `The current .NET SDK does not support targeting .NET 10.0`            | Instale/atualize o **.NET 10 SDK** e rode `dotnet --list-sdks`                                                |
| Erro **CS0118** (`'Console' is a namespace but is used like a type`)  | Confirme `<RootNamespace>TaskFlow</RootNamespace>` no `TaskFlow.Console.csproj`                              |
| `dotnet new sln` gerou `TaskFlow.slnx` em vez de `TaskFlow.sln`       | Use `dotnet new sln -n TaskFlow --format sln` (o .NET 10 passou a gerar `.slnx` por padrão)                  |
| Caracteres acentuados aparecem errados no terminal                    | Windows: `chcp 65001` antes de executar; ou use o **Windows Terminal**                                       |
| Testes não aparecem no Visual Studio                                  | `dotnet restore TaskFlow.sln` e reabra a solution; confirme o pacote `xunit.runner.visualstudio`             |

---

Projeto desenvolvido para a disciplina **Garantia da Qualidade de Software** —
*Gestão e Qualidade de Software*.
