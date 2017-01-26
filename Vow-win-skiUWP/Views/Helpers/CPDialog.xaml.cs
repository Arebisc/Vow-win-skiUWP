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
using Vow_win_skiUWP.Core.Processes;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Vow_win_skiUWP.Views.Helpers
{
    public sealed partial class CPDialog : ContentDialog
    {
        public CPDialog()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ListBoxItem selectedFile = FileList.SelectedItem as ListBoxItem;
            if(selectedFile != null && ProcessName.Text != string.Empty)
                UserInterface.CreateProcess(ProcessName.Text, selectedFile.Content as string);
        }
    }
}
