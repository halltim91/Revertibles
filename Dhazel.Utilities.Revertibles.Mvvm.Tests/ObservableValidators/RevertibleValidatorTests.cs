
using Dhazel.Utilities.Revertibles.Mvvm.ObservableValidators;

# pragma warning disable CA1707 // warning for underscores in method names

namespace Dhazel.Utilities.Revertibles.Mvvm.Tests.ObservableValidators;


/// <summary>
/// Tests for exploring the DX of composing Revertible
/// </summary>
public class RevertibleValidatorTests : IDisposable
{
    public RevertibleValidatorTests()
    {
    }

    [Fact]
    public async Task HasChanged_GivenNoChanges_ShouldReturnFalse()
    {
        var revertible = new MyRevertibleValidator();

        Assert.False(revertible.HasChanged("Id"));
    }

    [Fact]
    public async Task HasChanged_GivenChanges_ShouldReturnTrue()
    {
        var revertible = new MyRevertibleValidator();

        revertible.Id = 1;

        Assert.True(revertible.HasChanged("Id"));
    }

    [Fact]
    public async Task HasChanged_GivenAcceptedChanges_ShouldReturnFalse()
    {
        var revertible = new MyRevertibleValidator();
        revertible.Id = 1;

        revertible.AcceptChanges();

        Assert.False(revertible.HasChanged("Id"));
    }

    [Fact]
    public async Task Revert_GivenChanges_ShouldRevertTheChanges()
    {
        var revertible = new MyRevertibleValidator();

        revertible.Id = 1;

        revertible.Revert("Id");

        Assert.False(revertible.HasChanged("Id"));
    }

    public void Dispose()
    {
    }
}
