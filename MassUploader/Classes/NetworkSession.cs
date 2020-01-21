using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Text;

namespace MassUploader.Classes
{
    public class NetworkSession : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private HttpClientHandler httpClientHandler = new HttpClientHandler();

        private Enums.NetworkStatus networkStatus = Enums.NetworkStatus.Disabled;

        internal CookieContainer cookieContainer { get; } = new CookieContainer();

        internal HttpClient HttpClient { get; }

        internal bool Logged { get; set; }

        internal NetworkAccount Account { get; }

        internal NetworkSession(NetworkAccount account)
        {
            this.Account = account;

            if (HttpClient == null)
            {
                httpClientHandler.CookieContainer = cookieContainer;
                HttpClient = new HttpClient(httpClientHandler);
            }

        }

        /// <summary>
        /// Use for binding with WPF MVVM
        /// </summary>
        public Enums.NetworkStatus NetworkStatus
        {
            get => networkStatus;
            set
            {
                if (value != networkStatus)
                {
                    networkStatus = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NetworkStatus"));
                }
            }
        }
    }
}
