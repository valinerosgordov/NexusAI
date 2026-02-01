using NexusAI.Application.Interfaces;
using System.IO;

namespace NexusAI.Infrastructure.Services;

public sealed class DocumentParserFactory
{
    private readonly IDocumentParser[] _parsers;

    public DocumentParserFactory(IEnumerable<IDocumentParser> parsers)
    {
        _parsers = parsers.ToArray();
    }

    public IDocumentParser? GetParser(string filePath)
    {
        var extension = Path.GetExtension(filePath);
        return _parsers.FirstOrDefault(p => p.CanParse(extension));
    }

    public IEnumerable<string> GetSupportedExtensions()
    {
        return new[] { ".pdf", ".docx", ".pptx", ".epub", ".txt", ".md" };
    }

    public string GetFileDialogFilter()
    {
        return "All Supported Documents|*.pdf;*.docx;*.pptx;*.epub;*.txt;*.md|" +
               "PDF Files (*.pdf)|*.pdf|" +
               "Word Documents (*.docx)|*.docx|" +
               "PowerPoint Presentations (*.pptx)|*.pptx|" +
               "EPUB Books (*.epub)|*.epub|" +
               "Text Files (*.txt)|*.txt|" +
               "Markdown Files (*.md)|*.md";
    }
}
