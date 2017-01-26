using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Vow_win_skiUWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Credits : Page
    {
        public Credits()
        {
            this.InitializeComponent();
            creditsMichal.Text = "Lead UWP Developer\n" +
                                 "Core.CPU\n" +
                                 "GUI Pages: Main Page, CPU, Disc, Processess, Credits";
            creditsDawidW.Text = "UWP Developer\n" +
                                 "Core.IPC\n" +
                                 "GUI Pages: IPC, Lockers, Memory, Credits";
            creditsDaniel.Text = "Core.Lockers";
            creditsDawid.Text = "Core.Memory";
            creditsRobert.Text = "Core.Disc";
            creditsOrczyk.Text = "Core.Processess";
        }
    }
}
