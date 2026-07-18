
using Dhazel.Utilities.Revertibles;
using Dhazel.Utilities.TestHelpers;
using Xunit.Abstractions;

# pragma warning disable CA1707 // warning for underscores in method names

namespace Dhazel.Utilities.Revertibles.Tests;


public class ThirdPartyRevertibleTests(ITestOutputHelper output) : IDisposable
{

    [Fact]
    public async Task Revert_GivenNoConfiguredProps_ShouldThrow()
    {
        var observable = new MySimpleObject()
        {
            Id = 1
        };
        var revertible = new Revertible(observable);

        Assert.Throws<RevertibleException>(() => revertible.Revert("Id"));
    }

    [Fact]
    public async Task HasChanged_GivenNoChanges_ShouldReturnFalse()
    {
        var observable = new MySimpleObject()
        {
            Id = 1
        };
        var revertible = new Revertible(observable).WithProperties(["Id"]);

        Assert.False(revertible.HasChanged("Id"));
    }

    [Fact]
    public async Task HasChanged_GivenAChangedProperty_ShouldReturnTrue()
    {
        var observable = new MyRevertibleObject()
        {
            Id = 1
        };
        var revertible = Revertible.Track(observable).WithProperties(["Id"]);

        observable.Id = 2;

        Assert.True(revertible.HasChanged("Id"));
    }

    [Fact]
    public async Task HasChanged_GivenChanges_ShouldReturnTrue()
    {
        var observable = new MySimpleObject()
        {
            Id = 1,
            MyString = "123",
        };
        var revertible = Revertible.Track(observable).WithProperties(["Id", "MyString"]);

        observable.Id = 2;
        observable.MyString = "321";

        Assert.True(revertible.HasChanged());
    }

    [Fact]
    public async Task Revert_GivenAChangedProperty_ShouldResetIt()
    {
        var initialId = 12345;
        var observable = new MySimpleObject()
        {
            Id = initialId,
        };
        var revertible = Revertible.Track(observable).WithProperties(["Id"]);

        observable.Id = 54321;

        revertible.Revert("Id");

        Assert.Equal(initialId, observable.Id);
    }

    [Fact]
    public async Task Revert_GivenChanges_ShouldResetThemAll()
    {
        var initialId = 12345;
        var initialMyString = "123";
        var observable = new MySimpleObject()
        {
            Id = initialId,
            MyString = initialMyString,
        };
        var revertible = Revertible.Track(observable).WithProperties(["Id", "MyString"]);

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
        var observable = new MySimpleObject()
        {
            Id = initialId,
        };
        var revertible = Revertible.Track(observable).WithProperties(["Id"]);

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
        var observable = new MySimpleObject()
        {
            Id = initialId,
            MyString = initialMyString,
        };
        var revertible = Revertible.Track(observable).WithProperties(["Id", "MyString"]);

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
        var observable = new MySimpleObject()
        {
            MyBool = false
        };
        var revertible = Revertible.Track(observable).WithProperties(["MyBool"]);

        observable.MyBool = true;

        Assert.True(revertible.HasChanged("MyBool"));
    }

    [Fact]
    public async Task HasChanged_GivenABooleanChangedToFalse_ShouldReturnTrue()
    {
        var observable = new MySimpleObject()
        {
            MyBool = true
        };
        var revertible = Revertible.Track(observable).WithProperties(["MyBool"]);

        observable.MyBool = false;

        Assert.True(revertible.HasChanged("MyBool"));
    }

    public void Dispose()
    {
        // nothing needed (yet)
    }
}
