using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MassUploader.Core.Processor;

namespace MassUploader.Core
{
    public class MassUploader : IMassUploader
    {
        private Classes.NetworkSession uptboxSession;
        private Classes.NetworkSession megaSession;
        private Classes.NetworkSession dropboxSession;
        private Classes.NetworkSession unFichierSession;

        private Logger logger;

        private Processor.Upload upload;

        public MassUploader(Classes.NetworkAccount uptoboxAccount, Classes.NetworkAccount megaAccount, Classes.NetworkAccount dropboxAccount, Classes.NetworkAccount unFichierAccount, bool enableLogs)
        {
            if (enableLogs) logger = new Logger();

            if (uptoboxAccount != null)
            {
                uptboxSession = new Classes.NetworkSession(uptoboxAccount);
            }
            if (megaAccount != null)
            {
                megaSession = new Classes.NetworkSession(megaAccount);
            }
            if (dropboxAccount != null)
            {
                dropboxSession = new Classes.NetworkSession(dropboxAccount);
            }
            if (unFichierAccount != null)
            {
                unFichierSession = new Classes.NetworkSession(unFichierAccount);
            }

            upload = new Processor.Upload(uptboxSession, megaSession, dropboxSession, unFichierSession, logger);

        }

        public IUpload Upload => upload;

        public async Task<Dictionary<Enums.NetworksAvailable, Classes.Result>> LoginNetwork()
        {
            if (uptboxSession == null && dropboxSession == null && megaSession == null && unFichierSession == null)
            {
                throw new Exception("No network enabled");
            }
            
            Dictionary<Enums.NetworksAvailable, Classes.Result> results = new Dictionary<Enums.NetworksAvailable, Classes.Result>();

            if (uptboxSession !=null)
            results.Add(Enums.NetworksAvailable.Uptobox, await LoginUptobox());

            return results;
        }

        public async Task<Dictionary<Enums.NetworksAvailable, Classes.Result>> LogoutNetwork()
        {
            if (uptboxSession == null && dropboxSession == null && megaSession == null && unFichierSession == null)
            {
                throw new Exception("No network enabled");
            }

            Dictionary<Enums.NetworksAvailable, Classes.Result> results = new Dictionary<Enums.NetworksAvailable, Classes.Result>();

            if (uptboxSession != null && uptboxSession.Logged)
                results.Add(Enums.NetworksAvailable.Uptobox, await LogoutUptobox());

            return results;
        }

        private async Task<Classes.Result> LoginUptobox()
        {
            Classes.Result result = new Classes.Result();

            HttpResponseMessage first = await uptboxSession.HttpClient.GetAsync(Constant.UPTOBOX);

            var content = new FormUrlEncodedContent(new[]
           {
                new KeyValuePair<string, string>("login", uptboxSession.Account.Id),
                new KeyValuePair<string, string>("password", uptboxSession.Account.Password),
            });

            HttpResponseMessage responseLogin = await uptboxSession.HttpClient.PostAsync(Constant.UPTOBOX_LOGIN, content);

            if (responseLogin.IsSuccessStatusCode)
            {
                result.Success = true;
                uptboxSession.Logged = true;
            }
            else
            {
                result.Success = false;
                result.Message = $"Error login, code: {responseLogin.StatusCode}";
            }

            return result;
        }

        private async Task<Classes.Result> LogoutUptobox()
        {
            Classes.Result result = new Classes.Result();

            HttpResponseMessage first = await uptboxSession.HttpClient.GetAsync(Constant.UPTOBOX);

            string responseBody = await first.Content.ReadAsStringAsync();
            var logoutLink = Helpers.HtmlFind.GetBetween(responseBody, "<a href='https://uptobox.com/logout", "'>");

            if (string.IsNullOrEmpty(logoutLink))
            {
                result.Success = false;
                result.Message = "No logout link found";
                return result;
            }

            HttpResponseMessage logoutRequest = await uptboxSession.HttpClient.GetAsync("https://uptobox.com/logout" + logoutLink);
            if (logoutRequest.IsSuccessStatusCode)
            {
                result.Success = true;
                uptboxSession.Logged = false;
            }
            else
            {
                result.Success = false;
                result.Message = $"Error logout, code: {logoutRequest.StatusCode}";
            }

            return result;
        }
    }
}
