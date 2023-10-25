using Axeno.MessagePack;
using Axeno.Networking.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Axeno.Networking.Communication
{
    internal class HandleCMD
    {
        public void HandleResponse(Client cli, MsgPack msgpck)
        {
            try
            {
                cli.cmd.tbcommand_Copy.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                cli.cmd.tbcommand_Copy.VerticalContentAlignment = System.Windows.VerticalAlignment.Top;

                string response = msgpck.ForcePathObject("Response").AsString;
                cli.cmd.tbcommand_Copy.Text += response;
                cli.cmd.tbcommand_Copy.ScrollToEnd();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
