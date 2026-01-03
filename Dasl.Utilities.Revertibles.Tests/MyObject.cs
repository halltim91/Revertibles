using Dasl.Utilities.Revertibles;


namespace Dasl.Utilities.Revertibles.Tests;

public class MyObject
{
    [Revertible]
    public int Id { get; set; }

    [Revertible]
    public string MyString { get; set; }

    [Revertible]
    public bool MyBool { get; set; }
}
