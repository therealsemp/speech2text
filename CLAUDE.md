# CLAUDE.md — Référence projet speech2text

## Fichiers de référence
- **Specs fonctionnelles** : `docs/specs.md`
- **Architecture technique** : `docs/architecture.md`
- **Roadmap & tâches** : `docs/roadmap.md`

> Consulter ces fichiers avant toute implémentation.

## Stack
C# 13 / .NET 10 — WPF — self-contained exe

## Build & run

```bash
# Build (depuis la racine du repo)
dotnet build speech2text.sln

# Tests
dotnet test speech2text.sln
```

- La solution est `speech2text.sln` à la **racine** du repo (pas dans `src/`)
- `src/` contient les projets mais pas de fichier solution

## Statut du projet
Phase : **Prêt pour le développement**
Prochaine étape : Phase 1 — Bootstrap (voir `docs/roadmap.md`)
