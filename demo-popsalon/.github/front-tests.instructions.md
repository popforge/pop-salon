---
applyTo: "**/*.test.ts"
---
# Test Instructions for typeScript and Vue.js components

## Test Framework
- Use [Vitest](https://vitest.dev/) for unit tests
- Use [Vue Test Utils](https://vue-test-utils.vuejs.org/) for Vue.js component testing

## Test Structure
- Use the AAA pattern (Arrange, Act, Assert)
- Use `describe` blocks to group related tests
- Use `it` blocks for individual test cases
- Use `beforeEach` and `afterEach` for setup and teardown
- Use `expect` for assertions
- Use `mock` and `spy` for mocking and spying on functions
- Use `vi.fn()` for creating mock functions
- Use `vi.spyOn()` for spying on functions
- Use `vi.mock()` for mocking modules

## Test files
- Test files should be named with the same name as the file they are testing with the suffix `.test.ts`
- Test files should be located in the same directory as the file they are testing
