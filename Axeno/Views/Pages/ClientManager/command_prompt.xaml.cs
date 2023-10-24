﻿using Axeno.Networking.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Axeno.Views.Pages.ClientManager
{
    /// <summary>
    /// Interaction logic for command_prompt.xaml
    /// </summary>
    public partial class command_prompt : Page
    {
        public static Client Client;
        public command_prompt(Client cli)
        {
            InitializeComponent();
            cli.cmd = this;
            Client = cli;
            
        }
    }
}
