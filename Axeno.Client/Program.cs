using Axeno.Client.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Axeno.Client
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            while(true)
            {
                Task.Delay(1000);
                try
                {
                    if (!ClientSocket.IsConnected)
                    {

                        ClientSocket.Connect();

                    }
                }
                catch { }
            }
        }
    }
}
