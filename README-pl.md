# iSukces.BuildingTools

[![NuGet](https://img.shields.io/nuget/v/iSukces.Build.svg)](https://www.nuget.org/packages/iSukces.Build/)  
[English version](README.md)

Biblioteka do automatyzacji budowania i narzędzi dla projektów .NET. Udostępnia narzędzia do wywoływania MSBuild/Dotnet CLI, parsowania plików rozwiązania, generowania skryptów Inno Setup, synchronizacji plików oraz zarządzania dyrektywami kompilatora.

## Wymagania

- .NET SDK 6.0, 8.0, 9.0 lub 10.0
- Windows (integracja z MSBuild)

## Instalacja

```
dotnet add package iSukces.Build
```

## Użycie

Biblioteka jest referencjonowana ze skryptów budowania dziedziczących po `BuildingScriptBase`. Główne przestrzenie nazw:

- `iSukces.Build` — klasa bazowa skryptu, obsługa wiersza poleceń, operacje na plikach
- `iSukces.Build.InnoSetup` — model i budowniczy skryptów Inno Setup
- `iSukces.Build._sln` — parser plików `.sln`
- `iSukces.Build._msBuild` — wrapper MSBuild CLI
- `iSukces.Build._dotnetBuild` — wrapper `dotnet` CLI

## Budowanie

```
cd app
dotnet build iSukces.BuildingTools.sln
```

## Testy

```
cd app
dotnet test iSukces.Build.Tests
```

Framework testowy: xUnit.

## Licencja

MIT
