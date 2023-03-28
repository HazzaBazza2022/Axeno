using Axeno.Networking.Connection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
namespace Axeno.Helper
{
    public class ClientsLV : INotifyPropertyChanged
    {
        public Client Client { get; set; }
        public string uid { get; set; }
        public string ipadr { get; set; }
        public string _activewin;
        public string activewin
        {
            get { return _activewin; }
            set { _activewin = value; OnPropertyChanged(); }
        }
        public string _cpuUsage;
        public string cpuUsage
        {
            get { return _cpuUsage; }
            set { _cpuUsage = value; OnPropertyChanged();}
        }
        public string _ramUsage;
        public string ramUsage
        {
            get { return _ramUsage; }
            set { _ramUsage = value; OnPropertyChanged(); }
        }
        public string groupName { get; set; }
        public string clientName { get; set; }
        public string appLevel { get; set; }
        public string operatingSystem { get; set; }
        public string _ping;
        public string ping
        {
            get { return _ping; }
            set { _ping = value; OnPropertyChanged(); }
        }
        public string installDate { get; set; }
        public string version { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
