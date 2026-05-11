# Buggernaut

[![.NET 10](https://img.shields.io/badge/.NET-10-512BD4?style=flat-square&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/download) 
[![C%23](https://img.shields.io/badge/language-C%23-239120?style=flat-square&logo=csharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/) 
[![CLI](https://img.shields.io/badge/type-CLI-0A7EA4?style=flat-square)](#what-is-this) 
[![Contributions Welcome](https://img.shields.io/badge/contributions-welcome-brightgreen?style=flat-square&logo=github)](CONTRIBUTING.md) 
[![NuGet](https://img.shields.io/badge/NuGet-package-004880?style=flat-square&logo=nuget&logoColor=white)](https://www.nuget.org/packages/Buggernaut) 
[![License: WTFPL v2](https://img.shields.io/badge/license-WTFPL%20v2-red?style=flat-square)](LICENSE.md)

> ##### Link to the Swedish README file: [README.md](README.md)

Buggernaut is a CLI tool that generates C# exercises with built-in bugs through an LLM provider of your choice.  
The idea is simple: open a file, find the bug, run the tests, get better at programming.

The project is designed to give junior developers a practical way to train bug hunting and bug fixing, while building a
stronger understanding of programming concepts without leaving their own development environment.

> Inspired by [ThePrimeagen's Kata-Machine](https://github.com/ThePrimeagen/kata-machine).
> Thanks to [Marcus Loof](https://github.com/LeafMaster1) for ideas and feedback.

---

## What is this?

You run a command. Buggernaut asks an AI for a C# exercise, writes a `.cs` file with an intentional bug, and adds a
matching test file.
Your task is to find and fix the bug until the tests are green.

No browser tabs. No sign-up. Everything happens in your editor.

### Quick demo

![Quick demo of Buggernaut](docs/assets/BuggernautQuickRun.gif)

---

## Contents

- [Prerequisites](#prerequisites)
- [Quick start](#quick-start)
- [Daily usage](#daily-usage)
- [Configuration](#configuration)
- [Run tests](#run-tests)
- [Common issues](#common-issues)
- [Want to contribute?](#want-to-contribute)
- [Contact](#contact)

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- An API key for any provider (for example `Gemini`, `OpenAI`, `Anthropic`, `Mistral`)

> `Ollama` works without an API key, but requires a local server.

Want to try it without an API key first? Use `--dry-run`.

![Dry run without an API key](docs/assets/BuggernautDryRun.png)

---

## Quick start

Run all commands from `Buggernaut/` unless stated otherwise.

### 1) Install the tool

```bash
cd Buggernaut
dotnet tool restore
```

### 2) Save API key (one-time setup)

Run in `tools/Buggernaut.Generator/`:

```bash
cd tools/Buggernaut.Generator
dotnet user-secrets set "LLM:Gemini:ApiKey" "your-key"
```

The key is stored locally on your machine and is never committed to Git.

Using another provider? Just replace `Gemini`:

```bash
dotnet user-secrets set "LLM:OpenAI:ApiKey" "your-key"
dotnet user-secrets set "LLM:Anthropic:ApiKey" "your-key"
dotnet user-secrets set "LLM:Mistral:ApiKey" "your-key"
```

Need help finding your provider API key?

- **Gemini**: <https://aistudio.google.com/app/apikey>
- **OpenAI**: <https://platform.openai.com/api-keys>
- **Anthropic**: <https://console.anthropic.com/settings/keys>
- **Mistral**: <https://console.mistral.ai/>

### 3) Generate your first exercise

```bash
cd Buggernaut
dotnet buggernaut generate
```

A new `.cs` file appears in `src/Buggernaut.Exercises/`.
Open it, find the bug, and fix it.

![Quick start with a generated exercise](docs/assets/BuggernautQuickStart.gif)

---

## Daily usage

```bash
dotnet buggernaut generate                         # generate a new exercise
dotnet buggernaut generate -c LINQ -d Hard         # choose category and difficulty
dotnet buggernaut generate --dry-run               # test without API key

dotnet test exercises.slnf                         # run your exercise tests

dotnet buggernaut hint <ClassName>                 # get a hint when stuck
dotnet buggernaut explain <ClassName>              # get explanation when tests pass
```

> **Tip:** See all flags and categories with:
>
>```bash
> dotnet buggernaut generate --help
>```

![CLI help for the generate command](docs/assets/BuggernautHelp.png)

---

## Configuration

### Change LLM provider

Open `tools/Buggernaut.Generator/appsettings.json` and change `"Provider"`:

```json
{
  "LLM": {
    "Provider": "Gemini"
  }
}
```

Available providers: `Gemini`, `OpenAI`, `Anthropic`, `Mistral`, `Ollama`.
> Missing a provider? [Open an issue](https://github.com/discovicke/buggernaut/issues) and suggest it.

Want to run fully local without internet? Set up [Ollama](https://ollama.com) and use:

```json
{
  "LLM": {
    "Provider": "Ollama",
    "Ollama": {
      "BaseUrl": "http://localhost:11434/v1",
      "Model": "llama3"
    }
  }
}
```

#### Optional: set model per provider

```json
{
  "LLM": {
    "Provider": "Gemini",
    "Gemini": {
      "Model": "gemini-2.5-flash"
    },
    "OpenAI": {
      "Model": "gpt-4o-mini"
    },
    "Anthropic": {
      "Model": "claude-3-5-haiku-latest"
    },
    "Mistral": {
      "Model": "mistral-small"
    },
    "Ollama": {
      "BaseUrl": "http://localhost:11434/v1",
      "Model": "llama3"
    }
  }
}
```

---

## Run tests

| Command                      | What it runs                             |
|------------------------------|------------------------------------------|
| `dotnet test exercises.slnf` | Your exercise tests (`Buggernaut.Tests`) |
| `dotnet test generator.slnf` | Generator's own unit tests               |
| `dotnet test`                | All tests                                |

---

## Common issues

**`dotnet buggernaut` is not found**

- Run `dotnet tool restore` in `Buggernaut/`.

**API key is not found**

- Confirm `user-secrets` was set in `tools/Buggernaut.Generator/`.

**Wrong provider or model is used**

- Check `tools/Buggernaut.Generator/appsettings.json`.

---

## Want to contribute?

Great! Read [CONTRIBUTING.md](CONTRIBUTING.md) for branch, commit, and pull request guidelines.
No pressure, no stress - we learn together.

## Contact

Want to reach me? You can find contact details on [GitHub](https://github.com/discovicke), or send
an [email](mailto:johanssonviktor@pm.me).
