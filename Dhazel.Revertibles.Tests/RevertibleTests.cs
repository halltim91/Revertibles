
using Dhazel.Revertibles;

# pragma warning disable CA1707 // warning for underscores in method names

namespace Dhazel.Revertibles.Tests;


public class RevertibleTests : IDisposable
{
    public RevertibleTests()
    {
    }

    [Fact]
    public async Task HasChanged_GivenEnabledChangeTracking_ShouldSucceed()
    {
        var observable = new MyRevertibleObject()
        {
            Id = 1
        };
        var revertible = new Revertible(observable);

        Assert.False(revertible.HasChanged("Id"));
    }

    [Fact]
    public async Task HasChanged_GivenAnUnChangedProperty_ShouldReturnFalse()
    {
        var observable = new MyRevertibleObject()
        {
            Id = 1
        };
        var revertible = Revertible.Track(observable);

        Assert.False(revertible.HasChanged("Id"));
    }

    [Fact]
    public async Task HasChanged_GivenAChangedProperty_ShouldReturnTrue()
    {
        var observable = new MyRevertibleObject()
        {
            Id = 1
        };
        var revertible = Revertible.Track(observable);

        observable.Id = 2;

        Assert.True(revertible.HasChanged("Id"));
    }

    [Fact]
    public async Task HasChanged_GivenNoChanges_ShouldReturnFalse()
    {
        var observable = new MyRevertibleObject()
        {
            Id = 1
        };
        var revertible = Revertible.Track(observable);

        Assert.False(revertible.HasChanged());
    }

    [Fact]
    public async Task HasChanged_GivenChanges_ShouldReturnTrue()
    {
        var observable = new MyRevertibleObject()
        {
            Id = 1,
            MyString = "123",
        };
        var revertible = Revertible.Track(observable);

        observable.Id = 2;
        observable.MyString = "321";

        Assert.True(revertible.HasChanged());
    }

    [Fact]
    public async Task Revert_GivenAChangedProperty_ShouldResetIt()
    {
        var initialId = 12345;
        var observable = new MyRevertibleObject()
        {
            Id = initialId,
        };
        var revertible = Revertible.Track(observable);

        observable.Id = 54321;

        revertible.Revert("Id");

        Assert.Equal(initialId, observable.Id);
    }

    [Fact]
    public async Task Revert_GivenChanges_ShouldResetThemAll()
    {
        var initialId = 12345;
        var initialMyString = "123";
        var observable = new MyRevertibleObject()
        {
            Id = initialId,
            MyString = initialMyString,
        };
        var revertible = Revertible.Track(observable);

        observable.Id = 54321;
        observable.MyString = "321";

        revertible.Revert();

        Assert.Equal(initialId, observable.Id);
        Assert.Equal(initialMyString, observable.MyString);
    }

    [Fact]
    public async Task AcceptChanges_GivenAChangedProperty_ShouldClearItsChangeFlag()
    {
        var initialId = 12345;
        var observable = new MyRevertibleObject()
        {
            Id = initialId,
        };
        var revertible = Revertible.Track(observable);

        var newId = 54321;
        observable.Id = newId;

        revertible.AcceptChanges("Id");

        Assert.Equal(newId, observable.Id);
        Assert.False(revertible.HasChanged("Id"));
    }

    [Fact]
    public async Task AcceptChanges_GivenChanges_ShouldClearAllChangeFlags()
    {
        var initialId = 12345;
        var initialMyString = "123";
        var observable = new MyRevertibleObject()
        {
            Id = initialId,
            MyString = initialMyString,
        };
        var revertible = Revertible.Track(observable);

        var newId = 54321;
        var newMyString = "321";

        observable.Id = newId;
        observable.MyString = newMyString;

        revertible.AcceptChanges();

        Assert.Equal(newId, observable.Id);
        Assert.Equal(newMyString, observable.MyString);
        Assert.False(revertible.HasChanged());
    }

    [Fact]
    public async Task HasChanged_GivenABooleanChangedToTrue_ShouldReturnTrue()
    {
        var observable = new MyRevertibleObject()
        {
            MyBool = false
        };
        var revertible = Revertible.Track(observable);

        observable.MyBool = true;

        Assert.True(revertible.HasChanged("MyBool"));
    }

    [Fact]
    public async Task HasChanged_GivenABooleanChangedToFalse_ShouldReturnTrue()
    {
        var observable = new MyRevertibleObject()
        {
            MyBool = true
        };
        var revertible = Revertible.Track(observable);

        observable.MyBool = false;

        Assert.True(revertible.HasChanged("MyBool"));
    }

    public void Dispose()
    {
        // nothing needed (yet)
    }
}
