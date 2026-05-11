# Contributing to Buggernaut

Thanks for contributing.
This guide is intentionally short so it is easy to follow.

## Quick checklist

- Create a branch from `main`
- Keep the change focused
- Run relevant tests
- Open a pull request with clear context

---

## Branch naming

Use one of these patterns:

- `feat/<short-description>`
- `fix/<short-description>`
- `docs/<short-description>`
- `test/<short-description>`
- `chore/<short-description>`

Examples:
- `feat/add-new-challenge-category`
- `fix/handle-missing-provider`
- `docs/clarify-readme-setup`

---

## Commit messages

Use short, descriptive commit messages.

Suggested style:
- `feat: add support for ...`
- `fix: handle null ...`
- `docs: update setup instructions`

---

## Before opening a PR

Run from `Buggernaut/`:

```bash
dotnet test
```

If your change is docs-only, verify links and commands still make sense.

---

## Pull request template

Please include:

1. What changed
2. Why it changed
3. How it was tested

Use this checklist in your PR description:

- [ ] Change is focused and small
- [ ] Documentation updated if needed
- [ ] Relevant tests pass locally
- [ ] No secrets or keys committed

---

## Scope and quality

- Prefer small, focused PRs over large mixed changes.
- If you plan a bigger change, open an issue first so we can align.

---

## Need help?

Open an issue with context and what you are trying to achieve.

