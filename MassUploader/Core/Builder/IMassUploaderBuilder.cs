using System;
using System.Collections.Generic;
using System.Text;

namespace MassUploader.Core.Builder
{
    public interface IMassUploaderBuilder
    {
        IMassUploader Build();

        IMassUploaderBuilder Uptobox(string id, string password);
        //IMassUploaderBuilder Mega(string id, string password);
        //IMassUploaderBuilder Dropbox(string id, string password);
        //IMassUploaderBuilder UnFichier(string id, string password);
        //IMassUploaderBuilder Uploaded(string id, string password);
        //IMassUploaderBuilder Turbobit(string id, string password);
        //IMassUploaderBuilder Rapidgator(string id, string password);
        //IMassUploaderBuilder Nitroflare(string id, string password);

        //IMassUploaderBuilder EnableLogs(Enums.LogLevel logLevel);
    }
}
