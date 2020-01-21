using System;
using System.Collections.Generic;
using System.Text;

namespace MassUploader.Enums
{
    public enum LogLevel
    {
        /// <summary>
        /// Almost everything is logged even the responses in html of the requests sent
        /// </summary>
        Normal,

        /// <summary>
        /// All except responses in html
        /// </summary>
        NoResponseBody
    }
}
