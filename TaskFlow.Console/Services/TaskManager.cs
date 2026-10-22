using TaskFlow.Models;

namespace TaskFlow.Services;

/// <summary>
/// Serviço responsável por gerenciar a lista de tarefas em memória
/// (adicionar, listar, buscar, concluir e remover tarefas).
/// </summary>
public class TaskManager
{
    private readonly Dictionary<int, TaskItem> _tasks = new();
    private int _nextId = 1;

    /// <summary>Quantidade total de tarefas armazenadas.</summary>
    public int Count => _tasks.Count;

    /// <summary>Quantidade de tarefas ainda pendentes.</summary>
    public int PendingCount => _tasks.Values.Count(t => !t.IsCompleted);

    /// <summary>Quantidade de tarefas concluídas.</summary>
    public int CompletedCount => _tasks.Values.Count(t => t.IsCompleted);

    /// <summary>
    /// Adiciona uma nova tarefa à lista.
    /// </summary>
    /// <param name="title">Título obrigatório da tarefa.</param>
    /// <returns>A tarefa recém-criada.</returns>
    /// <exception cref="ArgumentException">
    /// Lançada quando <paramref name="title"/> é nulo, vazio ou apenas espaços em branco.
    /// </exception>
    public TaskItem Add(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException(
                "Não é possível adicionar uma tarefa sem título.", nameof(title));
        }

        var task = new TaskItem(_nextId++, title);
        _tasks.Add(task.Id, task);

        return task;
    }

    /// <summary>Lista todas as tarefas, ordenadas pelo Id.</summary>
    public IReadOnlyList<TaskItem> List()
    {
        return _tasks.Values.OrderBy(t => t.Id).ToList();
    }

    /// <summary>Lista apenas as tarefas pendentes.</summary>
    public IReadOnlyList<TaskItem> ListPending()
    {
        return _tasks.Values.Where(t => !t.IsCompleted).OrderBy(t => t.Id).ToList();
    }

    /// <summary>Lista apenas as tarefas concluídas.</summary>
    public IReadOnlyList<TaskItem> ListCompleted()
    {
        return _tasks.Values.Where(t => t.IsCompleted).OrderBy(t => t.Id).ToList();
    }

    /// <summary>
    /// Busca uma tarefa pelo Id de forma segura.
    /// </summary>
    /// <param name="id">Identificador da tarefa.</param>
    /// <returns>
    /// A tarefa encontrada ou <c>null</c> quando não existe nenhuma tarefa com o Id informado.
    /// </returns>
    /// <remarks>
    /// Antes usava o indexador do dicionário (<c>_tasks[id]</c>), que lançava
    /// <see cref="KeyNotFoundException"/> para Ids inexistentes. Agora usa
    /// <see cref="Dictionary{TKey,TValue}.TryGetValue"/> e devolve um tipo anulável
    /// (<c>TaskItem?</c>), tratando a ausência da tarefa com elegância.
    /// </remarks>
    public TaskItem? GetTaskById(int id)
    {
        return _tasks.TryGetValue(id, out var task) ? task : null;
    }

    /// <summary>Marca a tarefa informada como concluída.</summary>
    /// <returns><c>true</c> se a tarefa foi concluída; <c>false</c> se ela não existe.</returns>
    public bool Complete(int id)
    {
        var task = GetTaskById(id);

        if (task is null)
        {
            return false;
        }

        task.MarkAsCompleted();

        return true;
    }

    /// <summary>Alterna o status (pendente/concluída) da tarefa informada.</summary>
    /// <returns><c>true</c> se o status foi alternado; <c>false</c> se ela não existe.</returns>
    public bool Toggle(int id)
    {
        var task = GetTaskById(id);

        if (task is null)
        {
            return false;
        }

        task.ToggleCompletion();

        return true;
    }

    /// <summary>Remove a tarefa informada da lista.</summary>
    /// <returns><c>true</c> se a tarefa existia e foi removida; caso contrário, <c>false</c>.</returns>
    public bool Remove(int id)
    {
        return _tasks.Remove(id);
    }
}
