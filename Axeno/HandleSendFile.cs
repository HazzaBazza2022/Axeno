using Axeno.MessagePack;
using Axeno.Networking.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Axeno
{
    internal class HandleSendFile
    {
        public void HandleResult(Client cli, MsgPack msgpack)
        {
            string result = msgpack.ForcePathObject("Result").AsString;
            cli.sendFile.btnexec.IsEnabled = true;
            cli.sendFile.btnexec.Content = "Execute";
            switch (result)
            {
                case "Success":
                    {
                        MessageBox.Show("Your file was successfully executed!", "Axeno", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    }
                case "Failure":
                    {
                        MessageBox.Show("Your file was unable to be executed. Please try again.", "Axeno", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    }
            }
        }
    }
}
