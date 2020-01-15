using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MassUploader.Core
{
    public class Logger
    {
        public ObservableCollection<string> Logs { get; }

        public Logger()
        {
            Logs = new ObservableCollection<string>();
        }

        public void WriteLog(string process, string text)
        {
            if (Logs != null)
            Logs.Add("[" + DateTime.Now.ToString() + "][" + process + "]" + text);
        }
    }
}
