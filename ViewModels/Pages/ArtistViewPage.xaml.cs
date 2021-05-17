﻿using Player.ViewModels.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Player.ViewModels.Pages
{
    /// <summary>
    /// Interaction logic for ArtistViewPage.xaml
    /// </summary>
    public partial class ArtistViewPage : Page
    {
        public MainViewModel MainViewModel { get; private set; }

        public ArtistViewPage(MainViewModel mainViewModel)
        {
            InitializeComponent();
            MainViewModel = mainViewModel;
        }
    }
}
