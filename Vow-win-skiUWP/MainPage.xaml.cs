using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Channels;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Vow_win_skiUWP.Core;
using Vow_win_skiUWP.Core.CPU;
using Vow_win_skiUWP.Core.Processes;
using Vow_win_skiUWP.Log;
using Vow_win_skiUWP.Views.Helpers;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Vow_win_skiUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Dictionary<string, string> _defaultWatermarkDictionary;

        public MainPage()
        {
            this.InitializeComponent();
            InitializeWatermarkDictionary();
            SetUpAllWatermarks();
        }

        private void InitializeWatermarkDictionary()
        {
            _defaultWatermarkDictionary = new Dictionary<string, string>
            {
                {"CmdTb", "Wpisz komendę shella:"},
            };
        }

        private void SetUpAllWatermarks()
        {
            Watermark.SetWatermarkIfEmptyTextBox(CmdTb, _defaultWatermarkDictionary[CmdTb.Name]);
        }

        private void TextBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox selectedTextbox = sender as TextBox;
            Watermark.OnFocused(selectedTextbox, _defaultWatermarkDictionary[selectedTextbox.Name]);
        }

        private void TextBox_OnFocusLost(object sender, RoutedEventArgs e)
        {
            TextBox selectedTextbox = sender as TextBox;
            Watermark.OnFocusLost(selectedTextbox, _defaultWatermarkDictionary[selectedTextbox.Name]);
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void IconsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((sender as ListBox).SelectedItem as ListBoxItem).Name)
            {
                case "ProcessesListBoxItem":
                    RootFrame.Navigate(typeof(Views.ProcessesPage));
                    break;
                case "MemoryListBoxItem":
                    RootFrame.Navigate(typeof(Views.MemoryPage));
                    break;
                case "IPCListBoxItem":
                    RootFrame.Navigate(typeof(Views.IPCPage));
                    break;
                case "LockersListBoxItem":
                    RootFrame.Navigate(typeof(Views.LockersPage));
                    break;
                case "CPUListBoxItem":
                    RootFrame.Navigate(typeof(Views.CPUPage));
                    break;
                case "DiscListBoxItem":
                    RootFrame.Navigate(typeof(Views.DiscPage));
                    break;
            }
        }

        private void Cmd_EnterPressed(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                Cmd_ExecuteCommand(CmdTb.Text);
                CmdTb.Text = string.Empty;
                this.Focus(FocusState.Programmatic);
            }
        }

        //TODO
        private async void Cmd_ExecuteCommand(string command)
        {
            var popup = new HelpPopupDialog();
            popup.setText("Jeszcze nie działam!");
            await popup.ShowAsync();
        }

        private async void HelpButton_OnClick(object sender, RoutedEventArgs e)
        {
            var popup = new HelpPopupDialog();
            popup.setText(GetHelp());
            await popup.ShowAsync();
        }

        private string GetHelp()
        {
            string result = String.Empty;
            result += "\n"
                      + "Parametry: [opcjonalny] {wymagany}\n"
                      + "\n"
                      + "\n"
                      + "Polecenia\t\t   Opis\n"
                      + "-------------------------------Ogólne---------------------------------\n"
                      + "HELP\t\t   Wyświetla tę listę\n"
                      + "EX [-all]\t   Wykonuje kolejny rozkaz, [-all] - wszystkie rozkazy\n"
                      + "QUIT\t\t   Zamyka system\n"
                      + "\n"
                      + "------------------------------Procesor--------------------------------\n"
                      + "SRP\t\t   Wyświetla listę procesów Ready\n"
                      + "SRG\t\t   Wyświetla rejestry procesora\n"
                      + "\n"
                      + "-------------------------------Procesy--------------------------------\n"
                      + "CP {nazwa} {prog}  Tworzy proces {nazwa} z programu {prog} na dysku Windows\n"
                      + "CPD {nazwa} {prog} Tworzy proces {nazwa} z programu {prog} na dysku systemu\n"
                      + "CPP {nazwa} {pr}   Ustawia priorytet procesu {nazwa} na {pr}\n"
                      + "NPR {nazwa/\"-all\"} Uruchom nowy proces {nazwa}, {-all} - wszystkie procesy\n"
                      + "HP {nazwa}\t   Zatrzymuje proces {nazwa}\n"
                      + "SAP\t\t   Wyświetla listę wszystkich procesów\n"
                      + "SPCB {nazwa}\t   Wyświetla listę PCB procesu {nazwa}\n"
                      + "WP {nazwa}\t   Usypia uruchomiony proces {nazwa}\n"
                      + "RP {nazwa}\t   Wznawia uśpiony proces {nazwa}\n"
                      + "\n"
                      + "-------------------------------Pamięć---------------------------------\n"
                      + "SPL {nazwa}\t   Wyświetla listę stron procesu {nazwa}\n"
                      + "SPC {nazwa} {nr}   Wyświetla zawartość strony {nr} procesu {nazwa}\n"
                      + "SEP\t\t   Wyświetla puste stron\n"
                      + "SM\t\t   Wyświetla całą pamięć\n"
                      + "SLM\t\t   Wyświetla ostatnią wiadomość z pamięci\n"
                      + "SFIFO\t\t   Wyświetla kolejke FIFO\n"
                      + "\n"
                      + "-----------------------------Komunikacja------------------------------\n"
                      + "SAM\t\t   Wyświetla wszystkie oczekujące komunikaty\n"
                      + "SMH\t\t   Wyświetla historię komunikatów\n"
                      + "\n"
                      + "----------------------------Synchronizacja----------------------------\n"
                      + "SW\t\t   Wyświetla procesy oczekujące pod zamkiem komunikatów\n"
                      + "\n"
                      + "--------------------------------Dysk----------------------------------\n"
                      + "DIR/LS\t\t   Wyświetla listę plików\n"
                      + "CW {nazwa} {plik}  Tworzy plik {nazwa} i wypełnia danymi z {plik} Windows\n"
                      + "CF {nazwa} [dane]  Tworzy plik {nazwa} i wypełnia [dane]\n"
                      + "APP {nazwa} [dane] Dołącza [dane] do pliku {nazwa}\n"
                      + "TYPE {nazwa}\t   Wyświetla dane z pliku {nazwa}\n"
                      + "DF {nazwa}\t   Usuwa plik {nazwa}\n"
                      + "SDB [nr]\t   Wyświetla dane wszystkich bloków, [nr] bloków na ekran\n";

            return result;
        }

        private async void NextOrderButton_OnClick(object sender, RoutedEventArgs e)
        {
            Scheduler.GetInstance.PriorityAlgorithm().Name = "NazwaInna";

            //var popup = new HelpPopupDialog();
            //popup.setText("Jeszcze nie działam!");
            //await popup.ShowAsync();
        }
    }
}
