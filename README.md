# HolmiumOS

HolmiumOS is a hobby operating system written in C# using the Cosmos framework. It aims to provide a lightweight command-line environment while exploring operating system concepts such as shell development, file systems, scripting, and system utilities.

> **Status:** Work in Progress 🚧

## Overview

HolmiumOS is an experimental operating system created for learning and experimenting with operating system development. The project focuses on modular design, allowing new functionality to be added through independent components and commands.

## Features

- [Powerful UX]
- [Memory and storage resource checks]
- [Interactive command-line shell]
- [Scripting language interpreter]

## Components

### Shell

A modular command-line shell responsible for user interaction and command execution.

### File System

Provides support for basic file and directory operations.

### MIV Text Editor

**MIV** is the built-in terminal-based text editor for creating and editing text files.

### HE Interpreter

A built-in interpreter for the HE scripting language.

### Utilities

HolmiumOS includes various built-in utilities for system management, file handling, mathematical operations, and other everyday tasks.

## Project Structure

```text
Commands/
    Executable/
    FileSystem/
    Fun/
    Math/
    System/
    Tools/

HE/
Shell/

Kernel.cs
```

## Building

### Requirements

- .NET 6 SDK
- Cosmos DevKit
- Visual Studio 2022 (recommended)

Build the project:

Open the solution in Visual Studio 2022 and build the project. The bootable image will be generated automatically by Cosmos.

## Running

Build the project with Cosmos and launch it using the Cosmos debugger or Visual Studio.

## Roadmap

- [ ] More shell features
- [ ] Better file system support
- [ ] More built-in utilities
- [ ] Improve the MIV text editor
- [ ] Expand the HE interpreter
- [ ] Performance improvements
- [ ] Bug fixes

## License

This project is currently not licensed.
