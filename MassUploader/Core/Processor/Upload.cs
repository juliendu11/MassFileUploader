using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MassUploader.Core.Processor
{
    public class Upload : IUpload
    {
        private Classes.NetworkSession uptboxSession;
        private Classes.NetworkSession megaSession;
        private Classes.NetworkSession dropboxSession;
        private Classes.NetworkSession unFichierSession;
        private Classes.NetworkSession turbotBitSession;

        private Logger logger;

        public Upload(Classes.NetworkSession uptboxSession, Classes.NetworkSession megaSession, Classes.NetworkSession dropboxSession, Classes.NetworkSession unFichierSession, Classes.NetworkSession turbotBitSession, Logger logger)
        {
            this.uptboxSession = uptboxSession;
            this.megaSession = megaSession;
            this.dropboxSession = dropboxSession;
            this.unFichierSession = unFichierSession;
            this.turbotBitSession = turbotBitSession;
            this.logger = logger;
        }

        /// <summary>
        /// Run upload in parallel for all networks
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public async Task<Dictionary<Enums.NetworksAvailable, Classes.Result>> UploadNetwork(string filepath)
        {
            List<Task> uploadTask;

            if (!File.Exists(filepath))
            {
                throw new Exception("No file found in " + filepath);
            }

            if (uptboxSession == null && dropboxSession == null && megaSession == null && unFichierSession == null&& turbotBitSession ==null)
            {
                throw new Exception("No network enabled");
            }

            uploadTask = new List<Task>();

            Dictionary<Enums.NetworksAvailable, Classes.Result> results = new Dictionary<Enums.NetworksAvailable, Classes.Result>();

            if (uptboxSession != null && uptboxSession.Logged)
            {
                uploadTask.Add(Task.Run(async () => results.Add(Enums.NetworksAvailable.Uptobox, await UptoboxUpload(filepath))));
            }

            if (turbotBitSession != null && turbotBitSession.Logged)
            {
                uploadTask.Add(Task.Run(async () => results.Add(Enums.NetworksAvailable.TurbotBit, await TurbotbitUpload(filepath))));
            }

            await Task.WhenAll(uploadTask);

            return results;
        }

        //private async Task<Classes.Result> UnFichierUpload(string filepath)
        //{
        //    Classes.Result result = new Classes.Result();

        //    HttpResponseMessage first = await uptboxSession.HttpClient.GetAsync(Constant.ONEFICHIER);
        //}

        private async Task<Classes.Result> UptoboxUpload(string filepath)
        {
            Classes.Result result = new Classes.Result();

            string sessID = "";
            HttpResponseMessage first = await uptboxSession.HttpClient.GetAsync(Constant.UPTOBOX);

            //Recover session ID
            IEnumerable<Cookie> responseCookies = uptboxSession.cookieContainer.GetCookies(new Uri(Constant.UPTOBOX)).Cast<Cookie>();
            foreach (Cookie cookie in responseCookies)
            {
                if (cookie.Name == "xfss")
                    if (string.IsNullOrEmpty(cookie.Value))
                    {
                        result.Success = false;
                        result.Message = "Upload error: session cookie not found";
                    }
                    else
                        sessID = cookie.Value;
            }

            //Recover url upload
            string url = Helpers.HtmlFind.GetBetween(await first.Content.ReadAsStringAsync(), "action=\"//", "\" method=\"POST\"");
            url = url.Split('/')[0];


            var multiForm = new MultipartFormDataContent();

            FileStream fs = File.OpenRead(filepath);
            multiForm.Add(new StreamContent(fs), "file[]", Path.GetFileName(filepath));

            HttpResponseMessage responseUploadFile = await uptboxSession.HttpClient.PostAsync("https://" + url + "/upload?sess_id=" + sessID, multiForm);

            if (responseUploadFile.IsSuccessStatusCode)
            {
                result.Success = true;
                uptboxSession.NetworkStatus = Enums.NetworkStatus.Uploaded;

            }
            else
            {
                result.Success = false;
                result.Message = "Upload error";
                uptboxSession.NetworkStatus = Enums.NetworkStatus.UploadingError;

            }


            return result;
        }


        private async Task<Classes.Result> TurbotbitUpload(string filepath)
        {
            Classes.Result result = new Classes.Result();

            HttpResponseMessage first = await turbotBitSession.HttpClient.GetAsync(Constant.TURBOBIT);
            string firstContent = await first.Content.ReadAsStringAsync();

            //Recover url upload
            string url = Helpers.HtmlFind.GetBetween(firstContent, "action=\"//", "\" method=\"POST\"");
            url = url.Split('/')[0];

            string user_id = Helpers.HtmlFind.GetBetween(firstContent, "<input name=\"user_id\" value=\"", "\" type = \"hidden\"/> ");


            if (string.IsNullOrEmpty(user_id))
            {
                result.Success = false;
                result.Message = "Empty user_id";
                return result;
            }

            var multiForm = new MultipartFormDataContent();

            FileStream fs = File.OpenRead(filepath);
            multiForm.Add(new StreamContent(fs), "Filedata", Path.GetFileName(filepath));

            multiForm.Add(new StringContent("fd1"), "apptype");
            multiForm.Add(new StringContent("0"), "folder_id");
            multiForm.Add(new StringContent(user_id), "user_id");

            HttpResponseMessage responseUploadFile = await turbotBitSession.HttpClient.PostAsync("https://" + url + "/", multiForm);

            if (responseUploadFile.IsSuccessStatusCode)
            {
                result.Success = true;
                turbotBitSession.NetworkStatus = Enums.NetworkStatus.Uploaded;

            }
            else
            {
                result.Success = false;
                result.Message = "Upload error";
                turbotBitSession.NetworkStatus = Enums.NetworkStatus.UploadingError;

            }

            return result;
        }
    }
}
