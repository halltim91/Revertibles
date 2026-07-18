using CommunityToolkit.Mvvm.ComponentModel;
using Dhazel.Utilities.Revertibles.Mvvm.ObservableValidators;
using Dhazel.Utilities.Revertibles;


namespace Dhazel.Utilities.Revertibles.Mvvm.Tests.ObservableValidators;

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
