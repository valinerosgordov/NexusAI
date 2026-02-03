# GitHub Repository Setup Guide

Пошаговая инструкция по настройке репозитория NexusAI на GitHub.

---

## 1️⃣ Настройка секции "About"

### Где находится

На главной странице репозитория → правый верхний угол → **⚙️ (шестеренка) рядом с "About"**

### Что заполнить

**Description:**
```
AI-powered research assistant for your documents. Grounded answers, multi-provider support (Gemini/Ollama), artifacts generation, knowledge graph, and Obsidian integration.
```

**Website:**
```
https://github.com/yourusername/NexusAI
```

**Topics (теги):**
```
ai
artificial-intelligence
dotnet
csharp
wpf
gemini
ollama
llm
rag
retrieval-augmented-generation
knowledge-graph
document-parser
pdf-parser
obsidian
text-to-speech
wpf-application
clean-architecture
csharp-12
dotnet-8
mvvm
```

**Галочки:**
- ☑️ Include in the home page (показывать на главной)

---

## 2️⃣ Создание релиза v1.0

### Шаг 1: Перейти в Releases

GitHub → Ваш репозиторий → **Releases** (правая панель) → **Draft a new release**

### Шаг 2: Заполнить форму релиза

**Tag version:**
```
v1.0.0
```

**Target:** `main` (или ваша основная ветка)

**Release title:**
```
🧠 Nexus AI v1.0 - Initial Release
```

**Description:**

Скопируйте содержимое файла `RELEASE_README_v1.0.md` или используйте краткую версию:

```markdown
# 🧠 Nexus AI v1.0 - Initial Release

**AI-powered research assistant for your documents**

## ✨ What's New

First stable release featuring:

- 🤖 **Multi-AI Support**: Gemini 2.0 Flash + Ollama (local LLMs)
- 📚 **Multi-Format Docs**: PDF, DOCX, PPTX, EPUB, TXT, MD
- 🎨 **6 Artifact Types**: Deep Dive, Summary, Study Guide, FAQ, Podcast Script, Notebook Guide
- 🕸️ **Knowledge Graph**: Visual connections between documents
- 🎙️ **Text-to-Speech**: Read AI responses aloud
- 🖼️ **Multimodal AI**: Drag images into chat (Gemini 2.0)
- 📝 **Obsidian Integration**: Import/export with backlinks
- 🏗️ **Clean Architecture**: SOLID, Use Cases, Railway Oriented Programming

## 📦 Download

**System Requirements:**
- Windows 10/11 (x64)
- .NET 8 Runtime ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- Gemini API Key ([Free](https://aistudio.google.com/apikey))

**Installation:**
1. Download `NexusAI-v1.0.0-win-x64.zip` below
2. Extract to any folder
3. Run `NexusAI.exe`
4. Enter Gemini API key
5. Add documents and start chatting!

## 🚀 Quick Start

1. **Setup AI**: Paste Gemini API key OR install Ollama (`ollama pull llama3`)
2. **Add Docs**: Click "ADD DOCUMENTS" or drag & drop files
3. **Ask Questions**: AI answers only from your documents with citations
4. **Generate Artifacts**: Switch to Artifacts tab → select type → generate
5. **Explore Graph**: Visual connections between documents

## 🔒 Privacy

- **Local (Ollama)**: 100% offline, zero data transmission
- **Cloud (Gemini)**: Subject to Google's privacy policy

## 📖 Documentation

- [Full README](https://github.com/yourusername/NexusAI)
- [Contributing Guide](https://github.com/yourusername/NexusAI/blob/main/CONTRIBUTING.md)
- [Architecture Details](https://github.com/yourusername/NexusAI#-architecture)

## 🐛 Known Issues

- No persistence (chat history lost on restart) — planned for v1.1
- Large PDFs (>100MB) may be slow to parse

## 🛣️ Roadmap

**v1.1:**
- Auto-scroll to latest message
- Full-text search in sources
- Token usage display

**v2.0:**
- SQLite persistence
- Multiple conversation threads
- Vector embeddings for semantic search

---

**Built with ❤️ using .NET 8 and Clean Architecture**

⭐ **Star the repo if you find it useful!**
```

### Шаг 3: Прикрепить файлы (Assets)

**Что прикрепить:**

1. **Compiled Release** (если есть):
   - `NexusAI-v1.0.0-win-x64.zip` (exe + dll + dependencies)
   
2. **Source Code** (GitHub прикрепит автоматически):
   - `Source code (zip)`
   - `Source code (tar.gz)`

**Как создать ZIP для релиза:**

```powershell
# Если у вас есть release-build.ps1
.\release-build.ps1

# Или вручную
dotnet publish src/NexusAI.Presentation/NexusAI.Presentation.csproj `
  -c Release `
  -r win-x64 `
  --self-contained false `
  -o publish/

# Создать ZIP
Compress-Archive -Path publish/* -DestinationPath NexusAI-v1.0.0-win-x64.zip
```

### Шаг 4: Опубликовать

- ☑️ **Set as the latest release** (отметить как последний релиз)
- Нажать **Publish release**

---

## 3️⃣ Настройка README.md на главной странице

Файл `README.md` в корне репозитория автоматически отображается на главной странице GitHub.

**Текущий README уже настроен:**
- ✅ Badges (shields.io)
- ✅ Screenshots
- ✅ Feature table
- ✅ Architecture diagram
- ✅ Installation instructions
- ✅ Usage guide

**Дополнительные улучшения (опционально):**

### Добавить GitHub Actions badge (если настроите CI/CD)

```markdown
[![Build](https://github.com/yourusername/NexusAI/actions/workflows/build.yml/badge.svg)](https://github.com/yourusername/NexusAI/actions)
```

### Добавить GitHub Release badge

```markdown
[![Release](https://img.shields.io/github/v/release/yourusername/NexusAI)](https://github.com/yourusername/NexusAI/releases)
[![Downloads](https://img.shields.io/github/downloads/yourusername/NexusAI/total)](https://github.com/yourusername/NexusAI/releases)
```

### Добавить Contributors badge

```markdown
[![Contributors](https://img.shields.io/github/contributors/yourusername/NexusAI)](https://github.com/yourusername/NexusAI/graphs/contributors)
```

---

## 4️⃣ Настройка GitHub Pages (опционально)

Если хотите создать отдельный сайт с документацией:

1. Settings → Pages
2. Source: **Deploy from a branch**
3. Branch: `main` → `/docs` folder
4. Save

Затем создайте `docs/index.md` с красивой документацией.

---

## 5️⃣ Настройка Issue Templates

Создайте `.github/ISSUE_TEMPLATE/`:

### Bug Report (`bug_report.md`)

```markdown
---
name: Bug Report
about: Report a bug or unexpected behavior
title: '[BUG] '
labels: bug
---

**Describe the bug**
A clear description of what the bug is.

**To Reproduce**
Steps to reproduce:
1. Go to '...'
2. Click on '...'
3. See error

**Expected behavior**
What you expected to happen.

**Screenshots**
If applicable, add screenshots.

**Environment:**
 - OS: [e.g. Windows 11]
 - .NET Version: [e.g. .NET 8.0]
 - NexusAI Version: [e.g. v1.0.0]

**Additional context**
Any other context about the problem.
```

### Feature Request (`feature_request.md`)

```markdown
---
name: Feature Request
about: Suggest a new feature
title: '[FEATURE] '
labels: enhancement
---

**Is your feature request related to a problem?**
A clear description of the problem.

**Describe the solution you'd like**
What you want to happen.

**Describe alternatives you've considered**
Alternative solutions or features.

**Additional context**
Any other context or screenshots.
```

---

## 6️⃣ Настройка Pull Request Template

Создайте `.github/pull_request_template.md`:

```markdown
## Description

Brief description of changes.

## Type of Change

- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Checklist

- [ ] Code follows Clean Architecture principles
- [ ] All layers respect dependency rules
- [ ] Used `Result<T>` for error handling (no exceptions in business logic)
- [ ] Added/updated tests
- [ ] Updated README/documentation if needed
- [ ] No nullable reference warnings
- [ ] Strongly-typed IDs used where appropriate

## Testing

Describe how you tested your changes.

## Screenshots (if applicable)

Add screenshots for UI changes.
```

---

## 7️⃣ GitHub Repository Settings

### General

- ✅ **Features:**
  - ☑️ Issues
  - ☑️ Discussions (для Q&A)
  - ☐ Projects (если не нужно)
  - ☐ Wiki (если не нужно)

- ✅ **Pull Requests:**
  - ☑️ Allow squash merging
  - ☑️ Automatically delete head branches

### Branches

**Branch protection rule для `main`:**

1. Settings → Branches → Add rule
2. Branch name pattern: `main`
3. Настройки:
   - ☑️ Require pull request before merging
   - ☑️ Require approvals (1)
   - ☑️ Dismiss stale pull request approvals

---

## 8️⃣ Чек-лист перед публикацией

- [ ] README.md обновлен с актуальной информацией
- [ ] Секция "About" заполнена (description, website, topics)
- [ ] Создан релиз v1.0 с описанием и бинарниками
- [ ] LICENSE файл присутствует (MIT)
- [ ] CONTRIBUTING.md содержит guidelines
- [ ] .gitignore настроен (bin/, obj/, .vs/, .user)
- [ ] Issue templates созданы
- [ ] Pull request template создан
- [ ] Branch protection включен для main
- [ ] GitHub Actions настроен (опционально)

---

## 9️⃣ Продвижение релиза

После публикации релиза:

### Reddit

- r/dotnet
- r/csharp
- r/opensource
- r/AI_tools

**Пример поста:**

```
[Project] Nexus AI v1.0 - AI-powered research assistant built with .NET 8 and Clean Architecture

I've just released v1.0 of Nexus AI, an open-source WPF app for chatting with your documents.

Key features:
• Multi-AI support (Gemini 2.0 + Ollama for 100% local mode)
• Multi-format docs (PDF, DOCX, EPUB, etc.)
• Artifact generation (study guides, FAQs, summaries)
• Knowledge graph visualization
• Obsidian integration

Tech stack: .NET 8, C# 12, Clean Architecture, SOLID, Railway Oriented Programming, MVVM

GitHub: [link]
License: MIT

Would love your feedback!
```

### Hacker News / Product Hunt

- Краткое описание
- Демо GIF/видео
- Ссылка на GitHub

### Twitter/X

```
🧠 Just shipped Nexus AI v1.0!

AI research assistant for your docs:
✅ Gemini 2.0 + Ollama support
✅ Clean Architecture + SOLID
✅ 100% local mode available
✅ MIT licensed

Built with #dotnet #csharp #AI

GitHub: [link]
```

---

**Готово!** Теперь у вас есть полная инструкция по настройке GitHub репозитория. 🚀
