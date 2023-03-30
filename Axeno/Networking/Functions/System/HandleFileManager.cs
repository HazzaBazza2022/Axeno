using Axeno.Helper;
using Axeno.MessagePack;
using Axeno.Networking.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.SessionState;
using System.Windows;

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
                Task.Run(() =>
                {
                    List<FileManagerlv> folderItems = new List<FileManagerlv>();
                    List<FileManagerlv> fileItems = new List<FileManagerlv>();

                    foreach (string file in files)
                    {
                        FileManagerlv lvitm = new FileManagerlv();
                        lvitm.fName = Between("Name<", ">", file);
                        lvitm.fType = Between("Type<", ">", file);
                        lvitm.fLast = Between("Access<", ">", file);
                        lvitm.fSize = Between("Size<", ">", file);
                        lvitm.fullpath = Between("Path<", ">", file);
                        lvitm.parent = Between("Parent<", ">", file);

                        if (lvitm.fType == "Folder")
                        {
                            folderItems.Add(lvitm);
                        }
                        else
                        {
                            fileItems.Add(lvitm);
                        }
                    }

                    cli.fManager.Dispatcher.Invoke(() =>
                    {
                        foreach (FileManagerlv lvitm in folderItems)
                        {
                            cli.fManager.lvinfo.Items.Add(lvitm);
                        }

                        foreach (FileManagerlv lvitm in fileItems)
                        {
                            cli.fManager.lvinfo.Items.Add(lvitm);
                        }
                    });

                }).ContinueWith((t) =>
                {
                    cli.fManager.Dispatcher.Invoke(() =>
                    {
                        cli.fManager.progring.Visibility = Visibility.Hidden;
                        cli.fManager.lvinfo.IsEnabled = true;
                    });
                });
            }catch(Exception)
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
    }
}
