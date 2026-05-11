# Security Policy

## Reporting a vulnerability

If you find a security vulnerability in this project, please **do not open a public issue**.

Instead, report it privately through one of these channels:

- **GitHub Security Advisories**: [Report a vulnerability](https://github.com/discovicke/buggernaut/security/advisories/new)
- **Email**: [johanssonviktor@pm.me](mailto:johanssonviktor@pm.me)

Include as much context as possible:
- What the vulnerability is
- How it can be triggered or reproduced
- Any potential impact

I'll aim to respond within a few days and keep you updated on the fix.

---

## API keys and secrets

Buggernaut uses `dotnet user-secrets` to store API keys locally. Keys are **never** committed to Git.

If you accidentally commit a key, rotate it immediately with your provider:

- Gemini: <https://aistudio.google.com/app/apikey>
- OpenAI: <https://platform.openai.com/api-keys>
- Anthropic: <https://console.anthropic.com/settings/keys>
- Mistral: <https://console.mistral.ai/>

---

## Scope

This project is a local CLI tool. It does not run a server, expose any endpoints, or collect any data.
The only outbound traffic is the request to the LLM provider API you configure yourself.

