using TaskFlow.Services;
using Xunit;

namespace TaskFlow.Tests.Services;

/// <summary>
/// Suíte de testes unitários do <see cref="TaskManager"/>,
/// focada nas regras de validação de adição de tarefas.
/// </summary>
public class TaskManagerTests
{
    private readonly TaskManager _manager = new();

    // ------------------------------------------------------------------
    // Regra: o TaskManager deve adicionar tarefas corretamente
    // ------------------------------------------------------------------

    [Fact]
    public void Add_WithValidTitle_ShouldReturnTaskWithSameTitle()
    {
        // Arrange
        const string title = "Estudar para a prova de GQS";

        // Act
        var task = _manager.Add(title);

        // Assert
        Assert.NotNull(task);
        Assert.Equal(title, task.Title);
    }

    [Fact]
    public void Add_WithValidTitle_ShouldStoreTaskInList()
    {
        // Act
        _manager.Add("Primeira tarefa");

        // Assert
        var tasks = _manager.List();

        Assert.Single(tasks);
        Assert.Equal(1, _manager.Count);
        Assert.Contains(tasks, t => t.Title == "Primeira tarefa");
    }

    [Fact]
    public void Add_WithValidTitle_ShouldCreateTaskAsPending()
    {
        // Act
        var task = _manager.Add("Tarefa pendente");

        // Assert
        Assert.False(task.IsCompleted, "Uma tarefa recém-criada deve nascer pendente.");
        Assert.Null(task.CompletedAt);
    }

    [Fact]
    public void Add_CalledMultipleTimes_ShouldGenerateSequentialUniqueIds()
    {
        // Act
        var first = _manager.Add("Tarefa 1");
        var second = _manager.Add("Tarefa 2");
        var third = _manager.Add("Tarefa 3");

        // Assert
        Assert.Equal(1, first.Id);
        Assert.Equal(2, second.Id);
        Assert.Equal(3, third.Id);
        Assert.Equal(3, _manager.Count);
    }

    [Fact]
    public void Add_WithTitleSurroundedBySpaces_ShouldTrimTitle()
    {
        // Act
        var task = _manager.Add("   tarefa com espaços   ");

        // Assert
        Assert.Equal("tarefa com espaços", task.Title);
    }

    [Fact]
    public void List_ShouldReturnAllTasksOrderedById()
    {
        // Arrange
        _manager.Add("Comprar pão");
        _manager.Add("Pagar boleto");
        _manager.Add("Ligar para o cliente");

        // Act
        var ids = _manager.List().Select(t => t.Id).ToArray();

        // Assert
        Assert.Equal(new[] { 1, 2, 3 }, ids);
    }

    [Fact]
    public void Complete_WithExistingId_ShouldMarkTaskAsCompleted()
    {
        // Arrange
        var task = _manager.Add("Finalizar relatório");

        // Act
        var result = _manager.Complete(task.Id);

        // Assert
        Assert.True(result);
        Assert.True(task.IsCompleted);
        Assert.Equal(1, _manager.CompletedCount);
        Assert.Equal(0, _manager.PendingCount);
    }

    [Fact]
    public void Remove_WithExistingId_ShouldDeleteTaskFromList()
    {
        // Arrange
        var task = _manager.Add("Tarefa temporária");

        // Act
        var removed = _manager.Remove(task.Id);

        // Assert
        Assert.True(removed);
        Assert.Empty(_manager.List());
        Assert.Equal(0, _manager.Count);
    }

    // ------------------------------------------------------------------
    // Regra: impedir a adição de tarefas com títulos vazios ou nulos
    // ------------------------------------------------------------------

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("        ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Add_WithEmptyOrNullOrWhitespaceTitle_ShouldThrowArgumentException(string? title)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _manager.Add(title));

        Assert.Equal("title", exception.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Add_WithInvalidTitle_ShouldNotStoreAnyTask(string? title)
    {
        // Act
        Assert.Throws<ArgumentException>(() => _manager.Add(title));

        // Assert
        Assert.Equal(0, _manager.Count);
        Assert.Empty(_manager.List());
    }

    [Fact]
    public void Add_WithInvalidTitle_ShouldNotConsumeNextId()
    {
        // Arrange
        Assert.Throws<ArgumentException>(() => _manager.Add(""));

        // Act
        var task = _manager.Add("Tarefa válida");

        // Assert
        Assert.Equal(1, task.Id);
    }
}
