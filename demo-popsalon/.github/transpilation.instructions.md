---
applyTo: "server/src/Transpiler/**/*.*
---

# Instructions for add, modify or remove transpilation

## What is transpilation ?

The front business code is written in c# and transpiled to typescript using the transpiler.  
The transpiler used roslyn to visit the c# code and generate the typescript code.  
The c# symbols are converted to typescript symbols using yaml files.

The yaml files are located in the `server/src/Transpiler/GroupeIsa.Neos.Transpiler.Symbols/Symbols` directory.

## Symbol Yaml files

A yaml file contains the transpilation for a specific c# type. The name of the file is the full name of the c# type.
Every entries in the yaml file is a c# symbol with the following properties:

- **CSharpMember** : the type of the c# symbol (T : type, M : Method, P : Property, F : Field, C : Constructor, I : Indexer, L : Literal)
- **CSharpSymbol** : the full name of the c# symbol
- **TsTemplate** : the typescript template for the symbol, if empty the symbol is not transpiled, {this} represents the current instance of the class, {0} represents the first parameter of the method, {1} represents the second parameter of the method, etc.
- **Dependencies** : the list of dependencies for the symbol if a typescript module is needed
- **Tests** : the list of tests for the symbol to check if the transpilation is correct

> [!WARNING]
> If the symbol cannot be transpiled, the TsTemplate must be empty(`TsTemplate: ""`), dont add a comment in the yaml file and dont set `TsTemplate: "//Not Implemented"`.

## Dependencies entry

The sub entries of the dependencies entry are the following:

- **Module** : the name of the typescript module
- **Imports** : the import statements for the module needed in the transpiled code

## Tests entry

The sub entries of the tests entry are the following:

- **Cs**: the c# code to test the transpiled code
- **Ts**: the expected typescript code
- **ReturnType**: if different from the symbol type, the return type must be specified

### Example

File : System.String.yml

- Type

```yaml
- CSharpMember: T
  CSharpSymbol: "System.String"
  TsTemplate: "string"
```

- Generic Type, {T} is the parameter of the generic type

```yaml
- CSharpMember: T
  CSharpSymbol: "System.Collections.Generic.IList<T>"
  TsTemplate: "{T}[]"
```

- Literal

```yaml
- CSharpMember: L
  CSharpSymbol: '"ABC"'
  TsTemplate: "'ABC'"
```

- Constructor

```yaml
- CSharpMember: C
  CSharpSymbol: "String(System.Char[])"
  TsTemplate: "{0}.join('')"
  Tests:
    - Cs: return new string(new char[]{'A', 'B'});
      Ts: return ['A', 'B'].join('');
```

- Field

```yaml
- CSharpMember: F
  CSharpSymbol: "Empty"
  TsTemplate: "''"
```

- Property

```yaml
- CSharpMember: P
  CSharpSymbol: "Length"
  Infos: "Instance - System.Int32"
  TsTemplate: "{this}.length"
  Tests:
    - Cs: return "ABC".Length;
      Ts: return 'ABC'.length;
```

- Indexer

```yaml
- CSharpMember: I
  CSharpSymbol: "Chars[System.Int32]"
  Infos: "Instance - System.Char"
  TsTemplate: "{this}[{0}]"
  Tests:
    - Cs: return "ABC"[1];
      Ts: return 'ABC'[1];
```

- Instance Method

```yaml
- CSharpMember: M
  CSharpSymbol: "Compare(System.String, System.String)"
  Infos: "Static - System.Int32"
  TsTemplate: "{0}.localeCompare({1})"
  Tests:
    - Cs: return string.Compare("ABC", "ABC");
      Ts: return 'ABC'.localeCompare('ABC');
      ReturnType:
    - Cs: return string.Compare("ABC", "DEF") < 0;
      Ts: return 'ABC'.localeCompare('DEF') < 0;
      ReturnType: System.Boolean
```

- Static Method

```yaml
- CSharpMember: M
  CSharpSymbol: "Concat(System.Object, System.Object, System.Object)"
  Infos: "Static - System.String"
  TsTemplate: "[{0}, {1}, {2}].join('')"
  Tests:
    - Cs: return string.Concat(1, "A", 'c');
      Ts: return [1, 'A', 'c'].join('');
```

- Dependency

```yaml
- CSharpMember: M
  CSharpSymbol: "Contains(System.Char, System.StringComparison)"
  Infos: "Instance - System.Boolean"
  TsTemplate: "contains({this}, {0}, {1})"
  Dependencies:
    - Module: "@neos/shared"
      Imports:
        - contains
        - StringComparison
```

# Guidelines

- The `TsTemplate` entry must be a valid typescript code, and not be a complex typescript code.
- If needed, a typescript function must be added in the package `@neos/shared` located in the `client\packages\shared` directory.
  - A typescript can be added in the `client\packages\shared\src\helpers` directory or a function can be added in existing typescript files of this directory.
  - If a typescript function is added a unit test must be added in the `__tests__` subdirectory.
  - When you use an imported function in the transpilation, you must read the code of the function to understand how it works and if it is correct. If you have any doubt about the function, you can ask for help.
-
- Don't build the solution, but you can run and execute the project `server/src/Transpiler/GroupeIsa.Neos.Transpiler.SymbolsGenerator` to test the transpilation.
- When you add the transpilation for a new symbol, you must add entry tests for the symbol in the yaml file if possible (for example, an interface or an abstract class cannot be tested).

# Generate unit tests from test entry

- The `test` entry is used to generate the unit tests for the transpiled code.
- To Generate the unit tests run the `server/src/Transpiler/GroupeIsa.Neos.Transpiler.SymbolsGenerator` the project, it will generate the unit tests in the `server/src/Transpiler/GroupeIsa.Neos.Transpiler.Snippets.Tests.UnitTests` project (in the subdirectory `SymbolsUnitTests`).
