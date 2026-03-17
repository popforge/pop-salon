---
applyTo: "src/forge/**/*.cs,src/forge/**/*.csproj"
---

# Forge — Testing instructions

These instructions apply to all unit and integration tests for the Forge CLI tool
(`src/forge/Popforge.CodeGen/` and its future test projects).

---

## Test project structure

Each testable unit has a sibling test project named with the `.Tests.UnitTests` suffix:

```
src/forge/
  Popforge.CodeGen/          ← production code
  Popforge.CodeGen.Tests.UnitTests/   ← unit tests
    Generators/
    Parsers/
    Validators/
    Commands/
    Helpers/
```

---

## Test class structure

- Test classes inherit from `GroupeIsa.Neos.Shared.XUnit.TestBase` when available; otherwise use xUnit directly with `ITestOutputHelper` injection.
- One test class per production class, named `<ClassUnderTest>Tests`.
- Constructor only receives `ITestOutputHelper` — never add constructor logic or validation assertions.

```csharp
public class MetadataValidatorTests : TestBase
{
    public MetadataValidatorTests(ITestOutputHelper output)
        : base(output)
    {
    }
}

public class BackendGeneratorTests : TestBase
{
    public BackendGeneratorTests(ITestOutputHelper output)
        : base(output)
    {
    }
}
```

---

## Naming conventions

Use the `Action_WhenCondition_ShouldResult` or `Action_ShouldResult` pattern:

```csharp
// Preferred
public async Task Validate_WhenEntityHasNoKeyProperty_ShouldReturnError()
public async Task Validate_WhenEntityViewFromReferencesUnknownProperty_ShouldReturnError()
public async Task Render_WhenTemplateNotFound_ShouldThrowFileNotFoundException()
public async Task Render_WhenStrictVariableIsUndefined_ShouldThrowInvalidOperationException()
public async Task ToPlural_WhenWordEndsWithS_ShouldNotAppendS()
public async Task Generate_ShouldWriteFilesUnderGeneratedSubfolder()

// Acceptable for simple cases
public void ToPlural_ShouldWork()
public void ToCamelCase_ShouldWork()
```

---

## AAA pattern

Use explicit `// Arrange`, `// Act`, `// Assert` comments in every test method:

```csharp
[Fact]
public void Validate_WhenEntityHasNoKeyProperty_ShouldReturnError()
{
    // Arrange
    ClusterDefinition cluster = new() { Name = "MyCluster", Namespace = "My.Cluster" };
    EntityDefinition entity = new()
    {
        Name = "Product",
        Properties = [new PropertyDefinition { Name = "Name", Type = "string" }]
    };

    // Act
    bool isValid = MetadataValidator.Validate(cluster, [entity], out List<string> errors);

    // Assert
    isValid.Should().BeFalse();
    errors.Should().ContainMatch("*IsKey*");
}
```

---

## Mocking

Use `AutoMocker` (Moq extension) together with `GetMock<T>()` from the base class.
Never use `new Mock<T>()` for dependencies that will be injected — this creates isolated
instances that the AutoMocker will not use.

```csharp
// Correct — mock is consistent with the injected dependency
GetMock<ITemplateRenderer>()
    .Setup(r => r.Render("backend/entity-view.cs.scriban", It.IsAny<object>()))
    .Returns("// generated content")
    .Verifiable();

BackendGenerator generator = Mocker.CreateInstance<BackendGenerator>();

// Incorrect — this mock will NOT be injected by Mocker.CreateInstance
Mock<ITemplateRenderer> isolatedMock = new();
```

Use `new Mock<T>()` only when you need multiple independent instances in the same test
(e.g., two different `ClusterDefinition` objects), passing them explicitly.

---

## Mock verification

- In `[Fact]` tests: always add `.Verifiable()` to `Setup()` calls and call `VerifyAll()` in the Assert section.
- In `[Theory]` tests: add `.Verifiable()` only on setups that are **not** driven by the theory parameters.
- Prefer `Mock.Invocations.Count.Should().Be(n)` when verifying that a branch was NOT executed.

```csharp
// Fact — verify the renderer was called exactly once
GetMock<ITemplateRenderer>()
    .Setup(r => r.Render("backend/controller.cs.scriban", It.IsAny<object>()))
    .Returns("// generated")
    .Verifiable();

// Assert
GetMock<ITemplateRenderer>().VerifyAll();

// Theory — verify conditionally
GetMock<ITemplateRenderer>().Verify(
    r => r.Render("backend/entity-view.cs.scriban", It.IsAny<object>()),
    dryRun ? Times.Never : Times.Once);
```

---

## Assertions

Use FluentAssertions exclusively. Never use `Assert.Equal` or `Assert.True`.

```csharp
// Booleans
isValid.Should().BeTrue();
isValid.Should().BeFalse();

// Errors — string collection with params
errors.Should().BeEquivalentTo("cluster.yml : 'Name' est requis.");

// Errors — partial match
errors.Should().ContainMatch("*IsKey*");

// Generic collections — with collection expression
result.Should().BeEquivalentTo([entity1View, entity2View]);

// File system
File.Exists(outputPath).Should().BeTrue();
File.ReadAllText(outputPath).Should().Contain("namespace My.Cluster.Application");

// Null checks
content.Should().NotBeNullOrEmpty();
```

---

## Theory and InlineData

Use `[Theory]` + `[InlineData]` to cover all Boolean combinations and boundary conditions.
This is especially relevant for `MetadataValidator`, `StringHelpers`, and template rendering.

```csharp
[Theory]
[InlineData("Appointment",  "Appointments")]
[InlineData("Status",       "Statuses")]   // irregular — update ToPlural if needed
[InlineData("Appointments", "Appointments")] // already plural — no double 's'
public void ToPlural_ShouldReturnExpectedResult(string input, string expected)
{
    // Act
    string result = StringHelpers.ToPlural(input);

    // Assert
    result.Should().Be(expected);
}

[Theory]
[InlineData(true,  false, true)]   // IsKey=true, Required=false → valid (key implies required)
[InlineData(false, true,  true)]   // normal required property → valid
[InlineData(false, false, true)]   // optional property → valid
public void Validate_PropertyVariants_ShouldMatchExpectation(
    bool isKey, bool required, bool expectedValid)
{
    // ... 
}
```

---

## Snapshot / golden-file tests for generators

Template rendering is best verified with snapshot tests: render a complete template with
a fixed input model and compare the output to a committed `.expected` file.

```
Popforge.CodeGen.Tests.UnitTests/
  Snapshots/
    backend/
      entity-view.expected.cs
      create-command.expected.cs
    frontend/
      view-dto.expected.ts
```

```csharp
[Fact]
public async Task RenderEntityViewTemplate_ShouldMatchSnapshot()
{
    // Arrange
    TemplateRenderer renderer = new();
    var model = new { cluster = TestFixtures.SampleCluster, entity = TestFixtures.SampleEntity };

    // Act
    string output = renderer.Render("backend/entity-view.cs.scriban", model);

    // Assert
    string expected = await File.ReadAllTextAsync("Snapshots/backend/entity-view.expected.cs");
    output.Should().Be(expected);
}
```

When a template changes intentionally, regenerate the `.expected` file and commit it
alongside the template change.

---

## Test fixtures and helper methods

Avoid duplicating entity/cluster setup across tests. Centralise in a static `TestFixtures` class:

```csharp
internal static class TestFixtures
{
    public static ClusterDefinition SampleCluster => new()
    {
        Name = "Acme",
        Namespace = "Acme.Crm",
        Company = "ACME Corp"
    };

    public static EntityDefinition SampleEntity => new()
    {
        Name = "Customer",
        Module = "Sales",
        Namespace = "Acme.Crm",
        Properties =
        [
            new PropertyDefinition { Name = "Id",   Type = "Guid",   IsKey = true, Generated = true },
            new PropertyDefinition { Name = "Name",  Type = "string", Required = true, MaxLength = 100 },
            new PropertyDefinition { Name = "Email", Type = "string", Required = false }
        ],
        EntityView = new EntityViewDefinition
        {
            Name = "CustomerView",
            Properties =
            [
                new ViewPropertyDefinition { Name = "Id",   From = "Id",   Type = "Guid",   Required = true },
                new ViewPropertyDefinition { Name = "Name", From = "Name", Type = "string", Required = true }
            ]
        }
    };
}
```

Use private helper methods within a test class for complex mock setup that is reused between
multiple tests in that class.

---

## Async conventions

- All async tests use `async Task` return type (never `async void`).
- Pass `TestContext.Current.CancellationToken` to all async calls under test.
- Never use `.Result` or `.Wait()`.

---

## What NOT to test

- Do not write a test that only verifies the constructor runs without throwing.
- Do not test Scriban syntax (the template parse errors are detected at boot via `Template.HasErrors`).
- Do not test file I/O directly — wrap the file system behind an interface (`IFileSystem`) and mock it.
