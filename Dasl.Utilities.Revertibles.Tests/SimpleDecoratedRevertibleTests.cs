
# pragma warning disable CA1707 // warning for underscores in method names

namespace Dasl.Utilities.Revertibles.Tests;


/// <summary>
/// Tests for exploring the DX of composing Revertible
/// </summary>
public class SimpleDecoratedRevertibleTests : IDisposable
{
    public SimpleDecoratedRevertibleTests()
    {
    }

    [Fact]
    public async Task HasChanged_GivenNoChanges_ShouldReturnFalse()
    {
        var revertible = new SimpleDecoratedRevertible();

        Assert.False(revertible.HasChanged("Id"));
    }

    [Fact]
    public async Task HasChanged_GivenChanges_ShouldReturnTrue()
    {
        var revertible = new SimpleDecoratedRevertible();

        revertible.Id = 1;

        Assert.True(revertible.HasChanged("Id"));
    }

    [Fact]
    public async Task HasChanged_GivenAcceptedChanges_ShouldReturnFalse()
    {
        var revertible = new SimpleDecoratedRevertible();
        revertible.Id = 1;

        revertible.AcceptChanges();

        Assert.False(revertible.HasChanged("Id"));
    }

    [Fact]
    public async Task Revert_GivenChanges_ShouldRevertTheChanges()
    {
        var revertible = new SimpleDecoratedRevertible();

        revertible.Id = 1;

        revertible.Revert("Id");

        Assert.False(revertible.HasChanged("Id"));
    }

    public void Dispose()
    {
    }
}
