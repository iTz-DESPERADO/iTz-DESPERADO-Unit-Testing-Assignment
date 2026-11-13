using DriveNow.Domain.Interfaces;
using DriveNow.Infrastructure.Interfaces;

namespace DriveNow.Infrastructure.External;

public class EmailSender : IEmailSender
{
    readonly TimeProvider _timeProvider;
    readonly ITextFileWriter _files;
    readonly string _outboxFolder;

    public EmailSender(TimeProvider timeProvider, ITextFileWriter files, string outboxFolder)
    {
        _timeProvider = timeProvider;
        _files = files;
        _outboxFolder = outboxFolder;
    }

    public void Send(string to, string subject, string body)
    {
        _files.CreateDirectory(_outboxFolder);
        var path = Path.Combine(_outboxFolder, $"{_timeProvider.GetLocalNow():yyyyMMdd_HHmmssfff}_{to}.txt");
        var content = $"To: {to}{Environment.NewLine}Subject: {subject}{Environment.NewLine}{Environment.NewLine}{body}";
        _files.WriteAllText(path, content);
    }
}
