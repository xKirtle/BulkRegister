using System.Windows;
using Newtonsoft.Json.Linq;
using System.Net;
using WindowsInput;
using System.Windows.Controls;
using System.Collections;
using System;
using System.Diagnostics;

namespace BulkRegister
{
    public partial class MainWindow : Window
    {
        InputSimulator inp = new InputSimulator();
        ArrayList NameList = new ArrayList();
        ArrayList PasswordList = new ArrayList();
        ArrayList EmailList = new ArrayList();

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
            foreach (string item in Settings.Default.list_Name)
            {
                listBoxName.Items.Add(item.ToString());
            }

            foreach (string item1 in Settings.Default.list_Password)
            {
                listBoxPassword.Items.Add(item1.ToString());
            }

            foreach (string item2 in Settings.Default.list_Email)
            {
                listBoxEmail.Items.Add(item2.ToString());
            }
        }

        private void SaveList_Click(object sender, RoutedEventArgs e)
        {
            foreach (object item in listBoxName.Items)
            {
                string itm = item.ToString().Replace("System.Windows.Controls.ListBoxItem: ", "");
                NameList.Add(itm);
            }

            foreach (object item1 in listBoxPassword.Items)
            {
                string itm1 = item1.ToString().Replace("System.Windows.Controls.ListBoxItem: ", "");
                PasswordList.Add(itm1);
            }

            foreach (object item2 in listBoxEmail.Items)
            {
                string itm2 = item2.ToString().Replace("System.Windows.Controls.ListBoxItem: ", "");
                EmailList.Add(itm2);
            }
            Settings.Default.list_Name = NameList;
            Settings.Default.list_Password = PasswordList;
            Settings.Default.list_Email = EmailList;
            Settings.Default.Save();
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; }; //Simple "catch all" for the ssl/tls trust relationship errors

            using (WebClient client = new WebClient())
            {
                //Parsing the data we need from the api
                var json = client.DownloadString("https://api.namefake.com/portuguese-brazil/random/");
                JObject o = JObject.Parse(json);
                var name = (string)o["username"];
                var email_u = (string)o["email_u"];
                var email_d = (string)o["email_d"];
                var password = (string)o["password"];
                //Adding it to our text boxes
                Name.Text = name;
                Email.Text = email_u + "@" + email_d;

                if (CheckBoxPassword.IsChecked == true)
                {
                    TextBoxPassword.Text = password;
                }
                else
                {
                    Password.Password = password;
                }
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            //string script = "document.getElementById('username')[0].value = '" + Name.Text + "';";
            //chrome.ExecuteScriptAsync(script);
            //chrome.GetMainFrame().ExecuteJavaScriptAsync(script);

            //Couldn't solve this with DOM properties, yet
            if (Name.Text != "" && Email.Text != "" && Password.Password != "") //Preventing exceptions that would crash the program
            {
                if (listBoxName.Items.Count >= 1)
                {
                    for (int i = 0; i < 18; i++)
                    {
                        inp.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.TAB); //Leaves the input at the username
                    }
                }
                else
                {
                    for (int i = 0; i < 17; i++)
                    {
                        inp.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.TAB); //Leaves the input at the username
                    }
                }

                //Ciruclates through all of the other fields
                inp.Keyboard.TextEntry(Name.Text);
                inp.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.TAB);
                inp.Keyboard.TextEntry(Email.Text);
                inp.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.TAB);
                if (CheckBoxPassword.IsChecked == true)
                {
                    inp.Keyboard.TextEntry(TextBoxPassword.Text);
                }
                else
                {
                    inp.Keyboard.TextEntry(Password.Password);
                }
                inp.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.TAB);
                if (CheckBoxPassword.IsChecked == true)
                {
                    inp.Keyboard.TextEntry(TextBoxPassword.Text);
                }
                else
                {
                    inp.Keyboard.TextEntry(Password.Password);
                }
                inp.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.TAB);
                inp.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.SPACE);
                inp.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.TAB);
                inp.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.SPACE); //Registers 
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 14; i++)
            {
                inp.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.TAB); //Leaves the input at the username
            }
            inp.Keyboard.TextEntry(Name.Text);
            inp.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.TAB);
            if (CheckBoxPassword.IsChecked == true)
            {
                inp.Keyboard.TextEntry(TextBoxPassword.Text);
            }
            else
            {
                inp.Keyboard.TextEntry(Password.Password);
            }
            inp.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.TAB);
            inp.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.SPACE); //Logins
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
            //The only thing that matters to prevent duplicates is the username, as the password/email can be repeated
            if (Name.Text.Length > 0)
            {
                bool found = false;
                foreach (var item in listBoxName.Items)
                {
                    string item1 = item.ToString().Replace("System.Windows.Controls.ListBoxItem: ", ""); //Getting the item as a single string without its item type
                    if (item1.Equals(Name.Text))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found) //If it doesn't find any matches on listBoxName
                {
                    //Create a new item (object) and add it to our list
                    ListBoxItem item = new ListBoxItem();
                    item.Content = Name.Text;
                    listBoxName.Items.Add(item);

                    ListBoxItem item1 = new ListBoxItem();
                    item1.Content = Password.Password;
                    listBoxPassword.Items.Add(item1);

                    ListBoxItem item2 = new ListBoxItem();
                    item2.Content = Email.Text;
                    listBoxEmail.Items.Add(item2);
                }
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            //Removes the corresponding data from the selected index from each list
            int Index = listBoxName.SelectedIndex;
            if (Index >= 0)
            {
                listBoxName.Items.RemoveAt(Index);
                listBoxPassword.Items.RemoveAt(Index);
                listBoxEmail.Items.RemoveAt(Index);
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //We get the listbox index
            int Index = listBoxName.SelectedIndex;
            if (listBoxName.SelectedIndex >= 0)
            {
                //Fill the Name textbox with the listbox Name that was selected
                string name = listBoxName.SelectedItem.ToString();
                string nam = name.Replace("System.Windows.Controls.ListBoxItem: ", "");
                Name.Text = nam;

                //the Email
                string email = listBoxEmail.Items[Index].ToString();
                string emai = email.Replace("System.Windows.Controls.ListBoxItem: ", "");
                Email.Text = emai;

                //And, finally, the password
                string password = listBoxPassword.Items[Index].ToString();
                string passwor = password.Replace("System.Windows.Controls.ListBoxItem: ", "");
                if (CheckBoxPassword.IsChecked == true) //SelectedIndex needs to be >= 0 or else whenever we remove an item from the list it will throw an error
                {
                    TextBoxPassword.Text = passwor;   
                }
                else if (CheckBoxPassword.IsChecked == false)
                {
                    Password.Password = passwor;
                }
            }
        }

        private void Client_Click(object sender, RoutedEventArgs e)
        {
            if (Browser.Address == "https://hybbe.top/registro")
                return;

            else if (Browser.Address == "https://hybbe.top/principal") //Checks if the user is logged in before joining the client
            {
                for (int i = 0; i < 18; i++)
                {
                    inp.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.TAB); //Leaves the input at Client Button
                }
                inp.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.SPACE); //Joins the Client
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            listBoxName.Items.Clear();
        }

        private void Mute_Click(object sender, RoutedEventArgs e)
        {
            //Game has a built in radio system. I'll try to mute my application sound with this button
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

        private void Detach_Click(object sender, RoutedEventArgs e)
        {
            Browser window = new Browser();
            window.Show();
        }
    }
}
