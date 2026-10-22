using TaskFlow.Services;
using Xunit;

namespace TaskFlow.Tests.Services;

/// <summary>
/// Testes de regressão do método de busca por Id do <see cref="TaskManager"/>.
/// Cobrem o defeito em que a busca por um Id inexistente lançava
/// <see cref="KeyNotFoundException"/> sem tratamento.
/// </summary>
public class TaskManagerGetByIdTests
{
    private readonly TaskManager _manager = new();

    [Fact]
    public void GetTaskById_WithExistingId_ShouldReturnTheTask()
    {
        // Arrange
        var created = _manager.Add("Tarefa existente");

        // Act
        var found = _manager.GetTaskById(created.Id);

        // Assert
        Assert.NotNull(found);
        Assert.Equal(created.Id, found.Id);
        Assert.Equal("Tarefa existente", found.Title);
    }

    [Fact]
    public void GetTaskById_WithNonExistingId_ShouldReturnNullInsteadOfThrowing()
    {
        // Arrange
        _manager.Add("Única tarefa da lista");

        // Act
        var found = _manager.GetTaskById(999);

        // Assert
        Assert.Null(found);
    }

    [Fact]
    public void GetTaskById_WithEmptyList_ShouldReturnNull()
    {
        // Act
        var found = _manager.GetTaskById(1);

        // Assert
        Assert.Null(found);
    }

    [Fact]
    public void GetTaskById_WithRemovedTaskId_ShouldReturnNull()
    {
        // Arrange
        var task = _manager.Add("Tarefa que será removida");
        _manager.Remove(task.Id);

        // Act
        var found = _manager.GetTaskById(task.Id);

        // Assert
        Assert.Null(found);
    }

    [Fact]
    public void Complete_WithNonExistingId_ShouldReturnFalseInsteadOfThrowing()
    {
        // Act
        var result = _manager.Complete(42);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Toggle_WithNonExistingId_ShouldReturnFalseInsteadOfThrowing()
    {
        // Act
        var result = _manager.Toggle(42);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Toggle_WithExistingId_ShouldSwitchCompletionStatus()
    {
        // Arrange
        var task = _manager.Add("Alternar status");

        // Act
        _manager.Toggle(task.Id);
        var completedAfterFirstToggle = task.IsCompleted;
        _manager.Toggle(task.Id);

        // Assert
        Assert.True(completedAfterFirstToggle);
        Assert.False(task.IsCompleted);
    }
}
