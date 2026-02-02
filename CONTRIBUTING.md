# Contributing to Nexus AI

Thank you for your interest in contributing to Nexus AI! 🎉

## 🚀 Getting Started

1. **Fork** the repository
2. **Clone** your fork: `git clone https://github.com/YOUR_USERNAME/NexusAI.git`
3. **Create a branch**: `git checkout -b feature/your-feature-name`
4. **Make changes** and test thoroughly
5. **Commit**: `git commit -m "feat: add amazing feature"`
6. **Push**: `git push origin feature/your-feature-name`
7. **Open a Pull Request** on GitHub

## 📝 Commit Message Convention

We follow [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>(<scope>): <subject>

<body>

<footer>
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, no logic change)
- `refactor`: Code refactoring
- `perf`: Performance improvements
- `test`: Adding or updating tests
- `chore`: Build process, tooling, dependencies

**Examples:**
```
feat(ai): add Claude provider support
fix(parser): handle corrupted PDF files gracefully
docs(readme): update installation instructions
refactor(service): extract context builder to separate class
```

## 🏗️ Code Guidelines

### C# Style

- **Modern C# 12**: Use records, primary constructors, collection expressions
- **Nullable reference types**: Always enabled (`#nullable enable`)
- **No null returns**: Use `Result<T>` or `Option<T>`
- **Async/await**: Use `ConfigureAwait(false)` in library code
- **Naming**: PascalCase for public members, _camelCase for private fields

### Architecture

- **Clean Architecture**: Domain → Application → Infrastructure → Presentation
- **MVVM**: Use CommunityToolkit.Mvvm (`RelayCommand`, `ObservableProperty`)
- **Dependency Injection**: Constructor injection only
- **Railway Oriented Programming**: Always return `Result<T>` for operations that can fail

### Example:

```csharp
// ✅ Good
public sealed class MyService : IMyService
{
    private readonly ILogger<MyService> _logger;
    
    public MyService(ILogger<MyService> logger)
    {
        _logger = logger;
    }
    
    public async Task<Result<Data>> GetDataAsync(CancellationToken ct = default)
    {
        try
        {
            var data = await FetchAsync(ct).ConfigureAwait(false);
            return Result.Success(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch data");
            return Result.Failure<Data>($"Fetch failed: {ex.Message}");
        }
    }
}

// ❌ Bad
public class MyService // not sealed
{
    public Data GetData() // nullable return, sync
    {
        var data = Fetch();
        return data; // could be null
    }
}
```

## 🧪 Testing

- Write unit tests for new features using **xUnit**
- Use **FluentAssertions** for readable assertions
- Mock dependencies with **NSubstitute**
- Aim for 80%+ code coverage on business logic

### Example Test:

```csharp
[Fact]
public async Task AddDocumentAsync_ValidPdf_ShouldSucceed()
{
    // Arrange
    var service = CreateService();
    var pdfPath = "test.pdf";
    
    // Act
    var result = await service.AddDocumentAsync(pdfPath);
    
    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Name.Should().Be("test");
}
```

## 📦 Adding Dependencies

- Prefer **official Microsoft packages** when possible
- Keep dependencies **minimal** — justify each addition
- Update `NexusAI.csproj` and run `dotnet restore`
- Document new dependencies in README if user-facing

## 🐛 Reporting Bugs

Use [GitHub Issues](https://github.com/yourusername/NexusAI/issues) with this template:

**Bug Report:**
```
**Describe the bug**
A clear description of what the bug is.

**To Reproduce**
Steps to reproduce:
1. Open app
2. Click '...'
3. See error

**Expected behavior**
What you expected to happen.

**Screenshots**
If applicable, add screenshots.

**Environment:**
- OS: Windows 11
- .NET Version: 8.0.x
- Nexus AI Version: 1.0.0

**Additional context**
Any other context about the problem.
```

## 💡 Feature Requests

Use [GitHub Discussions](https://github.com/yourusername/NexusAI/discussions) for ideas and feature proposals.

## 🔍 Code Review Process

1. **Automated checks** must pass (build, lints)
2. **Manual review** by maintainer(s)
3. **Feedback** is provided within 72 hours
4. **Revisions** may be requested
5. **Merge** after approval

## 📄 License

By contributing, you agree that your contributions will be licensed under the MIT License.

---

**Thank you for making Nexus AI better!** 🙏
