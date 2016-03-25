using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace DumpTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<Tuple<string, string>> _fullList = new List<Tuple<string, string>>();
        public static List<Tuple<string, string>> _regionList = new List<Tuple<string, string>>();
        Dictionary<string, List<Tuple<string, string>>> _approxCountryBuckets = new Dictionary<string, List<Tuple<string, string>>>();
        public MainWindow()
        {
            DataContext = this;
            Resources["regioncreds"] = _regionList;
            InitializeComponent();
        }
        private void PartitionByLocation()
        {
            

            foreach (var item in _fullList)
            {
                string [] afterAt = item.Item1.Split('@');
                if (afterAt.Length > 2)
                    continue;
                string[] tlds = afterAt[afterAt.Length - 1].Split('.');
                string tldKey = "";
                if (tlds.Length == 2)
                    tldKey = $".{tlds[1]}";
                if (tlds.Length == 3)
                    tldKey = $".{tlds[1]}.{tlds[2]}";

                if ((tlds.Length < 2)||(tlds.Length > 3))
                    continue;
                if ((tldKey == "") || String.IsNullOrWhiteSpace(item.Item2))
                    continue;

                if (_approxCountryBuckets.ContainsKey(tldKey))
                    _approxCountryBuckets[tldKey].Add(item);
                else
                {
                    _approxCountryBuckets.Add(tldKey, new List<Tuple<string, string>>());
                    _approxCountryBuckets[tldKey].Add(item);
                }

                RegionCountLab.Text = $"TLDs: {_approxCountryBuckets.Count}";
            }

            List<string> tldKeys = _approxCountryBuckets.Keys.ToList();
            tldKeys.Sort();

            var approxCountryBucketsSorted = new Dictionary<string, List<Tuple<string, string>>>();

            foreach (var item in tldKeys)
                approxCountryBucketsSorted.Add(item, _approxCountryBuckets[item]);

            RegionListBox.ItemsSource = approxCountryBucketsSorted;
        }

        private void RegionListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RegionListBox.SelectedValue != null)
                _regionList = _approxCountryBuckets[(string)RegionListBox.SelectedValue];
            CredsByRegionListBox.ItemsSource = _regionList;
        }

        private void searchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            Dictionary<string, List<Tuple<string, string>>> updatedDict = new Dictionary<string, List<Tuple<string, string>>>();
            List<string> keyList = new List<string>();
            foreach (var key in _approxCountryBuckets.Keys)
            {
                if (key.Contains(searchTextBox.Text.ToLower()))
                    keyList.Add(key);
            }
            keyList.Sort();
            foreach (var key in keyList)
            {
                updatedDict.Add(key, _approxCountryBuckets[key]);
            }
            RegionListBox.ItemsSource = updatedDict;
        }

        private void UsernameTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock source = (TextBlock)sender;
            Clipboard.SetText(source.Text);
        }

        private void PasswordTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock source = (TextBlock)sender;
            Clipboard.SetText(source.Text);
        }
        private void LoadCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                pathLabel.Content = openFileDialog.FileName;

            string[] temp = openFileDialog.FileName.Split('.');

            if (temp[temp.Length - 1] == "txt")
                using (StreamReader sr = new StreamReader(openFileDialog.FileName))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] unp = line.Split(':');
                        if (unp.Length == 2)
                        {
                            Tuple<string, string> tempTupe = new Tuple<string, string>(unp[0], unp[1]);
                            _fullList.Add(tempTupe);
                            CredentialCountLab.Text = $"Creds: {_fullList.Count}";
                        }
                    }
                }

            PartitionByLocation();
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void LoadCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


    }
}
