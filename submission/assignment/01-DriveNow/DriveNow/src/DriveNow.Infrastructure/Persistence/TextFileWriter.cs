using DriveNow.Infrastructure.Interfaces;

namespace DriveNow.Infrastructure.Persistence;

public class TextFileWriter : ITextFileWriter
{
    public void CreateDirectory(string path) => Directory.CreateDirectory(path);

    public void WriteAllText(string path, string content) => File.WriteAllText(path, content);

    public void AppendAllText(string path, string content) => File.AppendAllText(path, content);
}
