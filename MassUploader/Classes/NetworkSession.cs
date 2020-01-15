using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace MassUploader.Classes
{
    public class NetworkSession
    {
        private HttpClientHandler httpClientHandler = new HttpClientHandler();

        public CookieContainer cookieContainer { get; } = new CookieContainer();

        public HttpClient HttpClient { get; }

        public bool Logged { get; set; }

        public NetworkAccount Account { get; }

        public NetworkSession(NetworkAccount account)
        {
            this.Account = account;

            if (HttpClient == null)
            {
                httpClientHandler.CookieContainer = cookieContainer;
                HttpClient = new HttpClient(httpClientHandler);
            }

        }
    }
}
