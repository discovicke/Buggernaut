# Buggernaut

Buggernaut är ett CLI-verktyg som genererar C#-övningar med buggar med hjälp av en valfri LLM-provider. Det är designat för att hjälpa utvecklare att träna på att hitta och fixa buggar i kod, samt att förbättra sin förståelse av olika programmeringskoncept utan att behöva lämna sin egna egna utvecklingsmiljö.

> Buggernauts grundkoncept är inspirerat av [ThePrimeagens Kata-Machine](https://github.com/ThePrimeagen/kata-machine). Tack till ytterligare inspiration från idésprutan [Marcus Lööf](https://github.com/LeafMaster1) som agerat bollplank och samtalat kring progammeringsprojekt och studietekniker.

---

## Förutsättningar

- [.NET 10 SDK](https://dotnet.microsoft.com/download) installerat
- En API-nyckel från valfri LLM-provider (se nedan)

---

## Kom igång

Allt körs från `Buggernaut/`-mappen.

### 1. Installera verktyget
```bash
cd Buggernaut
dotnet tool restore
```

### 2. Ange API-nyckel (en gång)

Kör från `tools/Buggernaut.Generator/` för att spara din nyckel:
```bash
cd tools/Buggernaut.Generator

# Välj din provider:
dotnet user-secrets set "LLM:Gemini:ApiKey"    "din-nyckel"
dotnet user-secrets set "LLM:OpenAI:ApiKey"    "din-nyckel"
dotnet user-secrets set "LLM:Anthropic:ApiKey" "din-nyckel"
dotnet user-secrets set "LLM:Mistral:ApiKey"   "din-nyckel"
```

> Var hittar jag min API-nyckel?
> - **Gemini** → [aistudio.google.com](https://aistudio.google.com/app/apikey)
> - **OpenAI** → [platform.openai.com/api-keys](https://platform.openai.com/api-keys)
> - **Anthropic** → [console.anthropic.com/settings/keys](https://console.anthropic.com/settings/keys)
> - **Mistral** → [console.mistral.ai](https://console.mistral.ai/)

### 3. Generera en övning
```bash
cd Buggernaut
dotnet buggernaut generate
```

---

## Daglig användning

Allt från `Buggernaut/`-mappen:

```bash
dotnet buggernaut generate                         # generera ny övning
dotnet buggernaut generate -c LINQ -d Hard         # välj kategori och svårighetsgrad
dotnet buggernaut generate --dry-run               # testa utan API-nyckel

dotnet test exercises.slnf                         # kör dina övningstester

dotnet buggernaut hint    <ClassName>              # ledtråd när du kört fast
dotnet buggernaut explain <ClassName>              # förklaring när testerna är gröna
```

> **Tips:** `dotnet buggernaut generate --help` visar alla tillgängliga flaggor, kategorier och svårighetsgrader.

---

## Konfiguration

### Välj provider

Öppna `tools/Buggernaut.Generator/appsettings.json` och ändra `Provider`:

```json
{
  "LLM": {
    "Provider": "Gemini"
  }
}
```

Tillgängliga providers: `Gemini`, `OpenAI`, `Anthropic`, `Mistral`, `Ollama`

### Välj modell (valfritt)

```json
{
  "LLM": {
    "Provider": "Gemini",
    "Gemini":    { "Model": "gemini-2.5-flash" },
    "OpenAI":    { "Model": "gpt-4o-mini" },
    "Anthropic": { "Model": "claude-3-5-haiku-latest" },
    "Mistral":   { "Model": "mistral-small" },
    "Ollama":    { "BaseUrl": "http://localhost:11434/v1", "Model": "llama3" }
  }
}
```

> **Ollama** kräver ingen API-nyckel men behöver en lokal server igång.

---

## Kör testerna

| Kommando | Vad körs |
|---|---|
| `dotnet test exercises.slnf` | **Dina övningar** — enbart `Buggernaut.Tests` |
| `dotnet test generator.slnf` | Generatorns egna enhetstester |
| `dotnet test` | Allt |

> Kör alltid från `Buggernaut/`-mappen.
