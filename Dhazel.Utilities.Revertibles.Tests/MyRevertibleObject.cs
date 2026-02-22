using Dhazel.Utilities.Revertibles;


namespace Dhazel.Utilities.Revertibles.Tests;

public class MyRevertibleObject
{
    [Revertible]
    public int Id { get; set; }

    [Revertible]
    public string MyString { get; set; }

    [Revertible]
    public bool MyBool { get; set; }
}
