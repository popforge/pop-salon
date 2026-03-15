---
applyTo: "**/metadata/UIViewMethodParameter s/*.yml"
---

# UIView Methods Parameter Metadata Instructions

## Overview

This metadata file defines the parameters for the methods available on the UIView.

## Method Parameter Definitions

- UIViewMethodName (string, required): The name of the method this parameter belongs to
- Name (string, required): Parameter Name (should follow C# naming conventions)
- DotNetDataType (string, required): Parameter Type (should be a valid C# type)
- Position (integer, required): The position of the parameter in the method signature (1-based index)
