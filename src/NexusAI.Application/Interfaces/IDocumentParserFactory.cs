namespace NexusAI.Application.Interfaces;

public interface IDocumentParserFactory
{
    IDocumentParser? GetParser(string filePath);
    string GetFileDialogFilter();
}
