using CefSharp;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace BulkRegister
{
    public partial class MainWindow : Window
    {
        List<Account> accounts = new List<Account>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Loads settings
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Saves settings
        }

        private void LoadList_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveList_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; }; //Simple "catch all" for the ssl/tls trust relationship errors

            using (WebClient client = new WebClient())
            {
                //Parsing the data we need from the api
                var json = client.DownloadString("https://api.namefake.com/portuguese-brazil/random/");
                JObject o = JObject.Parse(json);
                var nameb = (string)o["username"];
                var email_u = (string)o["email_u"];
                var email_d = (string)o["email_d"];
                var email = email_u + "@" + email_d;
                var password = (string)o["password"];
                var bdate = (string)o["birth_data"];

                //Creating a list with our DoB
                string[] dates = bdate.Split('-');
                List<string> birthDate = new List<string>(dates.Length);
                birthDate.AddRange(dates);

                //YY-MM-DD
                DateTime date = new DateTime(Int32.Parse(birthDate[0]), Int32.Parse(birthDate[1]), Int32.Parse(birthDate[2]));

                //MessageBox.Show(birthDate[0].ToString() + "/" + birthDate[1].ToString() + "/" + birthDate[2].ToString());

                //Removing special characters from the password since they are not accepted
                string name = Regex.Replace(nameb, @"[^A-Za-z0-9]+", "");

                //Adding it to our text boxes
                Name.Text = name;
                Email.Text = email;
                Gender.IsChecked = true; //M
                DatePicker.SelectedDate = date;

                if (CheckBoxPassword.IsChecked == true)
                    TextBoxPassword.Text = password;
                else
                    Password.Password = password;

                //var newUser = Account.CreateAccount(name, password, email, true, date);
                //accounts.Add(newUser);
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var newUser = Account.CreateAccount(Name.Text, Password.Password, Email.Text, true, DatePicker.DisplayDate);
            accounts.Add(newUser);
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Password_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBoxPassword.IsChecked == true)
            {
                Password.Visibility = Visibility.Collapsed;
                TextBoxPassword.Visibility = Visibility.Visible;
                TextBoxPassword.Text = Password.Password;
            }
            else
            {
                Password.Visibility = Visibility.Visible;
                TextBoxPassword.Visibility = Visibility.Collapsed;
                Password.Password = TextBoxPassword.Text;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e) //Saving all the accounts details that were generated
        {
            if (!String.IsNullOrWhiteSpace(Name.Text) && !accounts.Exists(x => x.Username == Name.Text))
            {
                var newUser = Account.CreateAccount(Name.Text, Password.Password, Email.Text, true, DatePicker.DisplayDate);
                accounts.Add(newUser);

                ListBoxItem newItem = new ListBoxItem { Content = newUser };
                listBox.Items.Add(newItem);
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedIndex >= 0)
            {
                accounts.RemoveAt(listBox.SelectedIndex);
                listBox.Items.RemoveAt(listBox.SelectedIndex);
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox.SelectedIndex >= 0)
            {
                var account = accounts[listBox.SelectedIndex];

                Name.Text = account.Username;
                Email.Text = account.Email;
                Gender.IsChecked = true;
                DatePicker.SelectedDate = account.DoB;

                if (CheckBoxPassword.IsChecked == true)
                    TextBoxPassword.Text = account.Password;
                else
                    Password.Password = account.Password;
            }
        }

        private void Client_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.Items.Count > 0)
            {
                DialogResult result = System.Windows.Forms.MessageBox.Show("Are you sure you want to Delete everything?", "Confirmation", MessageBoxButtons.YesNo);
                if (result.ToString() == "Yes")
                {
                    accounts.Clear();
                    listBox.Items.Clear();
                }
            }
        }

        //Mute App Volume with win api
        [DllImport("winmm.dll")]
        private static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

        public static void SetVolume(int volume)
        {
            int NewVolume = ((ushort.MaxValue / 10) * volume);
            uint NewVolumeAllChannels = (((uint)NewVolume & 0x0000ffff) | ((uint)NewVolume << 16));
            waveOutSetVolume(IntPtr.Zero, NewVolumeAllChannels);
        }
        bool mute = false;
        private void Mute_Click(object sender, RoutedEventArgs e)
        {
            if (mute == false)
            {
                SetVolume(0);
                mute = true;
                //Adds visual confirmation that the audio is muted
                this.Title += " (Audio Muted)";
            }
            else if (mute == true)
            {
                SetVolume(5);
                mute = false;
                this.Title = this.Title.Replace(" (Audio Muted)", "");
            }
        }

        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            //Gets the info to start the new process of the running app
            AppDomainSetup ads = AppDomain.CurrentDomain.SetupInformation;
            var path = ads.ApplicationBase + "\\" + ads.ApplicationName;
            //Opens a new process and kills the running (old) one
            Process.Start(new ProcessStartInfo(path));
            Process.GetCurrentProcess().Kill();
        }

        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            //Filters out anything that is not defined in the regex
            string onlyNumeric = @"^([0-9]+(.[0-9]+)?)$";
            Regex regex = new Regex(onlyNumeric);
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void CreateXAccounts_Click(object sender, RoutedEventArgs e)
        {

        }

        //private void Window_Activated(object sender, EventArgs e)
        //{

        //}
    }
}
