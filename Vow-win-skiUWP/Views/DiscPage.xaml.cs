using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Vow_win_skiUWP.Core.FileSystem;
using Vow_win_skiUWP.Core.Processes;
using Vow_win_skiUWP.Views.Helpers;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Vow_win_skiUWP.Views
{
    public class DiscPageModel
    {
        public ObservableCollection<Vow_win_skiUWP.Core.FileSystem.File> list { get; set; }
            = Disc.GetDisc.RootFolder.FilesInDirectory;

        public Vow_win_skiUWP.Core.FileSystem.File selectedFile = null;

    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DiscPage : Page
    {
        private DiscPageModel model;

        public DiscPage()
        {
            this.InitializeComponent();
            model = new DiscPageModel();
            this.DataContext = model;
        }

        private async void CFButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new CFDialog();
            await dialog.ShowAsync();
        }
        
        private async void CWButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new CWDialog();
            await dialog.ShowAsync();
        }

        private async void SDBButton_Clicked(object sender, RoutedEventArgs e)
        {
            var dialog = new MemoryPopupDialog("Data blocks", Disc.GetDisc.ShowDataBlocks());
            await dialog.ShowAsync();
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            if(model.list.ToList().Count != 0)
                if(model.selectedFile != null)
                    Disc.GetDisc.SaveToFile(model.selectedFile.FileName, FileContent.Text);
        }

        private void File_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            int selectedItemIndex = listView.SelectedIndex;
            var selectedItem = listView.ContainerFromIndex(selectedItemIndex) as ListViewItem;
            if (selectedItem != null)
            {
                Vow_win_skiUWP.Core.FileSystem.File selectedFile = selectedItem.Content as Vow_win_skiUWP.Core.FileSystem.File;
                model.selectedFile = selectedFile;
                FileContent.Text = Disc.GetDisc.GetFileData(selectedFile.FileName);
            }
        }

        private void DeleteFile_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            Disc.GetDisc.DeleteFile(model.selectedFile.FileName);
            this.FileContent.Text = string.Empty;
        }
    }
}
