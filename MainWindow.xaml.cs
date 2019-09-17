using System.Windows;
using Newtonsoft.Json.Linq;
using System.Net;
using CefSharp;
using CefSharp.Wpf;
using WindowsInput;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Collections;
using System.Linq;

namespace BulkRegister
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        InputSimulator inp = new InputSimulator();

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
                Password.Text = password;
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            //string script = "document.getElementById('username')[0].value = '" + Name.Text + "';";
            //chrome.ExecuteScriptAsync(script);
            //chrome.GetMainFrame().ExecuteJavaScriptAsync(script);

            //Couldn't solve this with DOM properties, yet
            if (Name.Text != "" && Email.Text != "" && Password.Text != "") //Preventing exceptions that would crash the program
            {
                if (listBox.Items.Count >= 1)
                {
                    for (int i = 0; i < 13; i++)
                    {
                        inp.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.TAB); //Leaves the input at the username
                    }
                }
                else
                {
                    for (int i = 0; i < 12; i++)
                    {
                        inp.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.TAB); //Leaves the input at the username
                    }
                }

                //Ciruclates through all of the other fields
                inp.Keyboard.TextEntry(Name.Text);
                inp.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.TAB);
                inp.Keyboard.TextEntry(Email.Text);
                inp.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.TAB);
                inp.Keyboard.TextEntry(Password.Text);
                inp.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.TAB);
                inp.Keyboard.TextEntry(Password.Text);
                inp.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.TAB);
                inp.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.SPACE);
                inp.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.TAB);
                inp.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.SPACE); //Registers 
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e) //Saving all the accounts details that were generated
        {
            //Create a new item (object) and add it to our list

            ComboBoxItem itm = new ComboBoxItem();
            itm.Content = Password.Text;
            comboBox.Items.Add(itm);

            ListBoxItem itm2 = new ListBoxItem();
            itm2.Content = Name.Text;
            listBox.Items.Add(itm2);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //We get the listbox index and set the email to blank
            int Index = listBox.SelectedIndex;
            Email.Text = "";
            //Fill the Name textbox with the listbox Name that was selected
            string name = listBox.SelectedItem.ToString();
            string nam = name.Replace("System.Windows.Controls.ListBoxItem: ", "");
            Name.Text = nam;
            //And get the password from our combobox to the Password textbox
            string password = comboBox.Items[Index].ToString();
            string passwor = password.Replace("System.Windows.Controls.ComboBoxItem: ", "");
            Password.Text = passwor;
        }

        private void Login_Click(object sender, RoutedEventArgs e) //chrome.Address is returning an empty string. This whole method is not working as intended
        {
            //if (chrome.Address == "https://hybbe.top/registro")
                //return;

            //else if (chrome.Address == "https://hybbe.top/principal") //Checks if the user is logged in before joining the client
            //{
                for (int i = 0; i < 11; i++)
                {
                    inp.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.TAB); //Leaves the input at Client Button
                }
                inp.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.SPACE); //Joins the Client
            //}
        }
    }
}
