﻿using Axeno.Views.Pages.MainWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Axeno.Helper
{
    internal class MainWindowSlides
    {
        public static Frame mainFrame { get; set; }
        public static portManager PortManager { get; set; }
        public static addDevice BuildClientPanel { get; set; }
        public static ClientPanel ClientPanel { get;set;}
    }
}
