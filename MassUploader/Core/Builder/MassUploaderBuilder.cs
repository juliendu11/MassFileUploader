using System;
using System.Collections.Generic;
using System.Text;

namespace MassUploader.Core.Builder
{
    public class MassUploaderBuilder : IMassUploaderBuilder
    {
        private Classes.NetworkAccount uptoboxAccount;
        private Classes.NetworkAccount megaAccount;
        private Classes.NetworkAccount dropboxAccount;
        private Classes.NetworkAccount unFichierAccount;

        private bool enableLogs = false;

        private MassUploaderBuilder() { }

        public IMassUploader Build()
        {
            return new MassUploader(uptoboxAccount, megaAccount, dropboxAccount, unFichierAccount, enableLogs);
        }

        public IMassUploaderBuilder EnableLogs()
        {
            enableLogs = true;

            return this;
        }

        public IMassUploaderBuilder Dropbox(string id, string password)
        {
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(password))
            {
                dropboxAccount = new Classes.NetworkAccount { Id = id, Password = password };
            }
            return this;
        }


        public IMassUploaderBuilder Mega(string id, string password)
        {
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(password))
            {
                megaAccount = new Classes.NetworkAccount { Id = id, Password = password };
            }
            return this;
        }

        public IMassUploaderBuilder UnFichier(string id, string password)
        {
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(password))
            {
                unFichierAccount = new Classes.NetworkAccount { Id = id, Password = password };
            }
            return this;
        }

        public IMassUploaderBuilder Uptobox(string id, string password)
        {
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(password))
            {
                uptoboxAccount = new Classes.NetworkAccount { Id = id, Password = password };
            }
            return this;
        }

        public static IMassUploaderBuilder CreateBuilder()
        {
            return new MassUploaderBuilder();
        }
    }
}
