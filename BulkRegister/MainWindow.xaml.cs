using System.Windows;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Windows.Controls;
using System.Collections;
using System;
using System.Diagnostics;
using CefSharp;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using System.Threading.Tasks;
//using WindowsInput;

namespace BulkRegister
{
    public partial class MainWindow : Window
    {
        //InputSimulator inp = new InputSimulator();
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
                var nameb = (string)o["username"];
                var email_u = (string)o["email_u"];
                var email_d = (string)o["email_d"];
                var password = (string)o["password"];

                //Removing special characters from the password since they are not accepted
                string name = Regex.Replace(nameb, @"[^A-Za-z0-9]+", ""); 

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
            if (Name.Text != "" && Email.Text != "" && Password.Password != "" && Browser.IsLoaded == true) //Preventing empty fields & crashes
            {
                //Inputs the Name at the Register username field
                string scriptName = "document.getElementById('username').value = '" + Name.Text + "';";
                Browser.ExecuteScriptAsync(scriptName);
                //Inputs the Email at the Register email field
                string scriptEmail = "document.getElementById('email-address').value = '" + Email.Text + "';";
                Browser.ExecuteScriptAsync(scriptEmail);
                //Checks which password box is active and prints it on the register field
                if (CheckBoxPassword.IsChecked == true)
                {
                    string scriptPassword = "document.getElementById('password_new').value = '" + TextBoxPassword.Text + "';";
                    Browser.ExecuteScriptAsync(scriptPassword);
                    string scriptPassword2 = "document.getElementById('password_new_repeated').value = '" + TextBoxPassword.Text + "';";
                    Browser.ExecuteScriptAsync(scriptPassword2);
                }
                else
                {
                    string script_Password = "document.getElementById('password_new').value = '" + Password.Password + "';";
                    Browser.ExecuteScriptAsync(script_Password);
                    string script_Password2 = "document.getElementById('password_new_repeated').value = '" + Password.Password + "';";
                    Browser.ExecuteScriptAsync(script_Password2);
                }
                //Selects the male gender (Lazy, I know)
                string scriptGender = "document.getElementById('masculino').checked = true;";
                Browser.ExecuteScriptAsync(scriptGender);
                //Registers
                string scriptRegister = "document.getElementById('botao_registrar_final').click();";
                Browser.ExecuteScriptAsync(scriptRegister);

                if (CheckBoxAutoSave.IsChecked == true)
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

                        //Saves the accounts info on a temp text file (plain text)
                        string pathString2 = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\TempAccounts.txt";
                        using (StreamWriter sw = File.AppendText(pathString2))
                        {
                            sw.WriteLine("Name: " + Name.Text);
                            sw.WriteLine("Email: " + Email.Text);
                            sw.WriteLine("Password: " + Password.Password + "\n");
                        }
                    }
                }
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (Name.Text != "" && Email.Text != "" && Password.Password != "" && Browser.IsLoaded == true) //Preventing empty fields & crashes
            {
                //Inputs the name
                string scriptName = "document.getElementById('emailorusername').value = '" + Name.Text + "';";
                Browser.ExecuteScriptAsync(scriptName);
                //Inputs the password
                if (CheckBoxPassword.IsChecked == true)
                {
                    string scriptPassword = "document.getElementsByName('password')[0].value = '" + TextBoxPassword.Text + "';";
                    Browser.ExecuteScriptAsync(scriptPassword);
                }
                else
                {
                    string scriptPassword2 = "document.getElementsByName('password')[0].value = '" + Password.Password + "';";
                    Browser.ExecuteScriptAsync(scriptPassword2);
                }
                //Login
                string scriptLogin = "document.getElementById('botao_login').click();";
                Browser.ExecuteScriptAsync(scriptLogin);
            }
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

                    //Saves the accounts info on a temp text file (plain text)
                    string pathString2 = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\TempAccounts.txt";
                    using (StreamWriter sw = File.AppendText(pathString2))
                    {
                        sw.WriteLine("Name: " + Name.Text);
                        sw.WriteLine("Email: " + Email.Text);
                        sw.WriteLine("Password: " + Password.Password + "\n");
                    }
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
            if (listBoxName.SelectedIndex >= 0) //SelectedIndex needs to be >= 0 or else whenever we remove an item from the list it will throw an error
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
                if (CheckBoxPassword.IsChecked == true)
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

            else if (Browser.Address == "https://hybbe.top/principal" && Browser.IsLoaded == true) //Checks if the user is logged in before joining the client
            {
                string scriptJoinClient = "document.getElementsByName('login')[1].click();";
                Browser.ExecuteScriptAsync(scriptJoinClient); //Joins the 60fps Client
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

        private void CheckSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Detach_Click(object sender, RoutedEventArgs e)
        {
            Browser window = new Browser();
            window.Show();
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
            Int32.TryParse(numberAccounts.Text, out int numberaccts); //Checking if the value makes sense
            if (numberaccts > 0)
            {
                //Creates File with our x number of accounts to be created
                string pathString = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\TempValue.txt";
                File.WriteAllText(pathString, numberAccounts.Text);

                File.SetAttributes(pathString, File.GetAttributes(pathString) | FileAttributes.Hidden);

                Process.Start("C:\\Users\\Kirtle\\source\\repos\\BulkRegister\\Demo\\bin\\x86\\Debug\\AccountsGenerator.exe");


                string pathString2_t = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\TempTempAccounts.txt";

                bool loop = true;
                while (loop)
                {
                    //Almost a solution but I can't interact with the file because it's being interacted with on the other app
                    if (File.Exists(pathString2_t) && File.ReadAllLines(pathString2_t).Length >= 3 * numberaccts)
                    { 
                        loop = false;
                        break;
                    }
                }

                if (File.Exists(pathString2_t))
                {

                    foreach (string line in File.ReadLines(pathString2_t).AsParallel().WithDegreeOfParallelism(3))
                    {
                        //Don't need to check for duplicates as these were freshly randomized and the file will be deleted after reading these accounts. (The odds are super slim..)
                        //I can also save each element found in each line into a listbox because I know their indexes will match since the next two lines always contain the info from x Name
                        if (line.Contains("Name: "))
                        {
                            var nameline = line.Replace("Name: ", "");

                            ListBoxItem item = new ListBoxItem();
                            item.Content = nameline;
                            listBoxName.Items.Add(item);
                        }
                        if (line.Contains("Email: "))
                        {
                            var emailline = line.Replace("Email: ", "");

                            ListBoxItem item1 = new ListBoxItem();
                            item1.Content = emailline;
                            listBoxEmail.Items.Add(item1);
                        }
                        if (line.Contains("Password: "))
                        {
                            var passwordline = line.Replace("Password: ", "");

                            ListBoxItem item2 = new ListBoxItem();
                            item2.Content = passwordline;
                            listBoxPassword.Items.Add(item2);
                        }
                    }

                    File.Delete(pathString2_t);
                }
            }
        }
    }
}
