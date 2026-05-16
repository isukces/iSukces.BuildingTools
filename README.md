# iSukces.BuildingTools

[![NuGet](https://img.shields.io/nuget/v/iSukces.Build.svg)](https://www.nuget.org/packages/iSukces.Build/)  
[Polska wersja](README-pl.md)

Build automation and tooling library for .NET projects. Provides utilities for MSBuild/Dotnet CLI invocation, solution file parsing, Inno Setup script generation, file synchronization, and compiler directive management.

## Requirements

- .NET SDK 6.0, 8.0, 9.0, or 10.0
- Windows (MSBuild integration)

## Installation

```
dotnet add package iSukces.Build
```

## Usage

The library is referenced from custom build scripts that inherit from `BuildingScriptBase`. Key namespaces:

- `iSukces.Build` — base script class, command-line handling, file operations
- `iSukces.Build.InnoSetup` — Inno Setup script model and builder
- `iSukces.Build._sln` — `.sln` file parser
- `iSukces.Build._msBuild` — MSBuild CLI wrapper
- `iSukces.Build._dotnetBuild` — `dotnet` CLI wrapper

## Build

```
cd app
dotnet build iSukces.BuildingTools.sln
```

## Tests

```
cd app
dotnet test iSukces.Build.Tests
```

Test framework: xUnit.

## License

MIT
