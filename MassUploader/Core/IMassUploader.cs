using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MassUploader.Core
{
    public interface IMassUploader
    {
        public Processor.IUpload Upload { get; }

        Task<Dictionary<Enums.NetworksAvailable, Classes.Result>> LoginNetwork();

        Task<Dictionary<Enums.NetworksAvailable, Classes.Result>> LogoutNetwork();
    }
}
