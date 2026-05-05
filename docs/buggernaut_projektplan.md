# Projektplan

## Buggernaut

Inspiration från [PrimeAgens KataMachine.](https://github.com/ThePrimeagen/kata-machine)

Ett REPL-program som ska anropa en LLM genom API som genererar en C#-utmaning/BlackBox-gåta. Den ska returnera ett bestämt JSON-format som sedan parseas med try/catch-logik och fallback-meddelande. 

- Ska jag ha en `--dry-run` med en hårdkodad mock-utmaning som demo utan API-anrop?

- Ska man ha en config-fil som man kan skriva in sin valda LLMs address och API-nyckel så man inte är Gemini-bunden?

## Koncept

Användaren anger något i stil med `dotnet run -- generate --category null-bug --difficulty easy`. Generatorn anropar Gemini (eller valfri LLM), får tillbaka strukturerad JSON, sedan scaffoldar den utmaningen som den fått som svar. 

```md
BuggerNaut/
|-- src/
|     |-- Exercises/
|         |-- ReverseString/
|             |-- ReverseString.cs  // användaren redigerar här
|-- tests/
|     |-- KataMachine.Tests/
|         |-- ReverseStringTests.cs // kör med `dotnet test`
|-- solutions/
|     |-- ReverseString.cs          // dolt facit (genereras men visas inte)
|-- tools/
     |-- Generator/
         |-- Program.cs             // CLI-verktyget
```

En utmaning kan jag tänka mig blir att få konsekvent, parsebar output från LLM utan hallucinationer. Lösningen är kanske att ge den väldigt avskalade och strukturerad JSON-output som den ska svara med?

##### Exempel-prompt från programmet:

```md
You are a C# coding challenge generator for junior developers.
Respond ONLY with valid JSON, no markdown, no explanation outside the JSON.

{
 "title": "string",
 "description": "string",
 "buggy_code": "string (C# code with intentional bug)",
 "hint": "string",
 "solution_code": "string (correct C# code)",
 "explanation": "string"
}



Rules:
- exercise_code: A C# class named {class_name} with ONE intentional bug.
  Must compile. Include a TODO comment near the bug.
- test_code: xUnit tests for class {class_name}. Import xunit. 
  At least 3 [Fact] tests. Tests must FAIL on buggy code, PASS on fixed code.
- solution_code: The corrected version of exercise_code.
- All code must be valid, self-contained C# (no using directives beyond System).

```

##### User-prompt:

```
Generate a C# bug-fixing challenge.
Category: NullReferenceException
Difficulty: Easy
```

Lägg en `JsonSerializer.Deserialize<ChallengeKlass>(response)` på svaret och jag borde vara igång.

## Brainstorm

#### Utmaningskategorier som passar juniorer

Bra att ha dessa som valbara:

* **Buggar** (null reference, off-by-one, fel loop-villkor, saknad `await`, cast-fel)
* **Vad skriver detta ut?** (output-gissning)
* **Fyll i det saknade** (komplettera en metod)
* **Algoritmgåta** (t.ex. "implementera FizzBuzz utan modulo")
* **LINQ-pussel** (fixa eller skriv en query)
* **Blackbox-pussel** (Få en input eller fil, ge en förbestämd output)

#### Saker att tänka på

**Do's:**

* Validera Geminis JSON-svar (try/catch + fallback-meddelande)
* Ha en `--dry-run` eller hårdkodad mock-utmaning för demo utan API-anrop
* Håll promptarna korta, LLM hallucinerar mindre med tydlig struktur

**Dont's:**

* Att låta användaren köra genererad kod (känns som en säkerhetsrisk)
* Scope creep, håll fokus på kärnfunktionalitet (frontend, inloggning, databas, leaderboard är bara bös)
* Säkerställ att lösningen inte visas för användaren, det dödar poängen av programmet.

**Bonus:**

* Spara avklarade utmaningar i en lokal JSON-fil
* Difficulty-adaptivitet (klarar man 3 i rad, höj svårigheten?)

**Gemini är opålitlig med kompilerbar kod**, så jag kommer behöva:

* Validera att JSON parsar korrekt (try/catch).
* Helst: anropa `dotnet build` programmatiskt efter scaffold och varna om det misslyckas.
* Ha en retry-loop (max 2 försök) om svaret är skräp.

**Håll namespace-konventionen konsekvent**, annars hittar inte testprojektet klassen. Skicka med namespace i prompten.

**`.gitignore` solutions-mappen** om jag vill att det ska vara "ärligt". Eller ge en `--reveal`-flagga som skriver ut lösningen i terminalen.




































