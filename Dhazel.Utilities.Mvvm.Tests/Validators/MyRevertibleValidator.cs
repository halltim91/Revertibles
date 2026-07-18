using CommunityToolkit.Mvvm.ComponentModel;
using Dhazel.Utilities.Mvvm.Validators;
using Dhazel.Utilities.Revertibles;


namespace Dhazel.Utilities.Mvvm.Tests.Validators;

public partial class MyRevertibleValidator : RevertibleValidator
{
    [ObservableProperty]
    [property: Revertible]
    private int _id;

    [ObservableProperty]
    [property: Revertible]
    private string? _myString;

    [ObservableProperty]
    [property: Revertible]
    private bool _myBool;
}
