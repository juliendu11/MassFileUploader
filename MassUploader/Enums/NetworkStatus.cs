using System;
using System.Collections.Generic;
using System.Text;

namespace MassUploader.Enums
{
    public enum NetworkStatus
    {
        Disabled,
        Enabled,

        Logged,
        LoginError,

        Uploaded,
        UploadingError
    }
}
