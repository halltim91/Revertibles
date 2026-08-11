# Dhazel.Revertibles

.NET utilities for property change tracking: detect dirty state, revert to pristine values, and accept changes as the new baseline.

## Projects

| Project                               | Description                                                       |
| ---------                             | -------------                                                     |
| `Dhazel.Revertibles`                  | Core change-tracking API                                          |
| `Dhazel.Revertibles.Generators`       | Source generator + analyzers for `[RevertibleObject]`             |
| `Dhazel.Revertibles.Mvvm`             | MVVM integration (`RevertibleValidator` + CommunityToolkit.Mvvm)  |
| `Dhazel.TestHelpers`                  | Shared test helpers                                               |
| `Dhazel.Revertibles.Tests`            | Unit tests for Revertibles                                        |
| `Dhazel.Revertibles.Generators.Tests` | Unit tests for the generator and analyzers                        |
| `Dhazel.Revertibles.Mvvm.Tests`       | Unit tests for Mvvm                                               |

## Requirements

- .NET 10

## Install

```bash
dotnet add package Dhazel.Revertibles
dotnet add package Dhazel.Revertibles.Mvvm
```

## Usage

All patterns share the same API:

| Method                                      | Description                                                                  |
| --------                                    | -------------                                                                |
| `HasChanged()` / `HasChanged("Prop")`       | Whether any (or a specific) tracked property differs from its pristine value |
| `Revert()` / `Revert("Prop")`               | Restore all (or a specific) tracked properties to pristine values            |
| `AcceptChanges()` / `AcceptChanges("Prop")` | Snapshot current values as the new pristine baseline                         |

### 1. Attribute + `Revertible.Track`

Decorate properties with `[Revertible]`, then track any instance:

```csharp
using Dhazel.Revertibles;

public class Person
{
    [Revertible]
    public int Id { get; set; }

    [Revertible]
    public string? Name { get; set; }
}

var person = new Person { Id = 1, Name = "Ada" };
var revertible = Revertible.Track(person);

person.Name = "Grace";
revertible.HasChanged();           // true
revertible.HasChanged("Name");     // true

revertible.Revert("Name");
// person.Name == "Ada"

person.Name = "Grace";
revertible.AcceptChanges();
// "Grace" is now the pristine value
```

### 2. Inherit `AbstractRevertible`

When the type itself should be revertible:

```csharp
using Dhazel.Revertibles;

public class Person : AbstractRevertible
{
    [Revertible]
    public int Id { get; set; }

    [Revertible]
    public string? Name { get; set; }
}

var person = new Person { Id = 1, Name = "Ada" };

person.Name = "Grace";
person.HasChanged();   // true
person.Revert();       // Name restored to "Ada"
```

### 3. Third-party / undecorated types

Track objects you cannot annotate (or choose not to) with `WithProperties`:

```csharp
using Dhazel.Revertibles;

// No [Revertible] attributes on this type
public class ExternalDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

var dto = new ExternalDto { Id = 1, Name = "Ada" };
var revertible = Revertible.Track(dto)
    .WithProperties(["Id", "Name"]);

dto.Name = "Grace";
revertible.HasChanged("Name");  // true
revertible.Revert();            // restored
```

### 4. MVVM with `RevertibleValidator`

Combine change tracking with [CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/) source generators:

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using Dhazel.Revertibles.Mvvm.ObservableValidators;
using Dhazel.Revertibles;

public partial class PersonViewModel : RevertibleValidator
{
    [ObservableProperty]
    [property: Revertible]
    private int _id;

    [ObservableProperty]
    [property: Revertible]
    private string? _name;
}

var vm = new PersonViewModel { Id = 1, Name = "Ada" };

vm.Name = "Grace";
vm.HasChanged();  // true
vm.Revert();      // Name restored to "Ada"
```

### 5. `[RevertibleObject]` — zero-boilerplate codegen

Skip the base class and manual wiring entirely. Decorate any `partial` class with `[RevertibleObject]`, and mark the properties to track with `[Revertible]`. 
A source generator adds `AcceptChanges`, `HasChanged`, `Revert`, and `WithProperties` directly to your class:

*Note: When using this attribute the `Revertible` instance is lazily loaded and change tracking will not begin until `AcceptChanges` is called. This can be done in the revertible objects constructor*

```csharp
using Dhazel.Revertibles;

[RevertibleObject]
public partial class Person
{
    [Revertible]
    public int Id { get; set; }

    [Revertible]
    public string? Name { get; set; }
}

var person = new Person { Id = 1, Name = "Ada" };
// AcceptChanges() runs automatically after construction by default

person.Name = "Grace";
person.HasChanged();   // true
person.Revert();       // Name restored to "Ada"
```

The class must be declared `partial` — an analyzer (`REV001`) flags this as a compile error if you forget. A second analyzer (`REV002`) warns if a `[RevertibleObject]` class has no `[Revertible]` properties, since that combination silently does nothing.

#### Deferring the initial snapshot

By default, the generated code snapshots pristine values the first time you call `AcceptChanges`, `HasChanged`, `Revert`, or `WithProperties` — which, left alone, happens automatically right after construction. 
If your class needs additional setup before its properties are safe to snapshot (e.g. a base class that finishes initializing after your constructor runs), set `AcceptPristineValues = false` and call `AcceptChanges()` yourself once setup is complete:

```csharp
[RevertibleObject(AcceptPristineValues = false)]
public partial class PersonViewModel : ObservableValidator
{
    [Revertible]
    public int Id { get; set; }

    public PersonViewModel()
    {
        // ... setup that must happen before values are snapshotted ...
        AcceptChanges();
    }
}
```

Reference both packages/projects as needed:

- `Dhazel.Revertibles` — core types
- `Dhazel.Revertibles.Mvvm` — `RevertibleValidator` (depends on Revertibles and CommunityToolkit.Mvvm)

## Build & test

```bash
dotnet build Dhazel.Revertibles.slnx
dotnet test Dhazel.Revertibles.slnx
```

## Releasing

Versions come from git tags via [MinVer](https://github.com/adamralph/minver) (`v1.0.0` → package `1.0.0`).

1. Ensure `NUGET_USER` is set as a GitHub Actions secret (nuget.org API key).
2. Push an annotated tag:

```bash
git tag -a v1.0.0 -m "v1.0.0"
git push origin v1.0.0
```

The Release workflow runs tests, packs, and publishes to nuget.org.

## Solution layout

```
root/
├── Dhazel.Revertibles/           # Core library
├── Dhazel.Revertibles/Generators # Roslyn Generators
├── Dhazel.Revertibles.Mvvm/      # MVVM helpers
├── Dhazel.TestHelpers/           # Test utilities
├── Dhazel.Revertibles.Tests/
├── Dhazel.Revertibles.Generators.Tests/
├── Dhazel.Revertibles.Mvvm.Tests/
└── Dhazel.Revertibles.slnx
```

## License

MIT — see [LICENSE](LICENSE).
