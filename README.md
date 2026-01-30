# Clarity.Dev

**A .NET Solution Analyzer for Developer Onboarding**

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-green)](https://opensource.org/licenses/MIT)

## Overview

Clarity.Dev is a powerful tool designed to help developers quickly understand and navigate .NET codebases.
It is perfect for onboarding Junior developers or getting up to speed on unfamiliar projects.

---

**Please Note:** *The project is still in its development phase and the MVP has not been completed.*

---
### Features
Once completed, Clarity.Dev will offer the following features:
- 🔍 **Solution Analysis**: Automatically scans `.sln` and `.slnx` files and all projects
- 📦 **Dependency Mapping**: Visualizes project-to-project dependencies and NuGet packages
- 🎯 **Service Detection**: Identifies controllers, APIs, background services, SignalR hubs, and gRPC services
- 🌐 **Communication Analysis**: Detects HTTP clients, message queues (RabbitMQ, Azure Service Bus, Kafka), and database access
- ⚠️ **Circular Dependency Detection**: Warns about circular references between projects
- 📊 **Rich Reports**: Generates HTML reports with interactive Mermaid.js diagrams
- 📄 **JSON Export**: Export analysis results for automation and custom processing

## Getting Started

### Installation

Currently, you need to build from source:

```
git clone https://github.com/Sphesihlebjamile/Clarity.Dev.git
cd Clarity.Dev
dotnet build
```

### Usage

```
dotnet run --project /Clarity.Dev/src/Clarity.Dev.CLI/ (<YourSolution.sln/.slnx>)
```
- **(<YourSolution.sln/.slnx)** represents the absolute/relative path to the solution file you want to analyze.

## Technology Stack
| Technology | Purpose |
|------------|---------|
| .NET 8.0  | Core framework for building the application (Cross-platform runtime) |
| **Roslyn (Microsoft.CodeAnalysis)** | C# syntax and semantic analysis |
| **Buildalyzer** | MSBuild project analysis |
| **Mermaid.js** | Interactive diagrams in HTML reports |
| **QuestPDF** | PDF generation (planned for v2) |

## Roadmap

### Phase 1: Core Analyzer (In-Progress)
- [x] Solution scanning and project parsing
- [x] Dependency graph building
- [x] Service detection
- [x] Communication analysis
- [x] Circular dependency detection
- [ ] HTML report generation
- [ ] JSON export

### Phase 2: Enhanced Reports (In Progress)
- [ ] PDF export with QuestPDF
- [ ] Improved diagram rendering
- [ ] Code metrics (lines of code, complexity)
- [ ] Architecture pattern detection

### Phase 3: IDE Extensions
- [ ] Visual Studio 2022 extension (VSIX)
- [ ] VS Code extension (cross-platform)

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Credits

Built with ❤️ using:
- [Roslyn](https://github.com/dotnet/roslyn) - The .NET Compiler Platform
- [Buildalyzer](https://github.com/daveaglick/Buildalyzer) - MSBuild project analysis
- [Mermaid.js](https://mermaid.js.org/) - Generation of diagrams from markdown-like syntax
- [QuestPDF](https://www.questpdf.com/) - Modern PDF generation library

## Support

For issues and questions, please open an issue on GitHub.

---

**Happy Onboarding with *Clarity.Dev*! 🚀**