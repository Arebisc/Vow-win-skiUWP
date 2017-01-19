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
    public class DiscPageModel : INotifyPropertyChanged
    {
        public ObservableCollection<Vow_win_skiUWP.Core.FileSystem.File> _list { get; set; } = Disc.GetDisc.FileList;

        public ObservableCollection<Vow_win_skiUWP.Core.FileSystem.File> list
        {
            get { return _list; }
            set
            {
                _list = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DiscPage : Page
    {
        public DiscPage()
        {
            this.InitializeComponent();
        }

        private async void CFButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new CFDialog();
            await dialog.ShowAsync();
        }
        
        private void CWButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SDBButton_Clicked(object sender, RoutedEventArgs e)
        {
            //var dialog = new MemoryPopupDialog("Data blocks", Disc.GetDisc.ShowDataBlocks());
            //await dialog.ShowAsync();
            throw new NotImplementedException();
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void File_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            int selectedItemIndex = listView.SelectedIndex;
            var selectedItem = listView.ContainerFromIndex(selectedItemIndex) as ListViewItem;
            Vow_win_skiUWP.Core.FileSystem.File selectedFile = selectedItem.Content as Vow_win_skiUWP.Core.FileSystem.File;
        }

        private void DeleteFile_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
