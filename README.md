# 🧠 NexusAI

<div align="center">

**AI-Powered Project & Knowledge Management Platform**

*RAG Document Analysis · Project Management · PowerPoint Generation · Wiki Knowledge Base · Dual AI Mode*

[![.NET 8 LTS](https://img.shields.io/badge/.NET-8.0_LTS-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![WPF](https://img.shields.io/badge/WPF-Windows-0078D4?logo=windows)](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
[![C# 12](https://img.shields.io/badge/C%23-12-239120?logo=c-sharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![EF Core](https://img.shields.io/badge/EF_Core-8.0-512BD4)](https://docs.microsoft.com/en-us/ef/core/)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-blue)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

![NexusAI Banner](docs/screenshots/banner.png)

</div>

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Key Features](#-key-features)
- [Screenshots](#-screenshots)
- [Tech Stack](#️-tech-stack)
- [Architecture](#️-architecture)
- [Database Schema](#-database-schema)
- [Installation](#-installation)
- [Quick Start](#-quick-start)
- [User Guide](#-user-guide)
- [Project Structure](#-project-structure)
- [Development](#-development)
- [Roadmap](#️-roadmap)
- [Contributing](#-contributing)
- [License](#-license)

---

## 🌟 Overview

**NexusAI** is a next-generation desktop application that combines AI-powered document analysis with project management, presentation generation, and knowledge organization. Built with Clean Architecture principles and modern C# 12, it provides a professional workspace for both business professionals and students.

### 🎯 Perfect For:

- **Business Professionals**: Project planning, document analysis, executive presentations
- **Students**: Study guides, research organization, knowledge graphs
- **Researchers**: Multi-document RAG analysis, wiki knowledge bases
- **Developers**: GitHub-integrated Kanban boards, code scaffolding

### ✨ What Makes NexusAI Special:

```
🧠 Dual AI Mode           → Switch between Professional and Student personas
🎨 Dark Neural Glass UI   → 2026-standard glassmorphic design with fluid animations
🌐 Multilingual           → Runtime language switching (English/Russian)
📊 Full RAG Pipeline      → Gemini 2.0 Flash + Ollama local LLM support
🗂️ Complete PM Suite     → Projects, Kanban boards, GitHub integration
📚 Knowledge Management   → Wiki, Knowledge Graph, Obsidian sync
🎤 Presentation Engine    → AI-generated PowerPoint decks from documents
```

---

## 🚀 Key Features

### 🤖 AI & Document Analysis

<table>
<tr>
<td width="50%">

#### **Multi-Provider AI**
- ✅ **Gemini 2.0 Flash** (Cloud, multimodal)
- ✅ **Ollama** (Local, privacy-first)
- ✅ Dynamic mode switching at runtime
- ✅ Streaming responses with citations

</td>
<td width="50%">

#### **RAG Document Processing**
- ✅ PDF, DOCX, PPTX, EPUB, TXT, MD
- ✅ Multi-document context merging
- ✅ Source citations `[filename.pdf]`
- ✅ Drag & drop support

</td>
</tr>
<tr>
<td>

#### **Dual AI Personality**
- 🎩 **Professional Mode**: Concise, business-focused
- 🎓 **Student Mode**: Socratic teaching, explanations
- ⚡ Instant UI transformation on toggle

</td>
<td>

#### **Multimodal Analysis**
- 🖼️ Image understanding (Gemini Vision)
- 🎙️ Text-to-Speech (integrated audio player)
- 📊 Visual artifact generation

</td>
</tr>
</table>

---

### 📊 Project Management

<table>
<tr>
<td width="50%">

#### **Advanced Kanban Board**
- ✅ Drag-and-drop task management
- ✅ **OrderIndex** for persistent sorting
- ✅ Priority badges (High/Medium/Low)
- ✅ Role-based visual tags
- ✅ Smart document linking

</td>
<td width="50%">

#### **GitHub Integration**
- ✅ Link repositories via `GitHubRepoUrl`
- ✅ Track issues with `GitHubIssueNumber`
- ✅ Generate code scaffolding
- ✅ Store project files (`ProjectFile` entity)

</td>
</tr>
<tr>
<td>

#### **AI-Powered Planning**
- ✅ Generate project roadmaps from descriptions
- ✅ Automatic task breakdown
- ✅ Estimated hours calculation
- ✅ Analytics dashboard

</td>
<td>

#### **Category Filtering**
- ✅ Work / Education / Personal
- ✅ Context-aware project lists
- ✅ Mode-based auto-filtering

</td>
</tr>
</table>

---

### 📚 Knowledge Management

<table>
<tr>
<td width="50%">

#### **Wiki System**
- ✅ Hierarchical knowledge base
- ✅ AI-generated wiki structures
- ✅ Markdown editing
- ✅ Tag-based organization

</td>
<td width="50%">

#### **Knowledge Graph**
- ✅ Visual document relationships
- ✅ Keyword-based connections
- ✅ Interactive canvas rendering
- ✅ Real-time graph updates

</td>
</tr>
<tr>
<td>

#### **Obsidian Integration**
- ✅ Vault import (with subfolders)
- ✅ Export chat/artifacts with backlinks
- ✅ Bidirectional sync

</td>
<td>

#### **Artifacts Generator**
- ✅ Study Guide
- ✅ FAQ (10-15 Q&A)
- ✅ Executive Summary
- ✅ Podcast Script
- ✅ Deep Dive Analysis

</td>
</tr>
</table>

---

### 🎨 Presentation Engine

<table>
<tr>
<td width="50%">

#### **PowerPoint Generation**
- ✅ AI-generated slide structures
- ✅ Topic-based deck creation
- ✅ Fully editable `.pptx` output
- ✅ Uses `DocumentFormat.OpenXml`

</td>
<td width="50%">

#### **Smart Content**
- ✅ Title + bullet points per slide
- ✅ Speaker notes
- ✅ Professional templates
- ✅ Multi-slide generation (configurable)

</td>
</tr>
</table>

---

### 🎨 Dark Neural Glass UI

<table>
<tr>
<td width="50%">

#### **2026 Design Standard**
- ✅ **Glassmorphism**: Frosted, semi-transparent surfaces
- ✅ **Cyber-Noir Palette**: `#050505` base, electric purple gradients
- ✅ **Heavy Rounded Corners**: `24px` cards, `12px` buttons
- ✅ **Apple Typography**: Segoe UI Variable Display

</td>
<td width="50%">

#### **Fluid Animations**
- ✅ **Message Entrance**: Slide up + fade (300ms)
- ✅ **Mode Transitions**: Cross-fade colors (500ms)
- ✅ **Hover States**: Scale + glow (150ms)
- ✅ **Liquid Chat Bubbles**: Animated plasma gradients

</td>
</tr>
<tr>
<td>

#### **Custom Window Chrome**
- ✅ No standard title bar
- ✅ Draggable glass header
- ✅ Custom minimize/maximize/close buttons
- ✅ Blends into sidebar

</td>
<td>

#### **Material Design 3**
- ✅ Elevation shadows
- ✅ Icon system (Material Design Icons)
- ✅ Card-based layouts
- ✅ Responsive components

</td>
</tr>
</table>

---

### 🌐 Localization

<table>
<tr>
<td width="50%">

#### **Runtime Language Switching**
- ✅ English (`en-US`)
- ✅ Russian (`ru-RU`)
- ✅ No restart required
- ✅ `ResourceDictionary` swapping

</td>
<td width="50%">

#### **70+ Translated Strings**
- ✅ UI labels, buttons, placeholders
- ✅ Error messages
- ✅ Settings panel
- ✅ Persistent preference storage

</td>
</tr>
</table>

---

## 📸 Screenshots

<table>
<tr>
<td align="center" width="50%">

### Professional Mode
![Professional Mode](docs/screenshots/professional-mode.png)

</td>
<td align="center" width="50%">

### Student Mode
![Student Mode](docs/screenshots/student-mode.png)

</td>
</tr>
<tr>
<td align="center">

### Kanban Board
![Kanban](docs/screenshots/kanban-board.png)

</td>
<td align="center">

### Knowledge Graph
![Graph](docs/screenshots/knowledge-graph.png)

</td>
</tr>
<tr>
<td align="center">

### Wiki System
![Wiki](docs/screenshots/wiki-system.png)

</td>
<td align="center">

### Presentation Generator
![Presentation](docs/screenshots/presentation-gen.png)

</td>
</tr>
</table>

---

## 🛠️ Tech Stack

### **Core Framework**

```yaml
Runtime:        .NET 8.0 LTS
Language:       C# 12 (Primary Constructors, Collection Expressions, File-Scoped Namespaces)
UI:             WPF (Windows Presentation Foundation)
Database:       SQLite + Entity Framework Core 8.0
Architecture:   Clean Architecture (4 layers)
Patterns:       MVVM, CQRS, Repository, Factory, Strategy
```

### **AI & Machine Learning**

| Provider | Model | Use Case |
|----------|-------|----------|
| **Google Gemini** | `gemini-2.0-flash-exp` | Cloud AI, multimodal, strict RAG |
| **Ollama** | `llama3`, `mistral`, etc. | Local LLM, privacy-first |

### **Document Processing**

```yaml
PDF:        iText7 (v8.0.5)
Office:     DocumentFormat.OpenXml (v3.4.1) - DOCX, PPTX
eBooks:     VersOne.Epub (v3.3.5)
Text:       Native support for TXT, MD
```

### **UI & Design**

```yaml
Theme:          MaterialDesignInXamlToolkit (v5.1.0)
Icons:          Material Design Icons
Layout:         Custom glassmorphic design system
Animations:     WPF Storyboards, DoubleAnimation, ColorAnimation
MVVM:           CommunityToolkit.Mvvm (v8.3.2)
```

### **Database & Persistence**

```yaml
ORM:                    Entity Framework Core 8.0.11
Provider:               Microsoft.EntityFrameworkCore.Sqlite
Migrations:             Code-First approach
Connection Pooling:     Enabled by default
```

### **Dependencies**

```xml
<!-- Core -->
<PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.2" />
<PackageReference Include="Microsoft.Extensions.Http" Version="8.0.1" />
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.3.2" />

<!-- Database -->
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="8.0.11" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.11" />

<!-- Document Processing -->
<PackageReference Include="itext7" Version="8.0.5" />
<PackageReference Include="DocumentFormat.OpenXml" Version="3.4.1" />
<PackageReference Include="VersOne.Epub" Version="3.3.5" />

<!-- Audio -->
<PackageReference Include="System.Speech" Version="8.0.0" />

<!-- GitHub Integration -->
<PackageReference Include="Octokit" Version="14.0.0" />
```

---

## 🏗️ Architecture

### **Clean Architecture (Robert C. Martin)**

NexusAI strictly follows Clean Architecture principles with **4 isolated layers**:

```
┌──────────────────────────────────────────────────────────────┐
│                  NexusAI.Presentation                        │
│  ┌────────────────────────────────────────────────────────┐  │
│  │ ViewModels · Views · Converters · XAML · App.xaml     │  │
│  │ Technology: WPF, MaterialDesign, MVVM                  │  │
│  └────────────────────────────────────────────────────────┘  │
└──────────────────────────┬───────────────────────────────────┘
                           │ depends on ↓
┌──────────────────────────▼───────────────────────────────────┐
│                  NexusAI.Application                         │
│  ┌────────────────────────────────────────────────────────┐  │
│  │ Use Cases · Commands · Handlers · Interfaces           │  │
│  │ Business Logic · Validation · DTOs                     │  │
│  └────────────────────────────────────────────────────────┘  │
└──────────────────────────┬───────────────────────────────────┘
                           │ depends on ↓
┌──────────────────────────▼───────────────────────────────────┐
│                     NexusAI.Domain                           │
│  ┌────────────────────────────────────────────────────────┐  │
│  │ Entities · Value Objects · Enums · Result<T>           │  │
│  │ ✅ ZERO external dependencies (pure C#)                │  │
│  └────────────────────────────────────────────────────────┘  │
└──────────────────────────▲───────────────────────────────────┘
                           │ depends on ↑
┌──────────────────────────┴───────────────────────────────────┐
│                  NexusAI.Infrastructure                      │
│  ┌────────────────────────────────────────────────────────┐  │
│  │ EF Core DbContext · AI Services · Parsers · File I/O   │  │
│  │ External APIs · Third-party integrations               │  │
│  └────────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────┘
```

### **Dependency Rule**

> **All dependencies point INWARD toward the Domain layer.**

```
✅ Presentation   → Application → Domain
✅ Infrastructure → Application → Domain
❌ Domain        → (NO dependencies on outer layers)
```

### **Design Patterns**

| Pattern | Implementation | Location |
|---------|----------------|----------|
| **MVVM** | `ObservableProperty`, `RelayCommand` | `Presentation/ViewModels/` |
| **CQRS** | Commands/Queries with Handlers | `Application/UseCases/` |
| **Repository** | `IProjectService`, `IAuthService` | `Application/Interfaces/` |
| **Factory** | `IAiServiceFactory`, `IDocumentParserFactory` | `Infrastructure/Services/` |
| **Strategy** | `IDocumentParser` (PDF/Word/EPUB) | `Infrastructure/Parsers/` |
| **Singleton** | `SessionContext`, `LocalizationService` | `Application/Services/` |
| **Railway Oriented** | `Result<T>` + 15 extension methods | `Domain/Common/Result.cs` |
| **Primary Constructor** | All services (C# 12) | Throughout |

### **SOLID Principles Compliance**

```diff
+ Single Responsibility:  One handler per use case
+ Open/Closed:            Add new parsers without modifying existing code
+ Liskov Substitution:    All IAiService implementations are interchangeable
+ Interface Segregation:  Small, focused interfaces (IDocumentParser, IAiService)
+ Dependency Inversion:   All layers depend on abstractions, not concretions
```

### **Error Handling: Railway Oriented Programming**

```csharp
// No exceptions in business logic - all errors are values
public async Task<Result<Project>> CreateProjectAsync(string title, ...)
{
    if (string.IsNullOrWhiteSpace(title))
        return Result<Project>.Failure("Title cannot be empty");

    var project = new Project { /* ... */ };
    await _context.SaveChangesAsync();

    return Result<Project>.Success(project);
}

// Fluent chaining
var result = await GetUserAsync(userId)
    .BindAsync(user => CreateProjectAsync(user.Id, title))
    .MapAsync(project => new ProjectDto(project));

// Pattern matching
result.Match(
    onSuccess: dto => Console.WriteLine($"Created: {dto.Title}"),
    onFailure: error => Console.WriteLine($"Error: {error}")
);
```

---

## 💾 Database Schema

### **Entity Relationship Diagram**

```
┌─────────────────┐
│      User       │
├─────────────────┤
│ Guid Id (PK)    │
│ Username        │◄────────┐
│ PasswordHash    │         │
│ CreatedAt       │         │
└─────────────────┘         │
        ▲                   │
        │ 1:N               │
        │                   │
┌───────┴─────────┐         │
│    Project      │         │
├─────────────────┤         │
│ Guid Id (PK)    │         │
│ UserId (FK) ────┼─────────┘
│ Title           │
│ Description     │
│ GitHubRepoUrl   │ (nullable)
│ CreatedAt       │
└─────────────────┘
        ▲ 1:N
        ├──────────────────┐
        │                  │
┌───────┴─────────┐  ┌─────┴───────────┐
│  ProjectTask    │  │  ProjectFile    │
├─────────────────┤  ├─────────────────┤
│ Guid Id (PK)    │  │ Guid Id (PK)    │
│ ProjectId (FK)  │  │ ProjectId (FK)  │
│ Title           │  │ FilePath        │
│ Description     │  │ Content         │
│ Status          │  │ Language        │
│ Role            │  │ CreatedAt       │
│ EstimatedHours  │  └─────────────────┘
│ OrderIndex      │ ⭐ Kanban sorting
│ GitHubIssueNum  │ ⭐ GitHub integration
│ CreatedAt       │
└─────────────────┘

┌─────────────────┐
│   ChatSession   │
├─────────────────┤
│ Guid Id (PK)    │
│ UserId (FK) ────┼──► User
│ Title           │
│ CreatedAt       │
│ LastActivityAt  │
└─────────────────┘
        ▲ 1:N
        │
┌───────┴─────────┐
│  ChatMessage    │
├─────────────────┤
│ Guid Id (PK)    │
│ ChatSessionId   │ (FK)
│ Content         │
│ Role            │ (User/Assistant/System)
│ Timestamp       │
└─────────────────┘
```

### **Tables & Indices**

#### **Users**
```sql
CREATE TABLE Users (
    Id           TEXT PRIMARY KEY,
    Username     TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    CreatedAt    TEXT NOT NULL
);
CREATE UNIQUE INDEX IX_Users_Username ON Users(Username);
```

#### **Projects**
```sql
CREATE TABLE Projects (
    Id            TEXT PRIMARY KEY,
    UserId        TEXT NOT NULL,
    Title         TEXT NOT NULL,
    Description   TEXT NOT NULL,
    GitHubRepoUrl TEXT,
    CreatedAt     TEXT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
```

#### **ProjectTasks**
```sql
CREATE TABLE ProjectTasks (
    Id                TEXT PRIMARY KEY,
    ProjectId         TEXT NOT NULL,
    Title             TEXT NOT NULL,
    Description       TEXT NOT NULL,
    Status            TEXT NOT NULL,  -- 'Todo', 'InProgress', 'Done'
    Role              TEXT NOT NULL,
    EstimatedHours    REAL NOT NULL,
    OrderIndex        INTEGER NOT NULL,     -- ⭐ For Kanban sorting
    GitHubIssueNumber INTEGER,              -- ⭐ GitHub issue tracking
    CreatedAt         TEXT NOT NULL,
    FOREIGN KEY (ProjectId) REFERENCES Projects(Id) ON DELETE CASCADE
);
CREATE INDEX IX_ProjectTasks_ProjectId ON ProjectTasks(ProjectId);
CREATE INDEX IX_ProjectTasks_Status ON ProjectTasks(Status);
CREATE INDEX IX_ProjectTasks_ProjectId_OrderIndex ON ProjectTasks(ProjectId, OrderIndex);
```

#### **ProjectFiles**
```sql
CREATE TABLE ProjectFiles (
    Id         TEXT PRIMARY KEY,
    ProjectId  TEXT NOT NULL,
    FilePath   TEXT NOT NULL,
    Content    TEXT NOT NULL,
    Language   TEXT NOT NULL,
    CreatedAt  TEXT NOT NULL,
    FOREIGN KEY (ProjectId) REFERENCES Projects(Id) ON DELETE CASCADE
);
CREATE INDEX IX_ProjectFiles_ProjectId ON ProjectFiles(ProjectId);
```

#### **ChatSessions**
```sql
CREATE TABLE ChatSessions (
    Id             TEXT PRIMARY KEY,
    UserId         TEXT NOT NULL,
    Title          TEXT NOT NULL,
    CreatedAt      TEXT NOT NULL,
    LastActivityAt TEXT,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
CREATE INDEX IX_ChatSessions_UserId ON ChatSessions(UserId);
CREATE INDEX IX_ChatSessions_LastActivityAt ON ChatSessions(LastActivityAt);
```

#### **ChatMessages**
```sql
CREATE TABLE ChatMessages (
    Id            TEXT PRIMARY KEY,
    ChatSessionId TEXT NOT NULL,
    Content       TEXT NOT NULL,
    Role          TEXT NOT NULL,  -- 'User', 'Assistant', 'System'
    Timestamp     TEXT NOT NULL,
    FOREIGN KEY (ChatSessionId) REFERENCES ChatSessions(Id) ON DELETE CASCADE
);
CREATE INDEX IX_ChatMessages_ChatSessionId ON ChatMessages(ChatSessionId);
CREATE INDEX IX_ChatMessages_Timestamp ON ChatMessages(Timestamp);
```

### **EF Core Configuration**

All relationships and constraints are configured using **Fluent API** in `AppDbContext.cs`:

```csharp
// Example: Project → Tasks cascade delete
modelBuilder.Entity<Project>(entity =>
{
    entity.HasMany(e => e.Tasks)
        .WithOne(e => e.Project)
        .HasForeignKey(e => e.ProjectId)
        .OnDelete(DeleteBehavior.Cascade);  // ✅ Auto-delete tasks when project deleted
});
```

---

## 📥 Installation

### **System Requirements**

```yaml
OS:           Windows 10/11 (x64)
Runtime:      .NET 8.0 SDK or Runtime
RAM:          4 GB minimum, 8 GB recommended
Storage:      500 MB for application + documents
Display:      1920x1080 or higher (for optimal UI experience)
```

### **Prerequisites**

1. **Install .NET 8.0 SDK**
   ```bash
   # Download from: https://dotnet.microsoft.com/download/dotnet/8.0
   winget install Microsoft.DotNet.SDK.8
   ```

2. **(Optional) Install Ollama for Local LLM**
   ```bash
   # Download from: https://ollama.ai/
   # Then pull a model:
   ollama pull llama3
   ```

3. **Get Gemini API Key**
   - Visit: https://aistudio.google.com/apikey
   - Create a free API key (Free tier: 15 requests/minute)

---

## 🚀 Quick Start

### **Option 1: Run Pre-built Release** (Easiest)

1. Download `NexusAI-v1.0.0-win-x64.zip` from [Releases](../../releases)
2. Extract to a folder (e.g., `C:\Apps\NexusAI`)
3. Run `NexusAI.exe`
4. Enter your Gemini API key in the header
5. Start adding documents!

### **Option 2: Build from Source**

```bash
# 1. Clone repository
git clone https://github.com/yourusername/NexusAI.git
cd NexusAI

# 2. Restore NuGet packages
dotnet restore NexusAI.sln

# 3. Build solution (all 4 projects)
dotnet build NexusAI.sln --configuration Release

# 4. Run Presentation project
cd src/NexusAI.Presentation
dotnet run

# Or open in Visual Studio 2022 and press F5
```

### **First Launch**

1. **Database Initialization**: SQLite database (`nexus.db`) is created automatically in the app directory
2. **Enter API Key**: Paste your Gemini API key in the top header
3. **Select Language**: Choose English or Russian from Settings
4. **Create Account**: Register a username/password (stored locally)

---

## 📚 User Guide

### **1. Document Management**

#### **Adding Documents**

**Method 1: File Dialog**
```
1. Click "ADD DOCUMENTS" button in left sidebar
2. Select files (PDF, DOCX, PPTX, EPUB, TXT, MD)
3. Files are parsed and indexed automatically
```

**Method 2: Drag & Drop**
```
1. Drag files from File Explorer
2. Drop onto the document list area
3. Watch real-time parsing progress
```

**Method 3: Obsidian Vault**
```
1. Open Settings → Obsidian Integration
2. Enter vault path: C:\Users\You\Documents\Obsidian\MyVault
3. (Optional) Specify subfolder: Research/AI
4. Click "Sync Vault"
5. All markdown files imported with structure preserved
```

#### **Document Actions**

- ✅ **Toggle Inclusion**: Click checkbox to include/exclude from AI context
- ✅ **Remove**: Click `✕` button to delete from list
- ✅ **View**: Click document name to preview (future feature)

---

### **2. AI Chat Interface**

#### **Asking Questions**

```
🎩 Professional Mode:
You: "Analyze Q4 revenue trends from the financial report."
AI:  "Revenue increased 23% YoY [Q4_Report.pdf]. Key drivers: ..."

🎓 Student Mode:
You: "What is photosynthesis?"
AI:  "Great question! Let's break it down step-by-step. Think of a plant 
      as a solar panel factory [biology_chapter3.pdf]..."
```

#### **Citations**

All AI responses include source citations:
```
[filename.pdf]    → Exact source document
[page 42]         → Specific page reference (PDF only)
```

Click a citation to highlight the source in the sidebar.

---

### **3. App Mode Switching**

Toggle between **Professional** and **Student** modes instantly:

| Feature | 🎩 Professional Mode | 🎓 Student Mode |
|---------|---------------------|----------------|
| **AI Tone** | Concise, business-focused | Explanatory, teaching |
| **UI Labels** | "Projects" | "Subjects" |
| **Color Accent** | Deep Purple (`#6200EA`) | Teal/Orange |
| **Target Audience** | Executives, PMs | Students, learners |

**Toggle Location**: Bottom of left sidebar (Briefcase ⇄ Graduation Cap icon)

---

### **4. Project Management**

#### **Creating a Project**

```
1. Go to "Projects" tab
2. Click "New Project"
3. Enter:
   - Title: "Website Redesign"
   - Description: "Overhaul company website with modern design"
   - GitHub Repo: https://github.com/company/website (optional)
   - Category: Work / Education / Personal
4. Click "Generate Plan" to auto-create tasks using AI
```

#### **Kanban Board**

```
┌─────────────┬─────────────┬─────────────┐
│    TODO     │ IN PROGRESS │    DONE     │
├─────────────┼─────────────┼─────────────┤
│ Task 1      │ Task 3      │ Task 5      │
│ Task 2      │ Task 4      │             │
│             │             │             │
│ [Drag here] │ [Drag here] │ [Drag here] │
└─────────────┴─────────────┴─────────────┘
```

**Features:**
- ✅ Drag tasks between columns
- ✅ **OrderIndex** automatically updated
- ✅ Priority badges (High = 🔴, Medium = 🟡, Low = 🟢)
- ✅ Role tags (Dev, Design, Marketing)
- ✅ Estimated hours display
- ✅ GitHub issue number (if linked)

#### **Analytics Dashboard**

```
┌─────────────────────────────────────┐
│ Project Completion: 67% ████████░░  │
│                                     │
│ Tasks by Role:                      │
│  Dev:       5 tasks                 │
│  Design:    3 tasks                 │
│  Marketing: 2 tasks                 │
└─────────────────────────────────────┘
```

---

### **5. Knowledge Management**

#### **Wiki System**

```
📁 Root Wiki
  ├─ 📄 Introduction
  ├─ 📁 Chapter 1: Basics
  │   ├─ 📄 1.1 Getting Started
  │   └─ 📄 1.2 Key Concepts
  ├─ 📁 Chapter 2: Advanced
  │   └─ 📄 2.1 Best Practices
  └─ 📄 Conclusion
```

**Actions:**
- ✅ **Generate Wiki**: AI creates hierarchical structure from documents
- ✅ **Edit Pages**: Markdown editor with live preview
- ✅ **Reorder**: Drag pages to change hierarchy
- ✅ **Tags**: Organize with custom tags
- ✅ **Export**: Save to Obsidian vault with backlinks

#### **Knowledge Graph**

```
       [Doc A]
      /   |   \
    /     |     \
[Doc B]--[Doc C]--[Doc D]
    \     |     /
      \   |   /
       [Doc E]
```

- **Nodes**: Documents (size = word count)
- **Edges**: Shared keywords (thickness = # of shared terms)
- **Interactive**: Click to highlight connections

---

### **6. Presentation Generator**

```
1. Go to "Presentation" tab
2. Enter topic: "AI in Healthcare"
3. Select slide count: 10
4. Click "Generate Deck"
5. AI creates:
   - Title slide
   - Content slides (bullet points)
   - Speaker notes
   - Conclusion
6. Download as .pptx (fully editable in PowerPoint)
```

**Prompt Example (sent to AI):**
```
You are a Presentation Expert. Create a structured outline for a PowerPoint 
deck on 'AI in Healthcare' with 10 slides. Return JSON:
[
  { "title": "...", "points": ["...", "..."], "notes": "..." },
  ...
]
```

---

### **7. Artifacts**

Generate specialized outputs from your documents:

| Artifact Type | Output Format | Use Case |
|--------------|---------------|----------|
| **Deep Dive** | Markdown report | Executive summary + key insights |
| **Summary** | 2-4 paragraphs | Quick overview |
| **Study Guide** | Structured outline | Learning objectives + questions |
| **FAQ** | 10-15 Q&A pairs | Common questions from sources |
| **Podcast Script** | Dialogue format | 2-person conversation |
| **Notebook Guide** | Russian-language guide | Structured learning notes |

---

### **8. Settings**

#### **Language**
- 🇬🇧 **English** (`en-US`)
- 🇷🇺 **Russian** (`ru-RU`)
- Changes apply instantly (no restart required)

#### **AI Provider**
- **Gemini 2.0 Flash**: Paste API key
- **Ollama**: Select local model

#### **Audio**
- Volume control
- Voice selection (system TTS)
- Speed adjustment

---

## 📁 Project Structure

```
NexusAI/
├── src/
│   ├── NexusAI.Domain/                           # ✅ Core (0 dependencies)
│   │   ├── Entities/                             # EF Core entities
│   │   │   ├── User.cs                           # Guid Id, Username, PasswordHash
│   │   │   ├── Project.cs                        # + GitHubRepoUrl (nullable)
│   │   │   ├── ProjectTask.cs                    # + OrderIndex, GitHubIssueNumber
│   │   │   ├── ProjectFile.cs                    # ⭐ NEW: FilePath, Content, Language
│   │   │   ├── ChatSession.cs                    # ⭐ NEW: UserId, Title, Messages
│   │   │   └── ChatMessage.cs                    # ChatSessionId, Content, Role
│   │   └── Common/
│   │       ├── Result.cs                         # Railway-oriented programming
│   │       └── ResultExtensions.cs               # Fluent methods (Bind, Map, Match)
│   │
│   ├── NexusAI.Application/                      # → Domain only
│   │   ├── Interfaces/
│   │   │   ├── IAiService.cs                     # Abstraction for Gemini/Ollama
│   │   │   ├── IAiServiceFactory.cs              # Runtime provider switching
│   │   │   ├── IAuthService.cs                   # User registration/login
│   │   │   ├── IProjectService.cs                # Project/Task CRUD
│   │   │   ├── IDocumentParser.cs                # Strategy pattern for parsers
│   │   │   ├── IObsidianService.cs               # Vault import/export
│   │   │   ├── IPresentationService.cs           # PowerPoint generation
│   │   │   └── ILocalizationService.cs           # Language switching
│   │   ├── UseCases/
│   │   │   ├── Auth/
│   │   │   │   ├── LoginCommand.cs               # Login use case
│   │   │   │   └── RegisterCommand.cs            # Registration use case
│   │   │   ├── Projects/
│   │   │   │   ├── CreateProjectCommand.cs
│   │   │   │   ├── GenerateProjectPlanHandler.cs # AI-powered task generation
│   │   │   │   ├── UpdateTaskStatusHandler.cs
│   │   │   │   └── GetUserProjectsHandler.cs
│   │   │   ├── Documents/
│   │   │   │   └── AddDocumentHandler.cs
│   │   │   ├── Chat/
│   │   │   │   └── AskQuestionHandler.cs         # RAG pipeline
│   │   │   ├── Wiki/
│   │   │   │   ├── GenerateWikiHandler.cs
│   │   │   │   └── UpdateWikiPageHandler.cs
│   │   │   ├── Presentations/
│   │   │   │   └── GeneratePresentationHandler.cs
│   │   │   └── Scaffold/
│   │   │       └── GenerateScaffoldHandler.cs    # Code generation
│   │   ├── Services/
│   │   │   ├── SessionContext.cs                 # Singleton: CurrentMode, CurrentUser
│   │   │   ├── KnowledgeGraphService.cs          # Graph computation
│   │   │   └── KnowledgeHubService.cs            # Document indexing
│   │   └── DependencyInjection.cs                # Use case registration
│   │
│   ├── NexusAI.Infrastructure/                   # → Domain + Application
│   │   ├── Persistence/
│   │   │   └── AppDbContext.cs                   # ✅ EF Core DbContext (primary constructor)
│   │   ├── Services/
│   │   │   ├── AuthService.cs                    # ✅ C# 12: primary constructor
│   │   │   ├── ProjectService.cs                 # ✅ C# 12: guard clauses
│   │   │   ├── GeminiAiService.cs                # Gemini 2.0 Flash implementation
│   │   │   ├── OllamaService.cs                  # Local LLM implementation
│   │   │   ├── AiServiceFactory.cs               # Factory pattern
│   │   │   ├── ObsidianService.cs                # Vault sync
│   │   │   ├── PresentationService.cs            # PPTX generation (OpenXml)
│   │   │   ├── SpeechSynthesisService.cs         # Text-to-Speech
│   │   │   ├── WikiService.cs                    # Wiki CRUD
│   │   │   └── ScaffoldingService.cs             # Code scaffolding
│   │   ├── Parsers/
│   │   │   ├── PdfParser.cs                      # iText7
│   │   │   ├── WordParser.cs                     # DocumentFormat.OpenXml
│   │   │   ├── PresentationParser.cs             # PPTX
│   │   │   ├── EpubParser.cs                     # VersOne.Epub
│   │   │   ├── TextParser.cs                     # TXT/MD
│   │   │   └── DocumentParserFactory.cs          # Strategy pattern
│   │   └── DependencyInjection.cs                # Service registration
│   │
│   └── NexusAI.Presentation/                     # → All layers
│       ├── ViewModels/
│       │   ├── MainViewModel.cs                  # Chat, documents, artifacts
│       │   ├── ProjectViewModel.cs               # Kanban board
│       │   ├── WikiViewModel.cs                  # Wiki editor
│       │   ├── PresentationViewModel.cs          # PPTX generator
│       │   ├── SettingsViewModel.cs              # Language, AI provider
│       │   └── ChatMessageViewModel.cs           # Individual message
│       ├── Views/
│       │   ├── MainWindow.xaml                   # ⭐ Dark Neural Glass UI
│       │   ├── ProjectView.xaml                  # Kanban board
│       │   ├── WikiView.xaml                     # Wiki editor
│       │   ├── PresentationView.xaml             # PPTX generator
│       │   └── SettingsView.xaml                 # Language/AI settings
│       ├── Converters/
│       │   ├── BoolToVisibilityConverter.cs
│       │   ├── ModeToStringConverter.cs          # Pro/Student labels
│       │   ├── ModeToAccentColorConverter.cs     # Dynamic colors
│       │   ├── ModeToIconConverter.cs            # Dynamic icons
│       │   └── FileIconConverter.cs
│       ├── Resources/
│       │   ├── Styles/
│       │   │   └── DarkNeuralGlass.xaml          # ⭐ Complete design system
│       │   └── Languages/
│       │       ├── en-US.xaml                    # English strings
│       │       └── ru-RU.xaml                    # Russian strings
│       ├── Services/
│       │   └── LocalizationService.cs            # ResourceDictionary swapping
│       ├── App.xaml                              # DI composition root
│       └── app.manifest                          # Windows manifest
│
├── docs/
│   ├── screenshots/                              # UI screenshots
│   ├── LOCALIZATION.md                           # Localization guide
│   ├── UI_DARK_NEURAL_GLASS.md                   # Design system spec
│   └── screenshots.md
│
├── NexusAI.sln                                   # Multi-project solution
├── README.md                                     # This file
├── LICENSE                                       # MIT License
├── REFACTORING_SUMMARY.md                        # Recent refactoring changelog
├── REFACTORING_COMPLETE.md                       # Architecture documentation
├── CONTRIBUTING.md                               # Contribution guidelines
├── RELEASE_NOTES.md                              # Version history
└── VERSION                                       # Current version number
```

---

## 🔧 Development

### **Building the Project**

```bash
# Clean build
dotnet clean NexusAI.sln
dotnet build NexusAI.sln --configuration Release

# Run tests (when implemented)
dotnet test NexusAI.sln

# Publish self-contained executable
dotnet publish src/NexusAI.Presentation/NexusAI.Presentation.csproj \
  -c Release \
  -r win-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:IncludeNativeLibrariesForSelfExtract=true
```

### **Code Style**

#### **C# 12 Modernization**

```csharp
// ✅ File-scoped namespaces
namespace NexusAI.Domain.Entities;

// ✅ Primary constructors
public sealed class AuthService(AppDbContext context) : IAuthService
{
    public async Task<Result<User>> LoginAsync(...)
    {
        var user = await context.Users.FirstOrDefaultAsync(...);
        return user is not null
            ? Result<User>.Success(user)
            : Result<User>.Failure("User not found");
    }
}

// ✅ Collection expressions
public ICollection<Project> Projects { get; set; } = [];

// ✅ Guard clauses (early returns)
if (string.IsNullOrWhiteSpace(username))
    return Result.Failure("Username cannot be empty");

// Continue main logic without nesting
```

#### **Naming Conventions**

```csharp
// Classes, methods, properties: PascalCase
public class ProjectService { }
public async Task<Result<Project>> CreateProjectAsync() { }

// Private fields: _camelCase
private readonly AppDbContext _context;

// Parameters, local variables: camelCase
public void ProcessData(string fileName) { }

// Constants: PascalCase
public const string DefaultLanguage = "en-US";
```

#### **Architecture Rules**

```diff
+ Use Railway Oriented Programming (Result<T>) for all business logic
+ No exceptions in Domain/Application layers (only Infrastructure)
+ All async methods must have CancellationToken parameter
+ Use ConfigureAwait(false) in library code
+ Inject interfaces, not concrete types
+ One handler per use case
+ ViewModels must not reference Infrastructure
```

### **Database Migrations**

```bash
# Add new migration
cd src/NexusAI.Infrastructure
dotnet ef migrations add MigrationName --context AppDbContext

# Apply migrations
dotnet ef database update

# Rollback
dotnet ef database update PreviousMigrationName

# Generate SQL script
dotnet ef migrations script
```

### **Adding a New Feature**

#### **Example: Add "Export Project to JSON" feature**

```bash
# 1. Create Use Case (Application layer)
src/NexusAI.Application/UseCases/Projects/ExportProjectCommand.cs
src/NexusAI.Application/UseCases/Projects/ExportProjectHandler.cs

# 2. Create Interface (Application layer)
src/NexusAI.Application/Interfaces/IJsonExportService.cs

# 3. Implement Service (Infrastructure layer)
src/NexusAI.Infrastructure/Services/JsonExportService.cs

# 4. Register in DI (Infrastructure)
services.AddSingleton<IJsonExportService, JsonExportService>();

# 5. Update ViewModel (Presentation)
src/NexusAI.Presentation/ViewModels/ProjectViewModel.cs
  → Add ExportProjectCommand

# 6. Update View (Presentation)
src/NexusAI.Presentation/Views/ProjectView.xaml
  → Add "Export to JSON" button
```

### **Testing Strategy**

```csharp
// Unit Tests (Application layer)
[Fact]
public async Task CreateProject_ValidData_ShouldSucceed()
{
    // Arrange
    var service = CreateProjectService();
    
    // Act
    var result = await service.CreateProjectAsync("Title", "Desc", userId);
    
    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Title.Should().Be("Title");
}

// Integration Tests (Infrastructure layer)
[Fact]
public async Task GeminiService_RealApi_ShouldReturnResponse()
{
    // Arrange
    var service = new GeminiAiService(httpClient, apiKey, sessionContext);
    
    // Act
    var result = await service.SendAsync("Hello");
    
    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Should().NotBeEmpty();
}
```

---

## 🗺️ Roadmap

### **v1.1 - Q2 2025** (Planned)

- [ ] **Performance**
  - [ ] Lazy loading for large document lists
  - [ ] Virtual scrolling in chat
  - [ ] Background indexing for RAG

- [ ] **UX Enhancements**
  - [ ] Auto-scroll to latest message
  - [ ] Search within documents
  - [ ] Document preview modal
  - [ ] Keyboard shortcuts (Ctrl+K, Ctrl+P)

- [ ] **Data**
  - [ ] Export chat history to PDF/Word
  - [ ] Import/export projects as JSON
  - [ ] Backup/restore database

### **v2.0 - Q3 2025** (Future)

- [ ] **AI Providers**
  - [ ] OpenAI GPT-4 support
  - [ ] Anthropic Claude integration
  - [ ] Azure OpenAI Service
  - [ ] Custom API endpoint support

- [ ] **Advanced Features**
  - [ ] Vector embeddings (semantic search)
  - [ ] Multi-user collaboration
  - [ ] Cloud sync (optional)
  - [ ] Mobile companion app (Blazor Hybrid)

- [ ] **Integrations**
  - [ ] Notion sync
  - [ ] Confluence integration
  - [ ] Google Drive import
  - [ ] Slack bot

### **v3.0 - 2026** (Vision)

- [ ] **Enterprise**
  - [ ] SSO/SAML authentication
  - [ ] Role-based access control
  - [ ] Audit logging
  - [ ] Self-hosted server option

- [ ] **AI Enhancements**
  - [ ] Fine-tuned models
  - [ ] Custom prompt templates library
  - [ ] Multi-agent workflows
  - [ ] Autonomous task execution

---

## 🤝 Contributing

Contributions are **highly welcome**! Here's how to get started:

### **How to Contribute**

1. **Fork the repository**
   ```bash
   gh repo fork yourusername/NexusAI
   ```

2. **Create a feature branch**
   ```bash
   git checkout -b feature/amazing-feature
   ```

3. **Make your changes**
   - Follow C# 12 coding style
   - Respect Clean Architecture layers
   - Add tests (when test infrastructure is set up)

4. **Commit with conventional commits**
   ```bash
   git commit -m "feat: add export to JSON functionality"
   git commit -m "fix: resolve null reference in ProjectService"
   git commit -m "docs: update README with new feature"
   ```

5. **Push to your fork**
   ```bash
   git push origin feature/amazing-feature
   ```

6. **Open a Pull Request**
   - Describe changes clearly
   - Link related issues
   - Wait for code review

### **Contribution Guidelines**

#### **Code Style**

```csharp
✅ DO:
- Use file-scoped namespaces
- Use primary constructors for services
- Use collection expressions []
- Use guard clauses (early returns)
- Return Result<T> from business logic
- Add XML documentation for public APIs

❌ DON'T:
- Throw exceptions in Domain/Application
- Reference Infrastructure from Application
- Use magic strings (use constants)
- Add "AI comments" that explain what code does
```

#### **Commit Message Format**

```
feat:     New feature
fix:      Bug fix
docs:     Documentation changes
style:    Code style (formatting, no logic change)
refactor: Code restructuring
perf:     Performance improvement
test:     Adding tests
chore:    Build process, dependencies
```

#### **Pull Request Template**

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
How was this tested?

## Checklist
- [ ] Code follows project style
- [ ] Self-review completed
- [ ] Documentation updated
- [ ] No new warnings
```

### **Areas Needing Help**

- 🧪 **Testing**: Unit tests, integration tests
- 🌐 **Localization**: Translations (German, Spanish, French)
- 🎨 **UI/UX**: Design improvements, accessibility
- 📚 **Documentation**: Tutorials, API docs
- 🐛 **Bug Fixes**: Check [Issues](../../issues)

---

## 📄 License

This project is licensed under the **MIT License**.

```
MIT License

Copyright (c) 2024-2025 NexusAI Contributors

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

**TL;DR**: You can freely use, modify, and distribute this software. Just include the original copyright notice.

---

## 🙏 Acknowledgments

Special thanks to the open-source community and these amazing projects:

- **[Google Gemini](https://ai.google.dev/)** - Powerful multimodal AI
- **[Ollama](https://ollama.ai/)** - Local LLM runtime
- **[MaterialDesignInXaml](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit)** - Beautiful WPF components
- **[iText7](https://itextpdf.com/)** - PDF processing
- **[DocumentFormat.OpenXml](https://github.com/OfficeDev/Open-XML-SDK)** - Office file manipulation
- **[CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet)** - MVVM helpers
- **[Entity Framework Core](https://github.com/dotnet/efcore)** - ORM framework

### **Contributors**

<!-- ALL-CONTRIBUTORS-LIST:START -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->
<table>
  <tbody>
    <tr>
      <td align="center"><a href="https://github.com/yourusername"><img src="https://github.com/yourusername.png" width="100px;" alt="Your Name"/><br /><sub><b>Your Name</b></sub></a><br />💻 🎨 📖</td>
    </tr>
  </tbody>
</table>
<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->
<!-- ALL-CONTRIBUTORS-LIST:END -->

---

## 📞 Support & Community

### **Get Help**

- 📖 **Documentation**: [Wiki](../../wiki)
- 💬 **Discussions**: [GitHub Discussions](../../discussions)
- 🐛 **Bug Reports**: [GitHub Issues](../../issues)
- 📧 **Email**: your.email@example.com

### **Stay Updated**

- ⭐ **Star this repo** to receive updates
- 👀 **Watch releases** for new versions
- 🐦 **Follow on Twitter**: [@NexusAI](https://twitter.com/nexusai) (example)

### **Community Guidelines**

We're committed to a welcoming community:
- Be respectful and constructive
- Help others learn
- Follow the [Code of Conduct](CODE_OF_CONDUCT.md)

---

## 📊 Project Stats

![GitHub stars](https://img.shields.io/github/stars/yourusername/NexusAI?style=social)
![GitHub forks](https://img.shields.io/github/forks/yourusername/NexusAI?style=social)
![GitHub issues](https://img.shields.io/github/issues/yourusername/NexusAI)
![GitHub pull requests](https://img.shields.io/github/issues-pr/yourusername/NexusAI)
![GitHub last commit](https://img.shields.io/github/last-commit/yourusername/NexusAI)
![Lines of code](https://img.shields.io/tokei/lines/github/yourusername/NexusAI)

---

<div align="center">

## 🌟 Star History

[![Star History Chart](https://api.star-history.com/svg?repos=yourusername/NexusAI&type=Date)](https://star-history.com/#yourusername/NexusAI&Date)

---

**Built with ❤️ using .NET 8 LTS and modern C# 12**

*NexusAI — Your Intelligent Workspace Companion*

[⬆️ Back to Top](#-nexusai) · [Download](../../releases) · [Documentation](../../wiki) · [Report Bug](../../issues)

</div>
