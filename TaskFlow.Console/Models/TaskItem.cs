namespace TaskFlow.Models;

/// <summary>
/// Entidade de domínio que representa uma tarefa (item de To-Do List)
/// gerenciada pela aplicação TaskFlow.
/// </summary>
public class TaskItem
{
    /// <summary>Identificador único e sequencial da tarefa.</summary>
    public int Id { get; }

    /// <summary>Título (descrição curta) da tarefa. Nunca é nulo ou vazio.</summary>
    public string Title { get; }

    /// <summary>Indica se a tarefa já foi concluída.</summary>
    public bool IsCompleted { get; private set; }

    /// <summary>Momento em que a tarefa foi criada.</summary>
    public DateTime CreatedAt { get; }

    /// <summary>Momento em que a tarefa foi concluída (nulo enquanto pendente).</summary>
    public DateTime? CompletedAt { get; private set; }

    /// <summary>
    /// Cria uma nova tarefa, sempre no estado "pendente".
    /// </summary>
    /// <param name="id">Identificador único da tarefa.</param>
    /// <param name="title">Título da tarefa (obrigatório).</param>
    /// <exception cref="ArgumentException">
    /// Lançada quando <paramref name="title"/> é nulo, vazio ou composto apenas por espaços.
    /// </exception>
    public TaskItem(int id, string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException(
                "O título da tarefa não pode ser nulo, vazio ou apenas espaços em branco.",
                nameof(title));
        }

        Id = id;
        Title = title.Trim();
        IsCompleted = false;
        CreatedAt = DateTime.Now;
    }

    /// <summary>Marca a tarefa como concluída.</summary>
    public void MarkAsCompleted()
    {
        IsCompleted = true;
        CompletedAt = DateTime.Now;
    }

    /// <summary>Reabre a tarefa, voltando-a para o estado pendente.</summary>
    public void MarkAsPending()
    {
        IsCompleted = false;
        CompletedAt = null;
    }

    /// <summary>
    /// Lógica básica de alternância: concluída vira pendente e pendente vira concluída.
    /// </summary>
    public void ToggleCompletion()
    {
        if (IsCompleted)
        {
            MarkAsPending();
        }
        else
        {
            MarkAsCompleted();
        }
    }

    /// <summary>Representação textual usada na listagem do console.</summary>
    public override string ToString()
    {
        var status = IsCompleted ? "[x]" : "[ ]";
        return $"{status} #{Id} - {Title}";
    }
}
