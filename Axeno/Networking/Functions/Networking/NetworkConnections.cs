using Axeno.Helper;
using Axeno.MessagePack;
using Axeno.Networking.Connection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using static Axeno.Networking.Functions.Networking.NetworkConnections;

namespace Axeno.Networking.Functions.Networking
{
    internal class NetworkConnections
    {
        public static byte[] Send()
        {
            MsgPack mpack = new MsgPack();
            mpack.ForcePathObject("Packet").AsString = "GetNetConnections";
            return mpack.Encode2Bytes();
        }
        public void Handle(Client cli, MsgPack mpack)
        {
            string networkConn_unpack = mpack.ForcePathObject("Data").AsString;
            string substring = networkConn_unpack.Substring(0, networkConn_unpack.Length - 11);

            string[] networkconnections = substring.Split(new[] { "(SPLITHERE)" }, StringSplitOptions.None);
            Task.Run(() =>
            {

                foreach (string networkConn in networkconnections)
                {
                    NetworkConectionsLV lvitm = new NetworkConectionsLV();

                    lvitm.process = Between("Process(", ")", networkConn);
                    lvitm.localaddr = Between("Localaddr(", ")", networkConn);
                    lvitm.localport = Between("Localport(", ")", networkConn);
                    lvitm.remoteaddr = Between("Remoteaddr(", ")", networkConn);
                    lvitm.remoteport = Between("Remoteport(", ")", networkConn);
                    lvitm.protocol = Between("Protocol(", ")", networkConn);
                    lvitm.state = Between("State(", ")", networkConn);
                    cli.netCon.Dispatcher.Invoke(() =>
                    {
                        cli.netCon.lvinfo.Items.Add(lvitm);
                    });
                }
            }).ContinueWith((t) =>
            {
                cli.netCon.Dispatcher.Invoke(() =>
                {
                    cli.netCon.progring.Visibility = Visibility.Hidden;
                    cli.netCon.lvinfo.IsEnabled = true;
                });
            });

        }
        public static string Between(string startDelimiter, string endDelimiter, string inputString)
        {
            // define the regular expression pattern
            string pattern = string.Format("{0}(.*?){1}", Regex.Escape(startDelimiter), Regex.Escape(endDelimiter));

            // use Regex.Match to find the first occurrence of the pattern in the input string
            Match match = Regex.Match(inputString, pattern);

            // check if a match was found
            if (match.Success)
            {
                // get the captured group value
                return match.Groups[1].Value;
            }
            else
            {
                return string.Empty;
            }
        }



    }
}
