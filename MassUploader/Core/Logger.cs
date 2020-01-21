using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MassUploader.Core
{
    public class Logger
    {
        public ObservableCollection<string> Logs { get; }

        private Enums.LogLevel logLevel;

        internal Logger(Enums.LogLevel logLever)
        {
            Logs = new ObservableCollection<string>();
            this.logLevel = logLever;
        }

        internal void WriteLog(string process, string text, bool isResponseBody=false)
        {
            if (Logs != null)
            {
                if (logLevel == Enums.LogLevel.NoResponseBody && isResponseBody)
                    return;

                Logs.Add("[" + DateTime.Now.ToString() + "][" + process + "]" + text);
            }
        }
    }
}
