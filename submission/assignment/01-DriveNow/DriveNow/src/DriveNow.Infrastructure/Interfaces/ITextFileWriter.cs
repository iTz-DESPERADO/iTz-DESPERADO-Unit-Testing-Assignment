namespace DriveNow.Infrastructure.Interfaces;

public interface ITextFileWriter
{
    void CreateDirectory(string path);
    void WriteAllText(string path, string content);
    void AppendAllText(string path, string content);
}
