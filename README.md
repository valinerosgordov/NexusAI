# 🧠 NexusAI

<div align="center">

**Your Second Brain. From Research to Execution.**

*Desktop-native AI workspace combining RAG Research, Project Management, and Content Creation*

[![.NET 8 LTS](https://img.shields.io/badge/.NET-8.0_LTS-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![WPF](https://img.shields.io/badge/WPF-Windows-0078D4?logo=windows&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
[![C# 12](https://img.shields.io/badge/C%23-12-239120?logo=c-sharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Gemini 2.0](https://img.shields.io/badge/Gemini-2.0_Flash-4285F4?logo=google&logoColor=white)](https://ai.google.dev/)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-blue)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Status](https://img.shields.io/badge/Status-Active-success)](https://github.com/yourusername/NexusAI)

![NexusAI Hero](docs/images/hero.png)

[🚀 Quick Start](#-quick-start) · [📸 Screenshots](#-screenshots) · [📖 Documentation](#-user-guide) · [🤝 Contributing](#-contributing)

</div>

---

## 📋 Table of Contents

- [✨ Introduction](#-introduction)
- [🚀 Key Features](#-key-features)
- [📸 Screenshots](#-screenshots)
- [🛠️ Tech Stack](#️-tech-stack)
- [🏗️ Getting Started](#️-getting-started)
- [📚 User Guide](#-user-guide)
- [🗺️ Roadmap](#️-roadmap)
- [🤝 Contributing](#-contributing)
- [📄 License](#-license)

---

## ✨ Introduction

**NexusAI** is a next-generation **desktop-native AI workspace** that transforms how you research, plan, and create. Built on **.NET 8 LTS** with **Clean Architecture**, it combines three powerful capabilities into one seamless experience:

### 🎯 **Three Pillars**

```
📚 RAG Research          →  Chat with your documents (PDF, DOCX, PPTX, EPUB)
📊 Project Management    →  AI-powered Kanban boards with GitHub integration  
🎨 Content Creation      →  Auto-generate presentations, wikis, and artifacts
```

### 🌟 **What Makes NexusAI Special**

<table>
<tr>
<td width="50%">

#### 🧠 **Dual AI Mode**

Switch between two distinct AI personalities:

- **💼 Professional Mode**: Executive assistant for business
  - Concise, action-oriented responses
  - Project management focus
  - Business terminology
  
- **🎓 Student Mode**: Socratic tutor for learning
  - Explanatory, teaching-focused
  - Study guides and concept breakdowns
  - Educational terminology

*UI instantly transforms (colors, labels, tone) on toggle!*

</td>
<td width="50%">

#### 🔒 **Hybrid AI Privacy**

**Cloud Power + Local Privacy**

```yaml
☁️ Gemini 2.0 Flash:
  - Multimodal (text + images)
  - RAG with citations
  - 15 req/min free tier

🔐 Ollama (Local):
  - 100% private
  - No internet required
  - Models: llama3, mistral, etc.
```

*Switch providers at runtime, no restart needed!*

</td>
</tr>
</table>

### 🎨 **Cyber-Noir Glass UI (2026 Standard)**

<table>
<tr>
<td width="33%">

**🌑 Void Background**

Almost pure black (`#050505`) creates infinite depth. All elements float on semi-transparent Obsidian Glass.

</td>
<td width="33%">

**⚡ Neon Plasma Accent**

Electric gradient (`#6200EA → #B500FF`) powers all interactive elements with liquid animations.

</td>
<td width="33%">

**🪟 Glassmorphism**

Frosted surfaces, super-rounded corners (16-32px), and subtle glows create tactile depth.

</td>
</tr>
</table>

**Zero instant changes** — Every element animates in (300ms slide-up), buttons glow on hover (150ms), mode switches cross-fade (500ms).

---

## 🚀 Key Features

### 🤖 **AI & Document Analysis**

| Feature | Description |
|---------|-------------|
| **📚 RAG 2.0 Pipeline** | Chat with PDF, DOCX, PPTX, EPUB, TXT, MD files using Retrieval-Augmented Generation |
| **🔍 Source Citations** | Every AI response includes `[filename.pdf]` citations linking to exact sources |
| **🖼️ Multimodal Vision** | Upload images for Gemini Vision analysis (diagrams, charts, screenshots) |
| **🎙️ Text-to-Speech** | Listen to AI responses with integrated audio player (pause/resume) |
| **🌐 Multi-Document Context** | Merge context from multiple files for comprehensive answers |
| **⚡ Streaming Responses** | Real-time token streaming with progress indicators |

<details>
<summary><b>🔧 How RAG Works</b></summary>

```
┌──────────────────────────────────────────────────┐
│  1. DOCUMENT INGESTION                           │
│     User uploads PDF/DOCX → Parser extracts text │
│     → Store in SessionContext.SourceDocuments    │
└──────────────────────────────────────────────────┘
                    ↓
┌──────────────────────────────────────────────────┐
│  2. CONTEXT BUILDING                             │
│     Merge top 3 relevant docs into prompt        │
│     Format: <Source>...[filename]...</Source>    │
└──────────────────────────────────────────────────┘
                    ↓
┌──────────────────────────────────────────────────┐
│  3. AI GENERATION                                │
│     Gemini 2.0 Flash generates response          │
│     → Extract citations via regex: \[.*?\]       │
│     → Link citations to source documents         │
└──────────────────────────────────────────────────┘
```

**Tech Stack:**
- **PDF**: iText7 (v8.0.5)
- **Office**: DocumentFormat.OpenXml (v3.2.0)
- **eBooks**: VersOne.Epub (v3.3.5)
- **AI**: Gemini 2.0 Flash, Ollama

</details>

---

### ⚡ **Actionable Output**

NexusAI doesn't just chat — it **creates real artifacts**:

<table>
<tr>
<td width="33%">

#### 🗂️ **Code Scaffolder**

```yaml
Input: "Create a REST API for blog"
Output:
  - BlogController.cs
  - BlogService.cs
  - BlogRepository.cs
  - Blog.cs (entity)
  - Folder structure

Action: Download as .zip
```

</td>
<td width="33%">

#### 🎤 **Presentation Engine**

```yaml
Input: Topic + slide count
Output:
  - Title slide
  - Content slides
  - Speaker notes
  - .pptx file

Action: Download + edit in PowerPoint
```

</td>
<td width="33%">

#### 📚 **Wiki Generator**

```yaml
Input: Documents + topic
Output:
  - Hierarchical structure
  - Chapter breakdown
  - Tagged pages
  - Markdown files

Action: Export to Obsidian vault
```

</td>
</tr>
</table>

**Other Artifacts:**
- 📖 **Deep Dive**: Executive summary + insights (Markdown)
- ❓ **FAQ**: 10-15 Q&A pairs from sources
- 📝 **Study Guide**: Learning objectives + questions
- 🎙️ **Podcast Script**: 2-person dialogue format

---

### 📊 **Smart Project Management**

<table>
<tr>
<td width="50%">

#### **🎯 AI-Powered Planning**

1. **Describe your project** in natural language
2. **AI generates** a complete task breakdown
3. **Kanban board** auto-populated with:
   - Task titles & descriptions
   - Priority levels (High/Medium/Low)
   - Role assignments (Dev/Design/Marketing)
   - Estimated hours
   - Drag-and-drop ordering

**Example:**
```
Input: "Build a mobile app for fitness tracking 
        with social features"

AI Generates:
✅ 15 tasks across 3 roles
✅ 120 estimated hours
✅ Ready-to-use Kanban board
```

</td>
<td width="50%">

#### **🔗 GitHub Integration**

- **Link repositories** via `GitHubRepoUrl` field
- **Track issues** with `GitHubIssueNumber` per task
- **Store project files** in database (`ProjectFile` entity)
- **Generate scaffolding** directly from project plan

**Kanban Features:**
- ✅ **OrderIndex** for persistent drag-and-drop sorting
- ✅ **3 status columns**: Todo / In Progress / Done
- ✅ **Priority badges** with visual indicators
- ✅ **Role-based color coding**
- ✅ **Analytics dashboard** (completion %, task distribution)
- ✅ **Floating glass cards** with tactile hover effects

</td>
</tr>
</table>

**Category Filtering:**
- 💼 **Work**: Business projects
- 🎓 **Education**: Academic subjects
- 🏠 **Personal**: Hobby projects

*Mode-aware filtering: Professional mode defaults to Work, Student mode to Education.*

---

### 🎨 **Neural Glass UI Design System**

<details>
<summary><b>🎭 2026 Design Language (Click to expand)</b></summary>

### **Color Palette: Cyber-Noir**

```css
/* Base */
--void-background:    #050505;  /* Almost pure black */
--obsidian-glass:     #CC141416; /* 80% opacity dark grey */

/* Accent: Neon Plasma Gradient */
--deep-indigo:        #6200EA;
--electric-violet:    #B500FF;

/* Text Hierarchy */
--text-primary:       #FFFFFF;  /* Pure white, Bold */
--text-secondary:     #B0B0C0;  /* Cool grey */
--text-tertiary:      #606070;  /* Subtle labels */

/* Effects */
--frosted-edge:       #33FFFFFF; /* Subtle inner glow */
```

### **Materials & Depth**

| Element | CornerRadius | Effect |
|---------|-------------|--------|
| **Windows/Modals** | `32px` | Super-rounded |
| **Cards** | `24px` | Obsidian Glass |
| **Buttons** | `16px` | Pill shapes |
| **Inputs** | `28px` | Floating capsules |

### **Glassmorphism Simulation**

Since native `BackdropFilter` blur isn't available in WPF, we simulate glass via:

1. **Semi-transparent backgrounds**: `#CC141416` (80% opacity)
2. **Colored drop shadows**: Purple glow for elevation
3. **Frosted edge borders**: `#33FFFFFF` 1px inner glow
4. **Layered transparency**: Multiple overlapping surfaces

### **Animations (The 2026 Feel)**

```yaml
Message Entrance:
  - Slide up: TranslateY +20 → 0
  - Fade in: Opacity 0 → 1
  - Duration: 300ms
  - Easing: CubicEase.EaseOut

Mode Switching:
  - Cross-fade colors
  - Duration: 500ms
  - Easing: CubicEase.EaseInOut

Hover States:
  - Scale: 1.0 → 1.05
  - Glow: 0 → 24px blur
  - Duration: 150ms

Card Lift (Drag):
  - Opacity: 1.0 → 0.85
  - TranslateY: 0 → -8px
  - Purple shadow intensifies
  - Duration: 150ms
```

### **Zone-by-Zone Implementation**

**Zone 1: Window Chrome**
- Custom draggable title bar (no standard Windows chrome)
- Obsidian Glass background blends into sidebar
- Logo uses Neon Plasma gradient
- Window controls (minimize/maximize/close) use `ChromeButton` style

**Zone 2: Chat Interface**
- User bubbles: Liquid Plasma gradient (RadialGradientBrush animated)
- AI bubbles: Obsidian Glass with white rim lighting
- Input field: Floating glass capsule with focus glow
- Messages slide up + fade in (300ms)

**Zone 3: Kanban Board**
- Columns have no visible borders (floating headers)
- Task cards: Obsidian Glass pills with `CornerRadius="24"`
- Drag effect: Card lifts (-8px), becomes translucent, emits purple glow
- All text uses TextPrimary/TextSecondary colors

</details>

---

### 🌐 **Localization**

<table>
<tr>
<td width="50%">

#### **Runtime Language Switching**

- 🇬🇧 **English** (`en-US`)
- 🇷🇺 **Russian** (`ru-RU`)

**No restart required!** UI updates instantly via `ResourceDictionary` swapping.

</td>
<td width="50%">

#### **70+ Translated Strings**

```yaml
Coverage:
  - UI labels, buttons, placeholders
  - Error messages
  - Settings panel
  - Context menus
  - Tooltips

Files:
  - Resources/Languages/en-US.xaml
  - Resources/Languages/ru-RU.xaml
```

</td>
</tr>
</table>

**Persistent Preference**: Language choice saved to `settings.json` and restored on app launch.

---

## 📸 Screenshots

<table>
<tr>
<td align="center" width="50%">

### 💼 Professional Mode
![Professional Mode](docs/screenshots/professional-mode.png)

*Concise AI responses + Project Kanban + Executive Purple theme*

</td>
<td align="center" width="50%">

### 🎓 Student Mode
![Student Mode](docs/screenshots/student-mode.png)

*Explanatory AI + Study Planner + Educational Orange/Teal theme*

</td>
</tr>
<tr>
<td align="center">

### 📊 Kanban Board (Cyber-Noir Glass)
![Kanban](docs/screenshots/kanban-board.png)

*Floating glass cards on Void background with Neon Plasma accents*

</td>
<td align="center">

### 💬 Chat Interface (Liquid Plasma)
![Chat](docs/screenshots/chat-interface.png)

*User bubbles with animated gradient + AI glass bubbles*

</td>
</tr>
<tr>
<td align="center">

### 📚 Knowledge Graph
![Graph](docs/screenshots/knowledge-graph.png)

*Interactive document relationship visualization*

</td>
<td align="center">

### 🎤 Presentation Generator
![Presentation](docs/screenshots/presentation-gen.png)

*AI-generated .pptx slides with speaker notes*

</td>
</tr>
</table>

---

## 🛠️ Tech Stack

### **Core Framework**

```yaml
Runtime:        .NET 8.0 LTS (Long-Term Support)
Language:       C# 12 (Primary Constructors, Collection Expressions, File-Scoped Namespaces)
UI Framework:   WPF (Windows Presentation Foundation)
Database:       SQLite + Entity Framework Core 8.0
Architecture:   Clean Architecture (4 layers: Domain → Application → Infrastructure → Presentation)
Patterns:       MVVM, CQRS, Repository, Factory, Strategy, Railway Oriented Programming
```

### **AI Providers**

<table>
<tr>
<th>Provider</th>
<th>Model</th>
<th>Use Case</th>
<th>Features</th>
</tr>
<tr>
<td><b>🌐 Google Gemini</b></td>
<td><code>gemini-2.0-flash</code></td>
<td>Cloud AI</td>
<td>
  ✅ Multimodal (text + images)<br>
  ✅ Strict RAG with citations<br>
  ✅ 15 req/min free tier<br>
  ✅ Streaming responses
</td>
</tr>
<tr>
<td><b>🔐 Ollama</b></td>
<td><code>llama3</code>, <code>mistral</code>, etc.</td>
<td>Local LLM</td>
<td>
  ✅ 100% offline<br>
  ✅ Privacy-first<br>
  ✅ No API costs<br>
  ✅ Custom models
</td>
</tr>
</table>

### **Document Processing**

```yaml
PDF:        iText7 (v8.0.5)
Office:     DocumentFormat.OpenXml (v3.2.0) - DOCX, PPTX
eBooks:     VersOne.Epub (v3.3.5)
Text:       Native support for TXT, MD
```

### **UI & Design**

```yaml
Theme:          MaterialDesignInXamlToolkit (v5.1.0)
Icons:          Material Design Icons (2,000+ icons)
Design System:  Custom Cyber-Noir Glass (Resources/Styles/DarkNeuralGlass.xaml)
Animations:     WPF Storyboards, DoubleAnimation, ColorAnimation
MVVM:           CommunityToolkit.Mvvm (v8.4.0) - ObservableProperty, RelayCommand
```

### **Architecture Deep Dive**

<details>
<summary><b>🏛️ Clean Architecture (Click to expand)</b></summary>

```
┌──────────────────────────────────────────────────────────────┐
│                  NexusAI.Presentation                        │  ← User Interface
│  ┌────────────────────────────────────────────────────────┐  │
│  │ • ViewModels (MVVM)                                    │  │
│  │ • Views (XAML)                                         │  │
│  │ • Converters (BoolToVisibility, ModeToColor)          │  │
│  │ • Resources (DarkNeuralGlass.xaml, Languages)         │  │
│  │ Technology: WPF, MaterialDesign, CommunityToolkit     │  │
│  └────────────────────────────────────────────────────────┘  │
└──────────────────────────┬───────────────────────────────────┘
                           │ depends on ↓
┌──────────────────────────▼───────────────────────────────────┐
│                  NexusAI.Application                         │  ← Business Logic
│  ┌────────────────────────────────────────────────────────┐  │
│  │ • Use Cases (Commands/Queries with Handlers)           │  │
│  │ • Interfaces (IAiService, IProjectService)            │  │
│  │ • Services (SessionContext, KnowledgeGraphService)    │  │
│  │ • DTOs (Data Transfer Objects)                         │  │
│  │ Pattern: CQRS, Result<T> (Railway Oriented)           │  │
│  └────────────────────────────────────────────────────────┘  │
└──────────────────────────┬───────────────────────────────────┘
                           │ depends on ↓
┌──────────────────────────▼───────────────────────────────────┐
│                     NexusAI.Domain                           │  ← Core (Pure C#)
│  ┌────────────────────────────────────────────────────────┐  │
│  │ • Entities (User, Project, ProjectTask, ChatMessage)   │  │
│  │ • Value Objects                                         │  │
│  │ • Enums (AppMode, AiProvider, TaskStatus)             │  │
│  │ • Result<T> (Railway Oriented Programming)            │  │
│  │ ✅ ZERO external dependencies                          │  │
│  └────────────────────────────────────────────────────────┘  │
└──────────────────────────▲───────────────────────────────────┘
                           │ depends on ↑
┌──────────────────────────┴───────────────────────────────────┐
│                  NexusAI.Infrastructure                      │  ← External Concerns
│  ┌────────────────────────────────────────────────────────┐  │
│  │ • Persistence (AppDbContext, EF Core migrations)       │  │
│  │ • AI Services (GeminiAiService, OllamaService)        │  │
│  │ • Parsers (PdfParser, WordParser, EpubParser)         │  │
│  │ • External APIs (HTTP clients, Obsidian sync)         │  │
│  │ Technology: EF Core, HttpClient, iText7, OpenXml      │  │
│  └────────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────┘
```

**Dependency Rule**: All dependencies point **INWARD** toward Domain.

```diff
+ ✅ Presentation   → Application → Domain
+ ✅ Infrastructure → Application → Domain
+ ❌ Domain        → (NO dependencies on outer layers)
```

**Design Patterns:**

| Pattern | Usage | Location |
|---------|-------|----------|
| **MVVM** | UI separation | `Presentation/ViewModels/` |
| **CQRS** | Commands/Queries | `Application/UseCases/` |
| **Repository** | Data access abstraction | `IProjectService`, `IAuthService` |
| **Factory** | Runtime provider switching | `IAiServiceFactory`, `IDocumentParserFactory` |
| **Strategy** | Parser selection | `IDocumentParser` (PDF/Word/EPUB) |
| **Railway Oriented** | Error handling | `Result<T>` + extensions |
| **Primary Constructor** | DI injection (C# 12) | All services |

</details>

---

## 🏗️ Getting Started

### **Prerequisites**

```yaml
OS:           Windows 10/11 (x64)
Runtime:      .NET 8.0 SDK
RAM:          4 GB minimum, 8 GB recommended
Storage:      500 MB for app + documents
Display:      1920x1080 or higher (for optimal glass UI)
```

### **Installation Steps**

#### **Option 1: Download Release** (Recommended)

1. Go to [Releases](../../releases)
2. Download `NexusAI-v1.0.0-win-x64.zip`
3. Extract to a folder (e.g., `C:\Apps\NexusAI`)
4. Run `NexusAI.exe`
5. Enter your Gemini API key (get one at [aistudio.google.com](https://aistudio.google.com/apikey))

#### **Option 2: Build from Source**

```bash
# 1. Clone repository
git clone https://github.com/yourusername/NexusAI.git
cd NexusAI

# 2. Restore dependencies
dotnet restore

# 3. Build solution (Debug or Release)
dotnet build --configuration Release

# 4. Run application
cd src/NexusAI.Presentation
dotnet run

# Or open NexusAI.sln in Visual Studio 2022 and press F5
```

### **Configuration**

#### **Gemini API Key** (Required for Cloud AI)

1. Visit [Google AI Studio](https://aistudio.google.com/apikey)
2. Create a free API key (15 requests/minute)
3. Paste into the **API Key** field in the app header

#### **Ollama Setup** (Optional for Local AI)

```bash
# 1. Download Ollama from https://ollama.ai/
# 2. Install and start the service
# 3. Pull a model (e.g., llama3)
ollama pull llama3

# 4. In NexusAI, select "Ollama" as AI provider
# 5. Choose your downloaded model from dropdown
```

**No API key needed for Ollama — 100% offline!**

---

## 📚 User Guide

### **1️⃣ Adding Documents**

<table>
<tr>
<td width="33%">

**📁 File Dialog**

1. Click **ADD DOCUMENTS**
2. Select files (multi-select supported)
3. Supported: PDF, DOCX, PPTX, EPUB, TXT, MD

</td>
<td width="33%">

**🖱️ Drag & Drop**

1. Drag files from File Explorer
2. Drop onto document sidebar
3. Watch real-time parsing

</td>
<td width="33%">

**📝 Obsidian Vault**

1. Settings → Obsidian
2. Enter vault path
3. Click **Sync Vault**
4. All `.md` files imported

</td>
</tr>
</table>

**Document Management:**
- ✅ **Toggle checkbox** to include/exclude from AI context
- ✅ **Click ✕** to remove document
- ✅ **Hover** to see file size and load timestamp

---

### **2️⃣ AI Chat Interface**

#### **Asking Questions**

```markdown
💼 Professional Mode Example:
You: "Analyze Q4 revenue trends from the financial report."
AI:  "Revenue increased 23% YoY to $4.2M [Q4_Report.pdf]. 
      Key drivers: Enterprise segment (+35%), Product A sales..."

🎓 Student Mode Example:
You: "What is photosynthesis?"
AI:  "Great question! Let's break it down step-by-step.
      Think of a plant as a tiny solar panel factory [biology_ch3.pdf].
      
      The process has two main stages:
      1. Light-dependent reactions (capturing energy)
      2. Light-independent reactions (making glucose)
      
      Let me explain each..."
```

**Citations**: All responses include `[filename.pdf]` links. Click to highlight source in sidebar.

---

### **3️⃣ App Mode Switching**

Toggle between modes instantly (no restart):

| Feature | 💼 Professional | 🎓 Student |
|---------|----------------|------------|
| **AI Tone** | Concise, business-focused | Explanatory, teaching |
| **UI Labels** | "Projects", "Tasks" | "Subjects", "Assignments" |
| **Accent Color** | Deep Purple (`#6200EA`) | Orange/Teal |
| **Default Category** | Work | Education |

**Toggle Location**: Top header (💼 ⇄ 🎓 icons)

---

### **4️⃣ Project Management**

#### **Creating a Project**

```
1. Go to "Projects" tab
2. Click "➕ NEW PROJECT"
3. Fill in:
   - Title: "Website Redesign"
   - Description: "Modern UI overhaul with dark mode"
   - GitHub Repo: https://github.com/company/website (optional)
   - Category: Work / Education / Personal
4. Click "Generate Plan" → AI creates task breakdown
```

#### **Kanban Board**

```
┌──────────────┬──────────────┬──────────────┐
│    📝 TODO   │  🚀 IN PROGRESS │   ✅ DONE   │
├──────────────┼──────────────┼──────────────┤
│ Task 1       │ Task 3       │ Task 5       │
│ Task 2       │ Task 4       │ Task 6       │
│              │              │              │
│ [Drag here]  │ [Drag here]  │ [Drag here]  │
└──────────────┴──────────────┴──────────────┘
```

**Drag-and-drop:**
- Cards automatically update `OrderIndex` in database
- Smooth animations (lift effect on grab)
- Purple glow when hovering over drop zone

---

### **5️⃣ Content Generation**

#### **Artifacts**

| Type | Output | Use Case |
|------|--------|----------|
| 📖 **Deep Dive** | Markdown | Executive summary + insights |
| 📄 **Summary** | 2-4 paragraphs | Quick overview |
| 📚 **Study Guide** | Structured outline | Learning objectives + questions |
| ❓ **FAQ** | 10-15 Q&A | Common questions from sources |
| 🎙️ **Podcast Script** | Dialogue | 2-person conversation |

#### **Presentation Generator**

```yaml
1. Go to "Presentation" tab
2. Enter:
   - Topic: "AI in Healthcare"
   - Slide count: 10
3. Click "Generate Deck"
4. AI creates:
   - Title slide
   - Content slides (bullets)
   - Speaker notes
   - Conclusion
5. Download as .pptx
6. Edit in PowerPoint
```

---

### **6️⃣ Settings**

- **🌐 Language**: English / Russian (instant switch)
- **🤖 AI Provider**: Gemini / Ollama
- **🔑 API Key**: Paste Gemini key
- **🔊 Audio**: Volume, voice, speed

---

## 🗺️ Roadmap

<details>
<summary><b>✅ Completed (v1.0)</b></summary>

- [x] Gemini 2.0 Flash integration with streaming
- [x] Ollama local LLM support
- [x] RAG pipeline with source citations
- [x] PDF, DOCX, PPTX, EPUB parsing
- [x] Dual mode (Professional/Student)
- [x] Cyber-Noir Glass UI (2026 design)
- [x] Kanban board with drag-and-drop
- [x] AI project planning
- [x] PowerPoint generation
- [x] Wiki system
- [x] Knowledge graph
- [x] Obsidian integration
- [x] Code scaffolding
- [x] Text-to-Speech
- [x] Localization (English/Russian)
- [x] Clean Architecture implementation
- [x] SQLite + EF Core persistence

</details>

### **🚧 v1.1 - Q2 2025** (In Progress)

- [ ] **Performance**
  - [ ] Lazy loading for document lists (1000+ files)
  - [ ] Virtual scrolling in chat
  - [ ] Background indexing for RAG
  - [ ] Database connection pooling optimization

- [ ] **UX Enhancements**
  - [ ] Auto-scroll to latest message
  - [ ] Ctrl+F search within documents
  - [ ] Document preview modal
  - [ ] Keyboard shortcuts (Ctrl+K command palette)
  - [ ] Undo/redo for task moves

- [ ] **Data Portability**
  - [ ] Export chat history to PDF/Markdown
  - [ ] Import/export projects as JSON
  - [ ] Backup/restore database
  - [ ] Bulk document import folder

---

### **🔮 v2.0 - Q3 2025** (Future)

- [ ] **AI Providers**
  - [ ] OpenAI GPT-4 integration
  - [ ] Anthropic Claude support
  - [ ] Azure OpenAI Service
  - [ ] Custom API endpoint configuration

- [ ] **Advanced Features**
  - [ ] Vector embeddings (ChromaDB/Qdrant)
  - [ ] Semantic search across documents
  - [ ] Multi-user collaboration (SQLite → PostgreSQL)
  - [ ] Cloud sync (optional, Azure/AWS)
  - [ ] Voice input (speech-to-text)

- [ ] **Integrations**
  - [ ] Notion sync
  - [ ] Confluence integration
  - [ ] Google Drive import
  - [ ] Slack notifications

---

### **🌟 v3.0 - 2026** (Vision)

- [ ] **Enterprise**
  - [ ] SSO/SAML authentication
  - [ ] Role-based access control (RBAC)
  - [ ] Audit logging
  - [ ] Self-hosted server option

- [ ] **AI Enhancements**
  - [ ] Fine-tuned models (custom RAG)
  - [ ] Prompt template library
  - [ ] Multi-agent workflows
  - [ ] Autonomous task execution

- [ ] **Platform Expansion**
  - [ ] Web version (Blazor)
  - [ ] Mobile app (MAUI)
  - [ ] Linux support (Avalonia UI)

---

## 🤝 Contributing

Contributions are **highly welcome**! Whether it's bug fixes, new features, or documentation improvements.

### **Quick Start**

```bash
# 1. Fork repository
gh repo fork yourusername/NexusAI

# 2. Create feature branch
git checkout -b feature/amazing-feature

# 3. Make changes (follow C# 12 style)

# 4. Commit with conventional commits
git commit -m "feat: add export to JSON"

# 5. Push and open PR
git push origin feature/amazing-feature
```

### **Contribution Guidelines**

<details>
<summary><b>📝 Code Style (Click to expand)</b></summary>

```csharp
✅ DO:
- Use file-scoped namespaces (namespace X;)
- Use primary constructors for DI (public class Service(IRepo repo))
- Use collection expressions (List<int> x = [];)
- Use guard clauses (early returns)
- Return Result<T> from business logic
- Add XML documentation for public APIs

❌ DON'T:
- Throw exceptions in Domain/Application layers
- Reference Infrastructure from Application layer
- Use magic strings (use constants/resources)
- Add "AI comments" that explain what code does
- Nest logic more than 3 levels deep
```

**Example:**

```csharp
// ✅ GOOD: Primary constructor, guard clauses, Result<T>
public sealed class ProjectService(AppDbContext context) : IProjectService
{
    public async Task<Result<Project>> CreateAsync(string title, ...)
    {
        // Guard clause (early return)
        if (string.IsNullOrWhiteSpace(title))
            return Result<Project>.Failure("Title cannot be empty");
        
        var project = new Project { Title = title, ... };
        context.Projects.Add(project);
        await context.SaveChangesAsync();
        
        return Result<Project>.Success(project);
    }
}
```

</details>

<details>
<summary><b>🎯 Commit Message Format</b></summary>

```
feat:     New feature (e.g., "feat: add Notion integration")
fix:      Bug fix (e.g., "fix: resolve null ref in ProjectService")
docs:     Documentation (e.g., "docs: update README with Docker setup")
style:    Code style/formatting (no logic change)
refactor: Code restructuring (no behavior change)
perf:     Performance improvement
test:     Adding/updating tests
chore:    Build process, dependencies
```

</details>

### **Areas Needing Help**

- 🧪 **Testing**: Unit tests, integration tests (xUnit, FluentAssertions)
- 🌐 **Localization**: German, Spanish, French translations
- 🎨 **UI/UX**: Design improvements, accessibility (WCAG)
- 📚 **Documentation**: Tutorials, video guides, API docs
- 🐛 **Bug Fixes**: See [Issues](../../issues)

**Before contributing, please read [CONTRIBUTING.md](CONTRIBUTING.md) for detailed guidelines.**

---

## 📄 License

This project is licensed under the **MIT License**.

```
MIT License

Copyright (c) 2026 NexusAI Contributors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

**TL;DR**: Free to use, modify, and distribute. Just include the copyright notice.

---

## 🙏 Acknowledgments

Built with ❤️ using these amazing open-source projects:

<table>
<tr>
<td>

**AI & ML**
- [Google Gemini](https://ai.google.dev/) - Multimodal AI
- [Ollama](https://ollama.ai/) - Local LLM runtime

</td>
<td>

**UI & Design**
- [MaterialDesignInXaml](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit) - WPF components
- [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) - MVVM helpers

</td>
</tr>
<tr>
<td>

**Document Processing**
- [iText7](https://itextpdf.com/) - PDF library
- [DocumentFormat.OpenXml](https://github.com/OfficeDev/Open-XML-SDK) - Office files
- [VersOne.Epub](https://github.com/vers-one/EpubReader) - EPUB reader

</td>
<td>

**Data & Infrastructure**
- [Entity Framework Core](https://github.com/dotnet/efcore) - ORM
- [SQLite](https://www.sqlite.org/) - Embedded database

</td>
</tr>
</table>

---

## 📞 Support & Community

### **Get Help**

- 📖 **Documentation**: [Wiki](../../wiki)
- 💬 **Discussions**: [GitHub Discussions](../../discussions)
- 🐛 **Bug Reports**: [Issues](../../issues)
- 📧 **Contact**: [vitalydoxr@gmail.com](mailto:vitalydoxr@gmail.com)

### **Stay Updated**

- ⭐ **Star this repo** to receive notifications
- 👀 **Watch releases** for new versions
- 🔔 **Subscribe to Discussions** for announcements

---

## 📊 Project Stats

![GitHub stars](https://img.shields.io/github/stars/yourusername/NexusAI?style=social)
![GitHub forks](https://img.shields.io/github/forks/yourusername/NexusAI?style=social)
![GitHub watchers](https://img.shields.io/github/watchers/yourusername/NexusAI?style=social)

![GitHub issues](https://img.shields.io/github/issues/yourusername/NexusAI)
![GitHub pull requests](https://img.shields.io/github/issues-pr/yourusername/NexusAI)
![GitHub last commit](https://img.shields.io/github/last-commit/yourusername/NexusAI)
![Code size](https://img.shields.io/github/languages/code-size/yourusername/NexusAI)

---

<div align="center">

## 🌟 Star History

[![Star History Chart](https://api.star-history.com/svg?repos=yourusername/NexusAI&type=Date)](https://star-history.com/#yourusername/NexusAI&Date)

---

**Built with ❤️ using .NET 8 LTS and Modern C# 12**

*NexusAI — Your Intelligent Workspace Companion*

**v1.0.0** • [Download](../../releases) • [Documentation](../../wiki) • [Report Bug](../../issues) • [Request Feature](../../issues/new?template=feature_request.md)

[⬆️ Back to Top](#-nexusai)

---

**Made with** 🧠 **by developers, for developers**

© 2026 NexusAI Contributors • [MIT License](LICENSE)

</div>
