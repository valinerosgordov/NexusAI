# NEXUS AI

<div align="center">

**AI research assistant for your documents**

*Grounded answers · Citations · Obsidian · Artifacts*

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![WPF](https://img.shields.io/badge/WPF-Windows-0078D4?logo=windows)](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

</div>

---

## ✨ Features

| Feature | Description |
|--------|-------------|
| **Grounded AI** | Answers only from your uploaded sources (PDF, MD). No hallucination — if the answer isn't in your docs, the AI says so. |
| **Citations** | Every claim is linked to a source. See *Sources: [doc_name]* under each answer. |
| **Obsidian** | Load notes from your vault (path + optional subfolder). Export chat or artifacts to `AI_Notebook` in Obsidian. |
| **Artifacts** | Generate **Notebook Guide**, **FAQ**, **Study Guide**, **Summary**, **Outline**, **Podcast script** from your sources. |
| **Chat** | Ask questions in natural language. Context is built from all included sources (up to large token limits). |

---

## 🛠 Tech stack

- **.NET 8** · **WPF** · **C# 12**
- **Google Gemini** (e.g. `gemini-2.0-flash`) for chat and artifacts
- **iText 7** for PDF parsing
- **CommunityToolkit.Mvvm** · **Microsoft.Extensions.DependencyInjection**
- **Roboto** (embedded) · dark theme · clean UI

---

## 📦 Requirements

- **Windows 10/11**
- **.NET 8 SDK**
- **Gemini API key** from [Google AI Studio](https://aistudio.google.com/apikey)

---

## 🚀 Quick start

1. **Clone**  
   ```bash
   git clone https://github.com/your-username/NexusAI.git
   cd NexusAI
   ```

2. **Build**  
   ```bash
   dotnet restore
   dotnet build
   ```

3. **Run**  
   ```bash
   dotnet run
   ```
   Or open `NexusAI.sln` in Visual Studio and run.

4. **Configure**
   - Enter your **Gemini API key** in the header (🔑 API Key).
   - (Optional) Set **Obsidian vault path** in the left panel and use **Sync Vault** to load `.md` notes.

5. **Use**
   - Add PDFs via **Add PDF** or drag & drop.
   - Ask questions in the chat; answers are grounded in your sources with citations.
   - Create artifacts (Notebook Guide, FAQ, etc.) on the **Artifacts** tab.
   - Export chat or artifacts to Obsidian with **Сохранить диалог в Obsidian** / **Экспорт в Obsidian**.

---

## 📁 Project structure

```
NexusAI/
├── Application/          # Services, interfaces
├── Domain/               # Models, Result
├── Infrastructure/        # Gemini, PDF, Obsidian
├── Presentation/         # ViewModels, Converters
├── Resources/             # Styles, Strings, Fonts (Roboto)
├── App.xaml
├── MainWindow.xaml
└── README.md
```

---

## 📄 License

MIT — see [LICENSE](LICENSE) for details.

---

<div align="center">

*Built with .NET and Gemini*

</div>
