using System.Windows;
using Newtonsoft.Json.Linq;
using System.Net;
using CefSharp;
using CefSharp.Wpf;
using WindowsInput;
using System.Collections.Generic;

namespace BulkRegister
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public class Accounts
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }

    public partial class MainWindow : Window
    {
        InputSimulator inp = new InputSimulator();
        ChromiumWebBrowser chrome = new ChromiumWebBrowser();

        List<Accounts> Account = new List<Accounts>() { };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //comboBox.ItemsSource = Account;
            //listBox.ItemsSource = Account;
            foreach (Accounts c in Account)
            {
                comboBox.Items.Add(Account);
                listBox.Items.Add(Account);
            }
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

            Account.Add(new Accounts() { Name = Name.Text, Password = Password.Text });

            for (int i = 0; i < 11; i++)
            {
                inp.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.TAB); //Leaves the input at the username
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

        private void Save_Click(object sender, RoutedEventArgs e) //Saving all the accounts details that were generated
        {
            Account.Add(new Accounts() { Name = Name.Text, Password = Password.Text }); //Not quite working

            //Can't read the values on the list since they show blank, yet a new index is generated
            comboBox.Items.Add(Name.Text);
            listBox.Items.Add(Password.Text);
        }
    }
}
