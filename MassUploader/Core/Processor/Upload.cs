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

        private Logger logger;

        public Upload(Classes.NetworkSession uptboxSession, Classes.NetworkSession megaSession, Classes.NetworkSession dropboxSession, Classes.NetworkSession unFichierSession, Logger logger)
        {
            this.uptboxSession = uptboxSession;
            this.megaSession = megaSession;
            this.dropboxSession = dropboxSession;
            this.unFichierSession = unFichierSession;
            this.logger = logger;
        }

        public async Task<Dictionary<Enums.NetworksAvailable, Classes.Result>> UploadNetwork(string filepath)
        {
            if (!File.Exists(filepath))
            {
                throw new Exception("No file found in " + filepath);
            }

            if (uptboxSession == null && dropboxSession == null && megaSession == null && unFichierSession == null)
            {
                throw new Exception("No network enabled");
            }

            Dictionary<Enums.NetworksAvailable, Classes.Result> results = new Dictionary<Enums.NetworksAvailable, Classes.Result>();

            if (uptboxSession != null)
            results.Add(Enums.NetworksAvailable.Uptobox, await UptoboxUpload(filepath));

            return results;
        }

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
            }
            else
            {
                result.Success = false;
                result.Message = "Upload error";
            }


            return result;
        }
    }
}
