# Repository-wide instructions

## Purpose
This repository contains:
- a code generation tool
- generated project sources

Always distinguish between:
- generator source code
- generator templates
- generated output

## General rules
- Prefer small, explicit, testable changes.
- Do not introduce hidden side effects.
- Preserve existing architecture unless the task explicitly asks for a redesign.
- When a file appears to be generated, prefer changing the generator or templates instead of editing generated output directly.
- Explain tradeoffs when modifying generation logic.
- Keep naming explicit and intention-revealing.
- Avoid magic strings and duplicated rules.

## Validation
- When changing generation behavior, also update related tests, snapshots, or golden files.
- Call out breaking changes in generated output.

## Copilot instruction best practices
- If a copilot instruction file is more than 4 000 characters, split it into multiple files otherwise it will not be considered in code review by GitHub.
