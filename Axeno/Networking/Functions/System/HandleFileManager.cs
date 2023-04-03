using Axeno.Helper;
using Axeno.MessagePack;
using Axeno.Networking.Connection;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Axeno.Networking.Functions.Networking
{
    internal class HandleFileManager
    {
        public static byte[] Initiate()
        {
            MsgPack mpack = new MsgPack();
            mpack.ForcePathObject("Packet").AsString = "GetDrives";
            return mpack.Encode2Bytes();
        }
        public static byte[] GetDirectory(string dir, bool goBack)
        {
            if (goBack)
            {
                if (dir.Equals(""))
                {
                    return Initiate();
                }
            }
            MsgPack mpack = new MsgPack();
            mpack.ForcePathObject("Packet").AsString = "GetDirectory";
            mpack.ForcePathObject("Directory").AsString = dir;

            return mpack.Encode2Bytes();
        }
        public void HandleFiles(Client cli, MsgPack mpack)
        {
            try
            {
                string files_unpack = mpack.ForcePathObject("Data").AsString;
                string substring = files_unpack.Substring(0, files_unpack.Length - 11);

                string[] files = substring.Split(new[] { "<SPLITHERE>" }, StringSplitOptions.None);
                Task.Run(async () =>
                {
                    List<FileManagerlv> folderItems = new List<FileManagerlv>();
                    List<FileManagerlv> fileItems = new List<FileManagerlv>();
                    foreach (string file in files)
                    {
                        if (file != "(EMPTYFOLDER)")
                        {
                            string fName = Between("Name<", ">", file);
                            string fType = Between("Type<", ">", file);
                            string fLast = Between("Access<", ">", file);
                            string fSize = Between("Size<", ">", file);
                            string fullpath = Between("Path<", ">", file);
                            string parent = Between("Parent<", ">", file);
                            string icon = Between("Icon<", ">", file);
                            if (icon == "FOLDER" || icon == "DRIVE")
                            {
                                Bitmap iconimg = icon == "FOLDER" ? Properties.Resources.folderico : Properties.Resources.Drive;
                                BitmapSource bmp = ConvertBitmap(iconimg);
                                bmp.Freeze(); 

                                FileManagerlv fileInfoWithIcon = new FileManagerlv
                                {
                                    fName = fName,
                                    fType = fType,
                                    fLast = fLast,
                                    fSize = fSize,
                                    fullpath = fullpath,
                                    parent = parent,
                                    fIcon = bmp
                                };

                                if (fType == "Folder" || fType == "Drive")
                                {
                                    folderItems.Add(fileInfoWithIcon);
                                }
                                else
                                {
                                    fileItems.Add(fileInfoWithIcon);
                                }
                            }
                            else
                            {
                                byte[] iconbyte = Convert.FromBase64String(icon);
                                using (MemoryStream ms = new MemoryStream(iconbyte))
                                {
                                    try
                                    {
                                        Image iconimg = Image.FromStream(ms);
                                        ms.Position = 0; // Reset MemoryStream position
                                        BitmapSource iconSource = BitmapFrame.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                                        iconSource.Freeze(); // Freeze the BitmapSource

                                        FileManagerlv fileInfoWithIcon = new FileManagerlv
                                        {
                                            fName = fName,
                                            fType = fType,
                                            fLast = fLast,
                                            fSize = fSize,
                                            fullpath = fullpath,
                                            parent = parent,
                                            fIcon = iconSource
                                        };

                                        if (fType == "Folder")
                                        {
                                            folderItems.Add(fileInfoWithIcon);
                                        }
                                        else
                                        {
                                            fileItems.Add(fileInfoWithIcon);
                                        }
                                    }
                                    catch (ArgumentException e)
                                    {
                                        MessageBox.Show(e.Message);
                                    }
                                }
                            }
                        }
                    }

                    await cli.fManager.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        cli.fManager.lvinfo.ItemsSource = folderItems.Concat(fileItems);
                    }));

                }).ContinueWith((t) =>
                {
                    cli.fManager.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        cli.fManager.progring.Visibility = Visibility.Hidden;
                        cli.fManager.lvinfo.IsEnabled = true;
                    }));

                });
            }
            catch (Exception)
            {
                MessageBox.Show("Access Denied", "File Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                cli.Send(Initiate());
            }
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
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public static BitmapSource ConvertBitmap(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
            }
        }



    }
}
