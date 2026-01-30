# Changelog

All notable changes to NexusAI will be documented in this file.

## [2.0.0] - 2024-01-30

### 🎉 Major Features: NotebookLM Advanced Features

#### Added
- **Studio Mode / Deep Dive Analysis**: Generate comprehensive, structured analysis of all included sources
  - Executive summary generation
  - Core concepts identification (5-7 themes)
  - Key findings extraction
  - Cross-source connections and patterns
  - Practical implications
  - Further exploration suggestions
  - Specialized prompt optimized for synthesis
  
- **Citation Highlighting System**
  - Real-time citation extraction from AI responses
  - Visual highlighting of citations in blue, bold text
  - Hover tooltips showing source names
  - Citation summary at bottom of AI messages
  - Source highlighting in source panel when cited
  
- **Folder-Specific Obsidian Scanning**
  - Optional subfolder parameter for targeted loading
  - Load notes from specific folders (e.g., "Research/AI")
  - Faster loading for large vaults
  - Focused analysis on specific projects/topics
  
- **Enhanced Obsidian Export**
  - Improved YAML frontmatter with timestamps
  - `source_count` metadata field
  - `knowledge-hub` tag
  - Source links section with Obsidian `[[WikiLinks]]`
  - Automatic backlinks in source documents
  - Enhanced discoverability in Obsidian graph view

- **Visual Feedback System**
  - Source highlighting with blue borders when cited
  - Type icons: 📄 for PDFs, 📝 for Obsidian notes
  - Highlighted state for active/cited sources
  - Improved UI contrast and visibility

#### Enhanced
- Source management UI with better visual hierarchy
- Chat message display with rich text formatting
- Status messages with emoji indicators
- Export workflow with source tracking

#### Technical
- `CitationHighlightConverter`: WPF converter for rich text citation display
- `GenerateDeepDiveAsync`: Method for Studio Mode analysis
- Enhanced `ObsidianService` with subfolder support
- Source link generation in export pipeline
- `SourceDocumentViewModel` with highlight state management

---

## [1.0.0] - 2024-01-30

### 🚀 Initial Release

#### Core Features
- **PDF Document Loading**
  - iText7 integration for text extraction
  - Full document parsing
  - Error handling for corrupted/protected PDFs
  
- **Obsidian Integration**
  - Direct file system access
  - Recursive .md file scanning
  - Full content loading
  
- **AI-Powered Q&A**
  - Google Gemini 1.5 Flash integration
  - Context-aware responses
  - Source citation enforcement
  - System prompt for research assistant behavior
  
- **Source Management**
  - Include/exclude checkboxes
  - Add/remove sources
  - Clear all functionality
  - Source list with metadata (type, loaded time)
  
- **Chat Interface**
  - User/Assistant message display
  - Timestamp for each message
  - Chat history persistence (in-memory)
  - Clear chat functionality
  
- **Export to Obsidian**
  - YAML frontmatter generation
  - `ai-generated` tag
  - Date metadata
  - Export to `AI_Notebook/` folder
  - Automatic filename sanitization

#### Architecture
- **Clean Architecture** implementation
  - Domain layer: Entities, Result<T> monad
  - Application layer: Interfaces, services
  - Infrastructure layer: Concrete implementations
  - Presentation layer: WPF UI, MVVM
  
- **MVVM Pattern**
  - CommunityToolkit.Mvvm for commands
  - ObservableObject base class
  - RelayCommand for UI bindings
  
- **Railway Oriented Programming**
  - `Result<T>` for error handling
  - No null returns
  - Explicit success/failure states
  
- **Dependency Injection**
  - Microsoft.Extensions.DependencyInjection
  - Service registration
  - Singleton lifetime for stateful services

#### Technical Stack
- .NET 8 with C# 12
- WPF for desktop UI
- iText7 for PDF parsing
- Google Gemini 1.5 Flash API
- System.Text.Json for serialization
- Modern C# features:
  - Records for DTOs
  - Pattern matching
  - Primary constructors
  - Collection expressions
  - Nullable reference types

#### UI/UX
- Modern, clean interface design
- Three-panel layout:
  - Left: Source management
  - Center: Chat interface
  - Top: Settings bar
  - Bottom: Status bar
- Color scheme:
  - Primary: #2563eb (blue)
  - Secondary: #64748b (gray)
  - Background: #f8fafc (light gray)
  - Surface: #ffffff (white)
- Rounded corners and modern styling
- Responsive layout
- Progress indicators for async operations

#### Documentation
- Comprehensive README.md
- Architecture documentation
- Usage instructions
- Development guidelines
- License (MIT)
- .gitignore for .NET projects

---

## Roadmap

### Planned for v2.1
- [ ] Auto-scroll to latest message in chat
- [ ] Search within sources
- [ ] Source preview modal
- [ ] Token usage display
- [ ] Cost estimation

### Planned for v3.0
- [ ] SQLite persistence (sources, chat history)
- [ ] Multiple conversation threads
- [ ] Conversation search
- [ ] Export to PDF/Word
- [ ] OCR support for scanned PDFs
- [ ] Vector embeddings for large document sets
- [ ] Semantic search across sources

### Future Considerations
- [ ] Dark mode UI theme
- [ ] Multiple AI provider support (OpenAI, Claude)
- [ ] Prompt templates
- [ ] Source snippets in responses
- [ ] Audio export (TTS)
- [ ] Mobile companion app
- [ ] Cloud sync (optional)

---

## Version History

| Version | Date | Description |
|---------|------|-------------|
| 2.0.0 | 2024-01-30 | NotebookLM advanced features |
| 1.0.0 | 2024-01-30 | Initial release |

---

## Breaking Changes

### v2.0.0
- `LoadObsidianVaultAsync` now accepts optional `subfolder` parameter
- `SaveNoteAsync` signature changed to include `sourceLinks` parameter
- `IObsidianService` interface updated with new parameters

### Migration from v1.0.0 to v2.0.0
No migration needed for users. Breaking changes only affect code-level API.
All v1.0.0 functionality remains compatible.

---

## Contributors

- Initial development: [Your Name]
- Architecture design: Clean Architecture + DDD principles
- UI/UX: Modern WPF with MVVM
- Documentation: Comprehensive guides and examples

---

## License

MIT License - See LICENSE file for details

---

**Built with ❤️ and .NET 8**
