using Microsoft.VisualBasic;
using System;
using System.Runtime.Intrinsics.Arm;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Media;
using System.Reflection;
using PushPush.Model;
using PushPush.ViewModel;

namespace PushPush
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow:Window
    {
        private readonly string[] icons = {"./Resources/empty.png",
                                           "./Resources/target.png",
                                           "./Resources/box.png",
                                           "./Resources/inbox.png",
                                           "./Resources/brick.png"};

        private readonly string[] worker = {"./Resources/back.png",
                                            "./Resources/front.png",
                                            "./Resources/left.png",
                                            "./Resources/right.png"};

        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService(typeof(MainViewModel));
        }
        //----------------------------------------------------------------------------------------
    }
}