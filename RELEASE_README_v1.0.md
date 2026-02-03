# 🧠 Nexus AI v1.0 - Initial Release

<div align="center">

**AI-powered research assistant for your documents**

*Grounded answers · Multi-provider · Artifacts · Knowledge Graph · Audio*

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![WPF](https://img.shields.io/badge/WPF-Windows-0078D4?logo=windows)](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

![NexusAI v1.0](docs/screenshot.png)

</div>

---

## 🎉 What's New in v1.0

This is the **first stable release** of Nexus AI, featuring:

### ✨ Core Features

- **🤖 Multi-AI Provider Support**
  - Google Gemini 2.0 Flash (cloud, multimodal)
  - Ollama (local LLMs: llama3, mistral, phi, etc.)
  - Runtime switching without restart

- **📚 Multi-Format Document Support**
  - PDF, DOCX, PPTX, EPUB, TXT, MD
  - Drag & drop interface
  - Concurrent document loading

- **🎨 6 Artifact Types**
  - Deep Dive (executive summary)
  - Summary (concise overview)
  - Notebook Guide (structured learning)
  - Study Guide (with practice questions)
  - FAQ (Q&A from sources)
  - Podcast Script (2-person dialogue)

- **🕸️ Knowledge Graph**
  - Visual connections between documents
  - Keyword-based relationship mapping
  - Interactive canvas rendering

- **🎙️ Text-to-Speech**
  - Read AI responses aloud
  - Integrated audio player (Play/Pause/Stop)
  - System TTS engine integration

- **🖼️ Multimodal AI (Images)**
  - Drag & drop images into chat
  - Visual analysis with Gemini 2.0
  - Multi-image comparison

- **📝 Obsidian Integration**
  - Import from vault (with subfolder support)
  - Export chat + artifacts with backlinks
  - Markdown-compatible output

### 🏗️ Architecture Highlights

- **Clean Architecture** (Robert C. Martin)
  - 4-layer separation (Domain, Application, Infrastructure, Presentation)
  - Strict dependency rules enforced by project structure
  - SOLID principles throughout

- **Design Patterns**
  - Use Cases (Command/Handler pattern)
  - Factory Pattern for AI provider switching
  - Strategy Pattern for document parsers
  - Railway Oriented Programming (`Result<T>` monad)
  - Interface Segregation Principle

- **Code Quality**
  - Strongly-typed IDs (`ChatMessageId`, `SourceDocumentId`)
  - Immutable records for domain models
  - Nullable reference types enabled
  - Thread-safe concurrent collections

---

## 📦 Download & Installation

### System Requirements

- **OS:** Windows 10/11 (x64)
- **.NET 8 Runtime** ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- **Gemini API Key** ([Get it here](https://aistudio.google.com/apikey)) — Free tier available
- **(Optional) Ollama** ([Download](https://ollama.ai/)) for local LLM support

### Installation Steps

1. **Download** `NexusAI-v1.0.0-win-x64.zip` from assets below
2. **Extract** to any folder (e.g., `C:\Program Files\NexusAI`)
3. **Run** `NexusAI.exe`
4. **Enter** your Gemini API key in the header
5. **Add documents** and start chatting!

---

## 🚀 Quick Start Guide

### 1. Setup AI Provider

**Cloud (Gemini):**
- Paste your API key in the `🔑 Gemini API Key` field
- Model: `gemini-2.0-flash` (auto-configured)

**Local (Ollama):**
- Install Ollama: `winget install Ollama.Ollama`
- Pull a model: `ollama pull llama3`
- Select **Local (Ollama)** from dropdown
- Choose your model from the list

### 2. Add Your Documents

**Method 1:** Click **ADD DOCUMENTS** button
**Method 2:** Drag & drop files to sidebar
**Method 3:** Sync Obsidian vault

Supported formats: `.pdf`, `.docx`, `.pptx`, `.epub`, `.txt`, `.md`

### 3. Ask Questions

Type in chat input → AI answers **only** from your documents → Citations appear as `[filename]`

**Example:**
```
You: What are the key findings?
AI: The study found three main results [research_paper.pdf]:
1. X increased by 40%
2. Y showed correlation with Z [analysis.docx]
...
```

### 4. Generate Artifacts

Switch to **🎨 Artifacts** tab → Select artifact type → Click **GENERATE**

Use cases:
- **Deep Dive**: Before important meeting
- **Study Guide**: For exam prep
- **FAQ**: For documentation
- **Podcast Script**: For content creation

### 5. Explore Knowledge Graph

Switch to **🕸️ Graph** tab → See visual connections between documents → Click **REFRESH GRAPH** to rebuild

---

## 🎨 Screenshots

### Main Interface - 3-Pane Layout
*Chat center · Documents left · Artifacts/Graph right*

### Knowledge Graph Visualization
*Visual connections between documents based on shared keywords*

### Artifact Generation
*One-click generation of study guides, FAQs, summaries*

### Multimodal Chat
*Drag images into chat for visual analysis with Gemini 2.0*

---

## 🛠️ Build from Source

```bash
# Clone repository
git clone https://github.com/yourusername/NexusAI.git
cd NexusAI

# Restore dependencies (multi-project solution)
dotnet restore NexusAI.sln

# Build all projects
dotnet build NexusAI.sln --configuration Release

# Run Presentation project
dotnet run --project src/NexusAI.Presentation/NexusAI.Presentation.csproj
```

**Or open `NexusAI.sln` in Visual Studio 2022+ and press F5.**

### Project Structure

```
src/
├── NexusAI.Domain/           # Core (0 dependencies)
├── NexusAI.Application/      # Use Cases + Interfaces
├── NexusAI.Infrastructure/   # AI Services + Parsers
└── NexusAI.Presentation/     # WPF UI + ViewModels
```

---

## 🔒 Security & Privacy

### Privacy Modes

- **Local (Ollama)**: 100% offline, zero data leaves your machine
- **Cloud (Gemini)**: Subject to [Google's privacy policy](https://ai.google.dev/gemini-api/terms)

### Security Features

- API keys stored in memory only (not persisted to disk)
- Thread-safe concurrent collections
- Nullable reference types (no null-related crashes)
- Defensive null checks for all external APIs

---

## 📖 Documentation

- **Main README**: [README.md](README.md)
- **Contributing Guide**: [CONTRIBUTING.md](CONTRIBUTING.md)
- **Release Notes**: [RELEASE_NOTES.md](RELEASE_NOTES.md)
- **Architecture**: See README.md → Architecture section

---

## 🐛 Known Issues

- ⚠️ Auto-scroll to latest message not implemented (manual scroll required)
- ⚠️ No persistence (chat history lost on app restart) — planned for v1.1
- ⚠️ Large PDF files (>100MB) may take time to parse

**Workarounds:**
- Use smaller PDF chunks if possible
- Export important chats to Obsidian before closing

---

## 🛣️ Roadmap (v1.1 and Beyond)

### v1.1 (Next Release)
- [ ] Auto-scroll to latest chat message
- [ ] Search within sources (full-text)
- [ ] Source preview modal (quick view without opening file)
- [ ] Token usage display + cost estimation

### v2.0 (Future)
- [ ] SQLite persistence (sources, chat history)
- [ ] Multiple conversation threads
- [ ] Export to PDF/Word
- [ ] Vector embeddings for semantic search
- [ ] OCR support for scanned PDFs

---

## 🤝 Contributing

We welcome contributions! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

### Code Style Requirements

- **Clean Architecture** - respect dependency rules (no Application → Infrastructure)
- **Use Cases** - one command/query per file, with handler
- **Railway Oriented Programming** - always return `Result<T>`, never throw in business logic
- Use modern C# 12 features (records, primary constructors, collection expressions)
- Strongly-typed IDs for entities
- Immutable by default

---

## 📄 License

MIT License - see [LICENSE](LICENSE) for details.

---

## 🙏 Acknowledgments

- **Google** for Gemini 2.0 Flash API
- **Ollama** for local LLM runtime
- **MaterialDesignInXaml** for beautiful UI components
- **iText** for PDF parsing
- **CommunityToolkit** for MVVM helpers

---

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/yourusername/NexusAI/issues)
- **Discussions**: [GitHub Discussions](https://github.com/yourusername/NexusAI/discussions)
- **Email**: your.email@example.com

---

<div align="center">

**Built with ❤️ using .NET 8 and Clean Architecture**

*Nexus AI — Your intelligent research companion*

### ⭐ If you find this useful, please star the repository!

[Download Latest](https://github.com/yourusername/NexusAI/releases/latest) · [Documentation](https://github.com/yourusername/NexusAI) · [Report Bug](https://github.com/yourusername/NexusAI/issues)

---

**Enjoy Nexus AI v1.0!** 🎉

</div>
