using CommunityToolkit.Mvvm.ComponentModel;
using Dasl.Utilities.Mvvm.Validators;
using Dasl.Utilities.Revertibles;


namespace Dasl.Utilities.Mvvm.Tests.Validators;

public partial class MyRevertibleValidator : RevertibleValidator
{
    [ObservableProperty]
    [property: Revertible]
    private int _id;

    [ObservableProperty]
    [property: Revertible]
    private string _myString;

    [ObservableProperty]
    [property: Revertible]
    private bool _myBool;
}
