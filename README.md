# Buggernaut

Buggernaut är ett CLI-verktyg som genererar C#-övningar med buggar med hjälp av en valfri LLM-provider.

#### Inspiration
Inspiration från och tack till [ThePrimeagen's Kata-Machine](https://github.com/ThePrimeagen/kata-machine) samt alla samtal med [Marcus Lööf](https://github.com/LeafMaster1) kring studietekniker och programmering.

---

## Kör testerna

Buggernaut innehåller två typer av tester som hålls separata med solution-filter:

| Kommando | Vad körs                                      |
|---|-----------------------------------------------|
| `dotnet test exercises.slnf` | **Dina övningar** - enbart `Buggernaut.Tests` |
| `dotnet test generator.slnf` | Generatorns egna enhetstester                 |
| `dotnet test` | Allt - båda testprojekten                     |

> Kör alltid från `Buggernaut/`-mappen.

```bash
cd Buggernaut
dotnet test exercises.slnf
```

---

## Konfiguration

### 1. Välj provider

Öppna `tools/Buggernaut.Generator/appsettings.json` och sätt `Provider` till den tjänst du vill använda.

```json
{
  "LLM": {
    "Provider": "Gemini"
  }
}
```

Tillgängliga providers: `Gemini`, `OpenAI`, `Anthropic`, `Mistral`, `Ollama`

---

### 2. Välj modell (valfritt)

Varje provider har en förinställd standardmodell. Du kan byta modell under respektive providers nyckel i `appsettings.json`:

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

> **Ollama** kräver ingen API-nyckel men den behöver en lokal server igång. Ändra `BaseUrl` om din server lyssnar på en annan adress.

---

### 3. Ange API-nyckel

API-nycklar sparas **aldrig** i `appsettings.json`. Använd istället `dotnet user-secrets` så lagras nyckeln säkert på din dator utanför källkoden.

Kör kommandot för din valda provider från `tools/Buggernaut.Generator/`-mappen:

```bash
# Gemini
dotnet user-secrets set "LLM:Gemini:ApiKey" "din-nyckel"

# OpenAI
dotnet user-secrets set "LLM:OpenAI:ApiKey" "din-nyckel"

# Anthropic
dotnet user-secrets set "LLM:Anthropic:ApiKey" "din-nyckel"

# Mistral
dotnet user-secrets set "LLM:Mistral:ApiKey" "din-nyckel"
```

> Var hittar jag min API-nyckel?
> - **Gemini** → [aistudio.google.com](https://aistudio.google.com/app/apikey)
> - **OpenAI** → [platform.openai.com/api-keys](https://platform.openai.com/api-keys)
> - **Anthropic** → [console.anthropic.com/settings/keys](https://console.anthropic.com/settings/keys)
> - **Mistral** → [console.mistral.ai](https://console.mistral.ai/)

---

### Snabbstart – exempel med Gemini

```bash
# 1. Gå till generator-mappen
cd tools/Buggernaut.Generator

# 2. Spara din API-nyckel
dotnet user-secrets set "LLM:Gemini:ApiKey" "din-nyckel"

# 3. Starta generatorn
dotnet run
```

> **Tips:** Kör `dotnet run -- generate --help` för att snabbt se alla flaggor, kategorier och exempel direkt i terminalen utan att behöva öppna README:n.

