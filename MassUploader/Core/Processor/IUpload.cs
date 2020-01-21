using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MassUploader.Core.Processor
{
    public interface IUpload
    {
        /// <summary>
        /// Run upload in parallel for all networks
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        Task<Dictionary<Enums.NetworksAvailable, Classes.Result>> UploadNetwork(string filepath);
    }
}
