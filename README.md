# 🧠 Nexus AI

<div align="center">

**AI-powered research assistant for your documents**

*Grounded answers · Multi-provider · Artifacts · Knowledge Graph · Audio*

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![WPF](https://img.shields.io/badge/WPF-Windows-0078D4?logo=windows)](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Version](https://img.shields.io/badge/Version-1.0.0-brightgreen)](https://github.com/yourusername/NexusAI/releases)

![NexusAI Screenshot](docs/screenshot.png)

</div>

---

## ✨ Features

### 🎯 Core Capabilities

| Feature | Description |
|---------|-------------|
| **🤖 Multi-AI Support** | Switch between **Gemini 2.0 Flash** (Cloud) and **Ollama** (Local). Full privacy mode with local LLMs. |
| **📚 Multi-Format Documents** | Support for **PDF**, **DOCX**, **PPTX**, **EPUB**, **TXT**, **MD** files with Strategy Pattern parsers. |
| **🎨 Artifacts Generator** | Create **Study Guide**, **FAQ**, **Notebook Guide**, **Summary**, **Podcast Script** from your sources. |
| **🕸️ Knowledge Graph** | Visual graph connecting documents by shared keywords and themes. |
| **🎙️ Text-to-Speech** | Read AI responses aloud with integrated audio player (Play/Pause/Stop). |
| **🖼️ Multimodal AI** | Drop images into chat for visual analysis (Gemini 2.0 multimodal support). |
| **📝 Obsidian Integration** | Import from vault (with subfolder support), export chat/artifacts with backlinks. |
| **🔒 Strict RAG** | Answers **only** from your documents. No hallucination — cites sources with `[filename]`. |

### 🎨 Modern UI

- **Dark Theme** with Apple-inspired color palette (purple accents)
- **3-Pane Layout**: Documents sidebar, Chat center, Artifacts/Graph right panel
- **Smooth Animations**: Hover effects, fade-in, scale transforms
- **Material Design**: Icons, cards, elevation shadows
- **Drag & Drop**: Files and images

---

## 🛠️ Tech Stack

### Core
- **.NET 8** with **C# 12** (Primary Constructors, Collection Expressions, Records)
- **WPF** + **MaterialDesignInXamlToolkit** for modern UI
- **CommunityToolkit.Mvvm** for MVVM pattern
- **Microsoft.Extensions.DependencyInjection** for IoC

### AI Providers
- **Google Gemini 2.0 Flash** (multimodal, strict RAG, system instruction)
- **Ollama** (local LLMs: llama3, mistral, etc.)

### Document Parsing
- **iText7** (PDF)
- **DocumentFormat.OpenXml** (DOCX, PPTX)
- **VersOne.Epub** (EPUB)
- Native support for TXT/MD

### Audio & Graph
- **System.Speech.Synthesis** for TTS
- Custom **Canvas-based Knowledge Graph** rendering

---

## 📦 Requirements

- **OS:** Windows 10/11 (x64)
- **.NET 8 Runtime** ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- **Gemini API Key** ([Get it here](https://aistudio.google.com/apikey)) — **Free tier available**
- **(Optional) Ollama** ([Download](https://ollama.ai/)) for local LLM support

---

## 🚀 Quick Start

### Option 1: Download Release (Easiest)

1. Download `NexusAI-v1.0.0-win-x64.zip` from [Releases](https://github.com/yourusername/NexusAI/releases)
2. Extract and run `NexusAI.exe`
3. Enter your **Gemini API Key** in the header
4. Add documents and start chatting!

### Option 2: Build from Source

```bash
# Clone repository
git clone https://github.com/yourusername/NexusAI.git
cd NexusAI

# Restore dependencies
dotnet restore

# Build
dotnet build --configuration Release

# Run
dotnet run --project NexusAI.csproj
```

**Or open `NexusAI.sln` in Visual Studio 2022+ and press F5.**

---

## 📖 Usage Guide

### 1️⃣ Setup AI Provider

**Cloud (Gemini):**
- Paste your API key in the `🔑 Gemini API Key` field
- Model: `gemini-2.0-flash` (auto-configured)

**Local (Ollama):**
- Install Ollama and pull a model: `ollama pull llama3`
- Select **Local (Ollama)** from dropdown
- Choose your model from the list

### 2️⃣ Add Documents

**Method 1: Click "ADD DOCUMENTS"**
- Supports: `.pdf`, `.docx`, `.pptx`, `.epub`, `.txt`, `.md`

**Method 2: Drag & Drop**
- Drag files directly to the left sidebar

**Method 3: Obsidian Vault**
- Set vault path (e.g., `C:\Users\You\Documents\Obsidian\MyVault`)
- *(Optional)* Specify subfolder (e.g., `Research/AI`)
- Click **Sync Vault**

### 3️⃣ Chat with Your Documents

- Type question in the input box
- AI responds based **only** on included documents
- Citations appear as `[filename]` in responses
- Click citation to highlight source in sidebar

**Example:**
```
You: What are the key findings in the research?
AI: The study found three main results [research_paper.pdf]:
1. X increased by 40%
2. Y showed correlation with Z [analysis.docx]
3. ...
```

### 4️⃣ Generate Artifacts

Switch to **🎨 Artifacts** tab and choose:

| Artifact | Use Case |
|----------|----------|
| **Deep Dive** | Executive summary + key concepts + connections |
| **Summary** | Concise overview (2-4 paragraphs) |
| **Notebook Guide** | Russian-language structured guide |
| **Study Guide** | Learning objectives + practice questions |
| **FAQ** | 10-15 Q&A pairs from sources |
| **Podcast Script** | 2-person dialogue format |

### 5️⃣ Explore Knowledge Graph

Switch to **🕸️ Graph** tab:
- Each document = circular node
- Shared keywords = edges between nodes
- Click **REFRESH GRAPH** to rebuild

### 6️⃣ Audio Playback

- Right-click any AI message → **Read Aloud**
- Or use Play/Pause/Stop controls above chat input
- Voice: System default TTS engine

### 7️⃣ Multimodal (Images)

- Drag `.jpg`, `.png` images to chat input area
- Drop multiple images for comparison
- AI analyzes images in context of your documents

---

### Key Patterns

- **Strategy Pattern**: Document parsers (`IDocumentParser`)
- **Railway Oriented Programming**: `Result<T>` monad for error handling
- **Dependency Injection**: Constructor injection throughout
- **MVVM**: `RelayCommand`, `ObservableProperty` via CommunityToolkit
- **Thread-Safe Collections**: `ConcurrentBag<T>` + locks for shared state
- **IDisposable**: Proper resource cleanup (audio, HTTP clients)

---

## 🔒 Security & Best Practices

✅ **Implemented:**
- API keys in HTTP headers (not URL query params)
- Thread-safe collections with locks
- Defensive null checks for API responses
- Scoped lifetime for `IDisposable` services
- `ConfigureAwait(false)` in all async library code

🔐 **Privacy:**
- Local mode (Ollama) → **zero** data leaves your machine
- Gemini API → subject to Google's privacy policy ([Read here](https://ai.google.dev/gemini-api/terms))

---

## 📁 Project Structure

```
NexusAI/
├── Application/
│   ├── Interfaces/          # IAiService, IDocumentParser, etc.
│   └── Services/            # KnowledgeHubService, KnowledgeGraphService
├── Domain/
│   ├── Models/              # ChatMessage, SourceDocument, AiResponse
│   └── Result.cs            # Railway-oriented error handling
├── Infrastructure/
│   ├── Services/            # GeminiAiService, OllamaService, Parsers
│   └── DependencyInjection.cs
├── Presentation/
│   ├── ViewModels/          # MainViewModel, ChatMessageViewModel
│   └── Converters/          # WPF value converters
├── App.xaml                 # MaterialDesign theme setup
├── MainWindow.xaml          # 3-pane layout
└── NexusAI.csproj           # .NET 8 project
```

---

## 🧪 Testing

### Manual Testing Checklist

- [ ] Add documents (all formats: PDF, DOCX, EPUB, TXT, MD)
- [ ] Switch AI providers (Gemini ↔ Ollama)
- [ ] Ask questions with citations
- [ ] Generate all artifact types
- [ ] Refresh knowledge graph
- [ ] Play audio (TTS)
- [ ] Drop images for multimodal analysis
- [ ] Export to Obsidian
- [ ] Concurrent document loading (thread safety)

### Recommended Unit Tests (Future)

```csharp
// Example test structure
[Fact]
public async Task AddDocumentAsync_ConcurrentCalls_ShouldNotCorruptState()
{
    // Arrange
    var service = CreateKnowledgeHubService();
    var tasks = Enumerable.Range(0, 100)
        .Select(i => service.AddDocumentAsync($"test{i}.pdf"));
    
    // Act
    await Task.WhenAll(tasks);
    
    // Assert
    service.Sources.Count.Should().Be(100);
}
```

---

## 🛣️ Roadmap

### v1.1 (Planned)
- [ ] Auto-scroll to latest chat message
- [ ] Search within sources (full-text)
- [ ] Source preview modal (quick view)
- [ ] Token usage display + cost estimation

### v2.0 (Future)
- [ ] SQLite persistence (sources, chat history)
- [ ] Multiple conversation threads
- [ ] Export to PDF/Word
- [ ] Vector embeddings for semantic search
- [ ] OCR support for scanned PDFs

### v3.0 (Vision)
- [ ] Claude/OpenAI provider support
- [ ] Prompt templates library
- [ ] Mobile companion app
- [ ] Cloud sync (optional)

---

## 🤝 Contributing

Contributions are welcome! Please:

1. Fork the repository
2. Create feature branch: `git checkout -b feature/amazing-feature`
3. Commit changes: `git commit -m 'Add amazing feature'`
4. Push to branch: `git push origin feature/amazing-feature`
5. Open Pull Request

### Code Style

- Follow [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use modern C# 12 features (records, primary constructors, collection expressions)
- Nullable reference types enabled (`<Nullable>enable</Nullable>`)
- No `null` returns — use `Result<T>` or `Option<T>`

---

## 📄 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

```
MIT License

Copyright (c) 2024 Nexus AI Contributors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software...
```

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

## ⭐ Star History

If you find this project helpful, please consider giving it a star! ⭐

---

<div align="center">

**Built with ❤️ using .NET 8 and modern C#**

*Nexus AI — Your intelligent research companion*

[Download](https://github.com/yourusername/NexusAI/releases) · [Documentation](https://github.com/yourusername/NexusAI/wiki) · [Report Bug](https://github.com/yourusername/NexusAI/issues)

</div>
