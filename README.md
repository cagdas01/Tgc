# TGC for CQRS Pattern

This project is a .NET 6 web form application that generates C# files based on a given string input, adhering to the CQRS (Command Query Responsibility Segregation) pattern. It includes a predefined infrastructure setup and is designed to automate the creation of command, handler, and mapper classes for entity operations.

## Features

- **Command Class Generation:** Creates command classes with required properties and default values.
- **Command Handler Class Generation:** Generates command handler classes for handling entity operations.
- **Command Result Class Generation:** Creates command result classes to encapsulate the result of the command execution.
- **Mapper Class Generation:** Generates AutoMapper profile classes for mapping commands to entities and vice versa.
- **Validator Class Generation:** Creates FluentValidation validator classes for the commands.

## Getting Started

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- A suitable IDE such as [Visual Studio](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)

### Installation

1. **Clone the repository:**

   ```bash
   git clone https://github.com/your-username/cqrs-code-generator.git
   cd cqrs-code-generator
   cd {repository-name}
   dotnet restore
   dotnet build

 ### Usage
 private void ConfigureModel(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<YourEntity>().ToTable("YOUR_TABLE_NAME");
    // Additional configurations...
}
Based on this configuration file, the system automatically generates all necessary C# files for the specified entity. The generated files include:

Commands: Classes that perform CRUD operations.
Command Handlers: Define how the operations are processed.
Validators: Check the validity of the data sent.
Mappers: Facilitate the transformation between data models.

## Usage

To use this tool, you need to configure your `ModelBuilder` with your entity definition. Here is an example:

```csharp
private void ConfigureModel(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<YourEntity>().ToTable("YOUR_TABLE_NAME");
    // Additional configurations...
}

Copyright (c)

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


