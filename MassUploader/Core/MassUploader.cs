using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MassUploader.Classes;
using MassUploader.Core.Processor;

namespace MassUploader.Core
{
    public class MassUploader : IMassUploader
    {
        private Classes.NetworkSession uptboxSession;
        private Classes.NetworkSession megaSession;
        private Classes.NetworkSession dropboxSession;
        private Classes.NetworkSession unFichierSession;
        private Classes.NetworkSession turbotBitSession;

        private Logger logger;

        private Processor.Upload upload;

        public MassUploader(Classes.NetworkAccount uptoboxAccount, Classes.NetworkAccount megaAccount, Classes.NetworkAccount dropboxAccount, Classes.NetworkAccount unFichierAccount,Classes.NetworkAccount turbotBitAccount, bool enableLogs, Enums.LogLevel logLevel)
        {
            if (enableLogs) logger = new Logger(logLevel);

            if (uptoboxAccount != null)
            {
                uptboxSession = new Classes.NetworkSession(uptoboxAccount);
                uptboxSession.NetworkStatus = Enums.NetworkStatus.Enabled;

                if (logger != null) logger.WriteLog("Build", $"Uptobox is enabled, {uptoboxAccount.Id}:{uptoboxAccount.Password}");
            }
            if (megaAccount != null)
            {
                megaSession = new Classes.NetworkSession(megaAccount);
                megaSession.NetworkStatus = Enums.NetworkStatus.Enabled;
                if (logger != null) logger.WriteLog("Build", $"MEGA is enabled, {megaAccount.Id}:{megaAccount.Password}");

            }
            if (dropboxAccount != null)
            {
                dropboxSession = new Classes.NetworkSession(dropboxAccount);
                dropboxSession.NetworkStatus = Enums.NetworkStatus.Enabled;
                if (logger != null) logger.WriteLog("Build", $"Dropbox is enabled, {dropboxAccount.Id}:{dropboxAccount.Password}");

            }
            if (unFichierAccount != null)
            {
                unFichierSession = new Classes.NetworkSession(unFichierAccount);
                unFichierSession.NetworkStatus = Enums.NetworkStatus.Enabled;
                if (logger != null) logger.WriteLog("Build", $"1Fichier is enabled, {unFichierAccount.Id}:{unFichierAccount.Password}");

            }
            if (turbotBitAccount != null)
            {
                turbotBitSession = new NetworkSession(turbotBitAccount);
                turbotBitSession.NetworkStatus = Enums.NetworkStatus.Enabled;
                if (logger != null) logger.WriteLog("Build", $"TurbotBit is enabled, {turbotBitAccount.Id}:{turbotBitAccount.Password}");

            }

            upload = new Processor.Upload(uptboxSession, megaSession, dropboxSession, unFichierSession,turbotBitSession, logger);

        }

        public IUpload Upload => upload;

        public NetworkSession UptoboxSession => uptboxSession;

        public NetworkSession TurbotBitSession => turbotBitSession;

        public Logger Logger => logger;

        public async Task<Dictionary<Enums.NetworksAvailable, Classes.Result>> LoginNetwork()
        {
            List<Task> loginTask;

            if (logger != null) logger.WriteLog("Login network", "Login in preogress");


            if (uptboxSession == null && dropboxSession == null && megaSession == null && unFichierSession == null && turbotBitSession ==null)
            {
                if (logger != null) logger.WriteLog("Login network", "No network enabled");
                throw new Exception("No network enabled");
            }

            loginTask = new List<Task>();

            Dictionary<Enums.NetworksAvailable, Classes.Result> results = new Dictionary<Enums.NetworksAvailable, Classes.Result>();

            if (uptboxSession != null)
            {
                if (logger != null) logger.WriteLog("Login network", "Login uptobox");

                loginTask.Add(Task.Run(async () => results.Add(Enums.NetworksAvailable.Uptobox, await LoginUptobox())));
            }

            if (turbotBitSession != null)
            {
                if (logger != null) logger.WriteLog("Login network", "Login turbobit");

                loginTask.Add(Task.Run(async () => results.Add(Enums.NetworksAvailable.TurbotBit, await LoginTurbotBit())));
            }

            if (unFichierSession != null)
            {
                if (logger != null) logger.WriteLog("Login network", "Login 1Fichier");

                loginTask.Add(Task.Run(async () => results.Add(Enums.NetworksAvailable.UnFichier, await LoginUnFichier())));
            }

            await Task.WhenAll(loginTask);

            if (logger != null) logger.WriteLog("Login network", "Finished");

            return results;
        }

        public async Task<Dictionary<Enums.NetworksAvailable, Classes.Result>> LogoutNetwork()
        {
            List<Task> logoutTask;

            if (uptboxSession == null && dropboxSession == null && megaSession == null && unFichierSession == null)
            {
                throw new Exception("No network enabled");
            }

            logoutTask = new List<Task>();

            Dictionary<Enums.NetworksAvailable, Classes.Result> results = new Dictionary<Enums.NetworksAvailable, Classes.Result>();

            if (uptboxSession != null && uptboxSession.Logged)
            {
                logoutTask.Add(Task.Run(async () => results.Add(Enums.NetworksAvailable.Uptobox, await LogoutUptobox())));
            }


            await Task.WhenAll(logoutTask);

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
            string responseBody = await responseLogin.Content.ReadAsStringAsync();

            if (responseLogin.IsSuccessStatusCode)
            {
                result.Success = true;
                uptboxSession.Logged = true;
                uptboxSession.NetworkStatus = Enums.NetworkStatus.Logged;
            }
            else
            {
                result.Success = false;
                result.Message = $"Error login, code: {responseLogin.StatusCode}";
                uptboxSession.NetworkStatus = Enums.NetworkStatus.LoginError;

            }

            return result;
        }

        private async Task<Classes.Result> LoginTurbotBit()
        {
            Classes.Result result = new Classes.Result();

            HttpResponseMessage first = await turbotBitSession.HttpClient.GetAsync(Constant.TURBOBIT);

            var content = new FormUrlEncodedContent(new[]
          {
                new KeyValuePair<string, string>("user[login]", turbotBitSession.Account.Id),
                new KeyValuePair<string, string>("user[pass]", turbotBitSession.Account.Password),
                new KeyValuePair<string, string>("user[captcha_type]", ""),
                new KeyValuePair<string, string>("user[captcha_subtype]", "Se connecter"),
                new KeyValuePair<string, string>("user[memory]", "on")
            });

            HttpResponseMessage responseLogin = await turbotBitSession.HttpClient.PostAsync(Constant.TURBOBIT_LOGIN, content);
            string responseBody = await responseLogin.Content.ReadAsStringAsync();

            if (responseBody.Contains("Please enter the captcha code."))
            {
                result.Success = false;
                result.Message = $"Error login, captcha required";
                turbotBitSession.NetworkStatus = Enums.NetworkStatus.LoginError;
                return result;
            }

            if (responseBody.Contains("E-Mail address appears to be invalid. Please try again"))
            {
                result.Success = false;
                result.Message = $"Error login, bad email";
                turbotBitSession.NetworkStatus = Enums.NetworkStatus.LoginError;
                return result;
            }

            if (responseLogin.IsSuccessStatusCode)
            {
                result.Success = true;
                turbotBitSession.Logged = true;
                turbotBitSession.NetworkStatus = Enums.NetworkStatus.Logged;

            }
            else
            {
                result.Success = false;
                result.Message = $"Error login, code: {responseLogin.StatusCode}";
                turbotBitSession.NetworkStatus = Enums.NetworkStatus.LoginError;

            }

            return result;
        }

        private async  Task<Classes.Result> LoginUnFichier()
        {
            Classes.Result result = new Classes.Result();

            HttpResponseMessage first = await unFichierSession.HttpClient.GetAsync(Constant.ONEFICHIER);

            var content = new FormUrlEncodedContent(new[]
         {
                new KeyValuePair<string, string>("mail", unFichierSession.Account.Id),
                new KeyValuePair<string, string>("pass", unFichierSession.Account.Password),
                new KeyValuePair<string, string>("It", "on"),
                new KeyValuePair<string, string>("purge", "on"),
                new KeyValuePair<string, string>("valider", "Envoyer")
            });

            HttpResponseMessage responseLogin = await unFichierSession.HttpClient.PostAsync(Constant.ONEFICHIER_LOGIN, content);
            string responseBody = await responseLogin.Content.ReadAsStringAsync();

            if (responseBody.Contains("Adresse email incorrecte.") || responseBody.Contains("Mot de passe incorrect.") || responseBody.Contains("Utilisateur Inconnu."))
            {
                result.Success = false;
                if (responseBody.Contains("Adresse email incorrecte."))
                    result.Message = "Error login, incorrect email";
                if (responseBody.Contains("Mot de passe incorrect."))
                    result.Message = "Error login, incorrect password";
                if (responseBody.Contains("Utilisateur Inconnu."))
                    result.Message = "Error login, user not found";
                unFichierSession.NetworkStatus = Enums.NetworkStatus.LoginError;
                return result;
            }

            if (responseLogin.IsSuccessStatusCode)
            {
                result.Success = true;
                unFichierSession.Logged = true;
                unFichierSession.NetworkStatus = Enums.NetworkStatus.Logged;

            }
            else
            {
                result.Success = false;
                result.Message = $"Error login, code: {responseLogin.StatusCode}";
                unFichierSession.NetworkStatus = Enums.NetworkStatus.LoginError;

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
