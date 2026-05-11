# Buggernaut

### Guide
##### API-nyckel till LLM
1. I `appsettings.json` finns en lista över LLM-providers och deras modeller. Vill du specificera ytterligare vilken motor du vill använda dig av så ändrar du namnet i den filen.  
2. Sedan kör `dotnet user-secrets set "{din-LLM-provider}:ApiKey" "din-nyckel"` innan du startar generatorn för att spara din API-nyckel till programmet.   
> Exempel:  
> - `dotnet user-secrets set "LLM:Gemini:ApiKey" "din-nyckel"`  
> - `dotnet user-secrets set "LLM:OpenAI:ApiKey" "din-nyckel"`  
> - `dotnet user-secrets set "LLM:Mistral:ApiKey" "din-nyckel"`  