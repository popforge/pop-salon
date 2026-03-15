---
description: "Comprehensive guidelines for C# unit tests"
applyTo: "**/*Tests.cs"
---

# C# Unit Test Guidelines

## Framework and Setup

- Use xUnit3 as the testing framework
- Don't add `using Xunit.Abstractions` clause
- For mocking, use `Moq`

## Test Structure and Naming

### Test Class Naming

- Name test classes with the pattern: `{ClassUnderTest}Tests`
- Example: `UserServiceTests`, `ProductRepositoryTests`

## Mocking Guidelines

### General Mocking Rules

- You cannot Setup an extension method directly. Mock the underlying method called by the extension method
- You must configure all arguments of a method when you mock it, even if the argument is optional
- Use `Mock.Of<T>()` for simple mocks, `new Mock<T>()` for complex setups

## Async Testing Best Practices

### Always Use ConfigureAwait(false) in Production Code

- In tests, you can omit `ConfigureAwait(false)` as test runners handle context properly
- Always pass `TestContext.Current.CancellationToken` to async methods under test

## Common Anti-Patterns to Avoid

1. **Don't test implementation details** - Test behavior, not internal workings
2. **Don't use Thread.Sleep** - Use proper async/await patterns
3. **Don't ignore test warnings** - Address CA1707, xUnit analyzer warnings
4. **Don't test multiple concepts in one test** - Keep tests focused and atomic
5. **Don't use magic numbers/strings** - Use constants or clear variable names
6. **Don't catch and ignore exceptions** - Let tests fail with clear error messages

## Code Coverage Considerations

- Aim for high code coverage but focus on meaningful tests
- Use `[ExcludeFromCodeCoverage]` attribute for generated code or simple properties
- Cover edge cases, error conditions, and boundary values
- Don't write tests just to increase coverage percentage

## Integration with CI/CD

- Ensure tests are deterministic and don't rely on external dependencies
- Configure proper timeout values for async operations
- Consider parallel test execution settings
