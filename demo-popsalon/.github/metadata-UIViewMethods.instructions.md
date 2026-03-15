---
applyTo: "**/metadata/UIViewMethods/*.yml"
---

# UIView Methods Metadata Instructions

## Overview

This metadata file defines the methods available on the UIView.
See [UIView Methods Parameter Metadata Instructions](./metadata-UIViewMethodParameters.instructions.md) for parameter definitions.

## Method Definitions

- Name (string, required): Method Name (Should end with "Async" for asynchronous methods)
- Asynchronous (boolean, default: true): Indicates if the method is asynchronous
- Description (string, optional): A brief description of what the method does
- DotNetReturnType (string, optional): The C# return type of the method, including Task for async methods, null if void
- Code (string, required): The C# code that implements the method (see [./metatadata-code.instructions.md](./metadata-code.instructions.md) for help on writing C# code)
