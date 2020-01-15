using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MassUploader.Core.Processor
{
    public interface IUpload
    {
        Task<Dictionary<Enums.NetworksAvailable, Classes.Result>> UploadNetwork(string filepath);
    }
}
