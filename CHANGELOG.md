# Changelog

All notable changes to this project are documented here.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

---

## [v1.0.1] - 2026-05-11

### Added
- `CONTRIBUTING.md` with branch, commit, and PR guidelines
- `CHANGELOG.md`
- `SECURITY.md`
- English README (`README-en.md`)
- Revamped Swedish README (`README.md`)

---

## [v1.0.0] – 2026-05-10

First stable release. Packaged as a .NET global tool and published via GitHub Actions.

### Milestone 6 – Testing & Quality

- GitHub Actions CI workflow triggered on pull requests
- Unit tests for JSON parsing (`ChallengeParser`)
- Unit tests for file generation (`ExerciseScaffolder`)
- Unit tests for CLI argument parsing (`CliArgumentParser`)
- Unit tests for challenge validation (`ChallengeValidator`)
- Unit tests for prompt building (`PromptBuilder`)
- Integration tests for LLM calls
- Manual testing across all categories and difficulty levels
- Fixed: core generator tests were visible when running exercise tests
- Fixed: appsettings path resolved incorrectly in some environments

### Milestone 5 – Documentation & Polish

- `README.md` written with setup instructions and daily usage guide
- Example exercises added for `--dry-run`
- Code refactored and extracted to smaller, readable methods
- Final manual validation of all features

### Milestone 4 – User Experience

- Additional challenge categories added
- `list` command to show available categories and difficulties
- User flow for `hint`, `explain`, and description viewing
- Prompt builder updated to avoid leaking bug details in the buggy code

### Milestone 3 – Configuration & Extensibility

- `appsettings.json` configuration for provider and model selection
- Config reader extracted to its own class
- Multi-LLM support: `Gemini`, `OpenAI`, `Anthropic`, `Mistral`, `Ollama`
- `--reveal` / `explain` flag to show explanation after solving
- `.gitignore` for the exercises solution folder
- `appsettings.json` path resolved correctly regardless of working directory
- Packaged as a .NET NuGet global tool (`dotnet buggernaut`)
- GitHub Actions workflow to build and publish NuGet package

### Milestone 2 – Challenge Generation & Scaffolding

- Exercise scaffolding: generates `.cs` file and matching test file from LLM response
- Dynamic prompt template based on category and difficulty
- File generation pipeline from JSON response to disk
- TODO comments added to buggy code to guide the user
- `///summary` XML comments added to `--dry-run` example exercises
- Consistent namespace convention enforced across generated files

### Milestone 1 – Core CLI & LLM Integration

- Project plan and solution structure initialized
- LLM API integration via HTTP client
- `Challenge` model class defined
- JSON parser for LLM responses
- Challenge categories defined
- Prompt template created
- CLI argument parsing implemented (`generate`, `hint`, `explain`, `-c`, `-d`)
- `--dry-run` flag with mock challenge for testing without API key
- JSON validation with retry loop on malformed responses
- Error handling for HTTP errors (503, 429, 401, etc.)
- 503 response handling with retry logic

### Not implemented (skipped)

- Difficulty adaptivity based on user performance
- Save completed challenges to JSON
- Compile-time validation of generated code via `dotnet build`
