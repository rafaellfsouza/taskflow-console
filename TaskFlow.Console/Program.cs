using TaskFlow.Models;
using TaskFlow.Services;

// ============================================================================
//  TaskFlow — Gerenciador de Tarefas via Console
// ----------------------------------------------------------------------------
//  Camada de apresentação: cuida apenas de entrada/saída e da navegação do
//  usuário. Toda a regra de negócio vive em TaskItem (domínio) e TaskManager
//  (serviço), o que mantém este arquivo testável e livre de lógica duplicada.
// ============================================================================

const string AppName = "TaskFlow";

var manager = new TaskManager();
var running = true;

WriteHeader();

while (running)
{
    WriteMenu();

    switch (ReadOption())
    {
        case 1:
            HandleAdd();
            break;
        case 2:
            HandleListAll();
            break;
        case 3:
            HandleListPending();
            break;
        case 4:
            HandleListCompleted();
            break;
        case 5:
            HandleComplete();
            break;
        case 6:
            HandleToggle();
            break;
        case 7:
            HandleSearch();
            break;
        case 8:
            HandleRemove();
            break;
        case 0:
            running = false;
            break;
        default:
            WriteError("Opção inválida. Digite um número entre 0 e 8.");
            break;
    }

    if (running)
    {
        WaitToContinue();
    }
}

WriteFooter();

// ============================================================================
//  Ações do menu
// ============================================================================

void HandleAdd()
{
    WriteSection("Adicionar tarefa");
    Console.Write("Título da tarefa: ");

    var title = Console.ReadLine();

    try
    {
        var task = manager.Add(title);
        WriteSuccess($"Tarefa #{task.Id} adicionada com sucesso: \"{task.Title}\".");
    }
    catch (ArgumentException ex)
    {
        // Regra de validação: títulos nulos, vazios ou só com espaços são rejeitados.
        WriteError(ex.Message);
    }
}

void HandleListAll()
{
    WriteSection($"Todas as tarefas ({manager.Count})");
    WriteTaskList(manager.List());
}

void HandleListPending()
{
    WriteSection($"Tarefas pendentes ({manager.PendingCount})");
    WriteTaskList(manager.ListPending());
}

void HandleListCompleted()
{
    WriteSection($"Tarefas concluídas ({manager.CompletedCount})");
    WriteTaskList(manager.ListCompleted());
}

void HandleComplete()
{
    WriteSection("Concluir tarefa");

    if (!TryReadTaskId(out var id))
    {
        return;
    }

    if (manager.Complete(id))
    {
        WriteSuccess($"Tarefa #{id} marcada como concluída.");
    }
    else
    {
        WriteNotFound(id);
    }
}

void HandleToggle()
{
    WriteSection("Alternar status da tarefa");

    if (!TryReadTaskId(out var id))
    {
        return;
    }

    var task = manager.GetTaskById(id);

    if (manager.Toggle(id) && task is not null)
    {
        WriteSuccess($"Tarefa #{id} agora está {(task.IsCompleted ? "CONCLUÍDA" : "PENDENTE")}.");
    }
    else
    {
        WriteNotFound(id);
    }
}

void HandleSearch()
{
    WriteSection("Buscar tarefa por Id");

    if (!TryReadTaskId(out var id))
    {
        return;
    }

    var task = manager.GetTaskById(id);

    if (task is null)
    {
        WriteNotFound(id);
        return;
    }

    WriteTaskDetail(task);
}

void HandleRemove()
{
    WriteSection("Remover tarefa");

    if (!TryReadTaskId(out var id))
    {
        return;
    }

    if (manager.Remove(id))
    {
        WriteSuccess($"Tarefa #{id} removida da lista.");
    }
    else
    {
        WriteNotFound(id);
    }
}

// ============================================================================
//  Auxiliares de entrada
// ============================================================================

/// <summary>Lê a opção do menu; devolve -1 quando a entrada não é numérica.</summary>
int ReadOption()
{
    Console.Write("\n> Escolha uma opção: ");

    var input = Console.ReadLine();

    return int.TryParse(input?.Trim(), out var option) ? option : -1;
}

/// <summary>
/// Pede e valida o Id de uma tarefa.
/// </summary>
/// <returns><c>true</c> se um Id numérico válido foi informado.</returns>
bool TryReadTaskId(out int id)
{
    id = 0;

    if (manager.Count == 0)
    {
        WriteError("A lista está vazia. Cadastre uma tarefa primeiro (opção 1).");
        return false;
    }

    Console.Write("Id da tarefa: ");

    var input = Console.ReadLine();

    if (!int.TryParse(input?.Trim(), out id) || id <= 0)
    {
        WriteError("Id inválido. Informe um número inteiro maior que zero.");
        return false;
    }

    return true;
}

// ============================================================================
//  Auxiliares de saída
// ============================================================================

void WriteHeader()
{
    ClearScreen();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("╔══════════════════════════════════════════════╗");
    Console.WriteLine($"║{Center(AppName, 46)}║");
    Console.WriteLine($"║{Center("Gerenciador de Tarefas via Console", 46)}║");
    Console.WriteLine("╚══════════════════════════════════════════════╝");
    Console.ResetColor();
}

void WriteMenu()
{
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine(
        $"  Total: {manager.Count}  |  Pendentes: {manager.PendingCount}  |  Concluídas: {manager.CompletedCount}");
    Console.ResetColor();
    Console.WriteLine("┌────────────────── M E N U ──────────────────┐");
    Console.WriteLine("  1. Adicionar tarefa");
    Console.WriteLine("  2. Listar todas as tarefas");
    Console.WriteLine("  3. Listar tarefas pendentes");
    Console.WriteLine("  4. Listar tarefas concluídas");
    Console.WriteLine("  5. Concluir tarefa");
    Console.WriteLine("  6. Alternar status da tarefa");
    Console.WriteLine("  7. Buscar tarefa por Id");
    Console.WriteLine("  8. Remover tarefa");
    Console.WriteLine("  0. Sair");
    Console.WriteLine("└─────────────────────────────────────────────┘");
}

void WriteSection(string title)
{
    ClearScreen();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"── {title} ──");
    Console.ResetColor();
    Console.WriteLine();
}

void WriteTaskList(IReadOnlyList<TaskItem> tasks)
{
    if (tasks.Count == 0)
    {
        WriteWarning("Nenhuma tarefa para exibir.");
        return;
    }

    foreach (var task in tasks)
    {
        WriteTaskLine(task);
    }
}

void WriteTaskLine(TaskItem task)
{
    var marker = task.IsCompleted ? "[x]" : "[ ]";

    Console.ForegroundColor = task.IsCompleted ? ConsoleColor.Green : ConsoleColor.Yellow;
    Console.Write(marker);
    Console.ResetColor();
    Console.Write($" #{task.Id,-3} ");

    if (task.IsCompleted)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
    }

    Console.Write(task.Title);
    Console.ResetColor();

    if (task.IsCompleted && task.CompletedAt is { } completedAt)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"  (concluída em {completedAt:dd/MM/yyyy HH:mm})");
        Console.ResetColor();
    }

    Console.WriteLine();
}

void WriteTaskDetail(TaskItem task)
{
    Console.WriteLine($"  Id ............: {task.Id}");
    Console.WriteLine($"  Título ........: {task.Title}");
    Console.WriteLine($"  Situação ......: {(task.IsCompleted ? "Concluída" : "Pendente")}");
    Console.WriteLine($"  Criada em .....: {task.CreatedAt:dd/MM/yyyy HH:mm}");
    Console.WriteLine(
        $"  Concluída em ..: {(task.CompletedAt is { } at ? at.ToString("dd/MM/yyyy HH:mm") : "nao concluida")}");
}

void WriteSuccess(string message)
{
    WriteColored(message, ConsoleColor.Green);
}

void WriteError(string message)
{
    WriteColored($"ERRO: {message}", ConsoleColor.Red);
}

void WriteWarning(string message)
{
    WriteColored($"! {message}", ConsoleColor.Yellow);
}

void WriteNotFound(int id)
{
    WriteError($"Não existe tarefa com o Id {id}.");
}

void WriteColored(string message, ConsoleColor color)
{
    Console.ForegroundColor = color;
    Console.WriteLine(message);
    Console.ResetColor();
}

void WaitToContinue()
{
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write("Pressione ENTER para voltar ao menu...");
    Console.ResetColor();
    Console.ReadLine();
}

void WriteFooter()
{
    ClearScreen();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("Obrigado por usar o TaskFlow. Até a próxima!");
    Console.ResetColor();
}

/// <summary>Centraliza um texto dentro de uma largura fixa.</summary>
string Center(string text, int width)
{
    var padding = Math.Max(0, width - text.Length);
    var left = padding / 2;

    return new string(' ', left) + text + new string(' ', padding - left);
}

/// <summary>Limpa a tela apenas quando a saída é um console real (evita erro em I/O redirecionado).</summary>
void ClearScreen()
{
    if (!Console.IsOutputRedirected)
    {
        Console.Clear();
    }
}
