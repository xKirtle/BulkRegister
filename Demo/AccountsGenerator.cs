using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.IO;
using CefSharp;
using CefSharp.OffScreen;
using System.Threading;
using System.Text.RegularExpressions;

namespace AccountsGenerator
{
    class AccountsGenerator
    {
        static void Main(string[] args)
        {
            CefSettings settings = new CefSettings();
            Cef.Initialize(settings);

            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\TempValue.txt";
            string pathCombine = Path.Combine(path, "\\TempValue.txt");
            if (File.Exists(path))
            {
                string pathString = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\TempValue.txt";
                string numberXAccounts = File.ReadAllText(pathString);
                Int32.TryParse(numberXAccounts, out int numberAccounts);
                File.Delete(pathString);

                GenData(numberAccounts);
                System.Windows.Forms.MessageBox.Show("Generated " + numberAccounts + " accounts.", "Accounts Generator");
            }
            else
            {
                System.Console.WriteLine("Please don't open this application directly");
                System.Console.ReadKey();
            }
        }

        public static void GenData(int times)
        {
            for (int i = 0; i < times; i++)
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; }; //Simple "catch all" for the ssl/tls trust relationship errors

                using (WebClient client = new WebClient())
                {

                    var browser = new ChromiumWebBrowser("https://hybbe.top/registro");

                    //Parsing the data we need from the api
                    var json = client.DownloadString("https://api.namefake.com/portuguese-brazil/random/");
                    JObject o = JObject.Parse(json);
                    var nameb = (string)o["username"];
                    var email_u = (string)o["email_u"];
                    var email_d = (string)o["email_d"];
                    var password = (string)o["password"];
                    var email = email_u + "@" + email_d;

                    bool parsed;
                    if (nameb != null && password != null && email != null)
                    {
                        parsed = true;
                    }
                    else
                    {
                        parsed = false;
                    }

                    while (!parsed) //while loop to loop on until everything has been parsed from the API
                    {
                        if (nameb != null && password != null && email != null)
                        {
                            parsed = true;
                        }
                    }

                    string name = Regex.Replace(nameb, @"[^0-9a-zA-Z]+", ""); //Removing special characters from the password since they are not accepted

                    //Prints info on the console
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nName: " + name);
                    Console.WriteLine("Email: " + email);
                    Console.WriteLine("Password: " + password + "\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    
                     
                    bool loaded;
                    if (browser.IsLoading)
                    {
                        loaded = false;
                    }
                    else
                    {
                        loaded = true;
                    }

                    while (!loaded) //while loop to loop on until the page has finished loading
                    {
                        if (!browser.IsLoading)
                        {
                            loaded = true;
                            break;
                        }
                    }

                    //Actually registers
                    string scriptName = "document.getElementById('username').value = '" + name + "';";
                    browser.ExecuteScriptAsync(scriptName);
                    Thread.Sleep(50);

                    string scriptEmail = "document.getElementById('email-address').value = '" + email + "';";
                    browser.ExecuteScriptAsync(scriptEmail);
                    Thread.Sleep(50);

                    string scriptPassword = "document.getElementById('password_new').value = '" + password + "';";
                    browser.ExecuteScriptAsync(scriptPassword);
                    Thread.Sleep(50);

                    string scriptPassword2 = "document.getElementById('password_new_repeated').value = '" + password + "';";
                    browser.ExecuteScriptAsync(scriptPassword2);
                    Thread.Sleep(50);

                    string scriptGender = "document.getElementById('masculino').checked = true;";
                    browser.ExecuteScriptAsync(scriptGender);
                    Thread.Sleep(50);

                    string scriptRegister = "document.getElementById('botao_registrar_final').click();";
                    browser.ExecuteScriptAsync(scriptRegister);
                    Thread.Sleep(200);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n" + browser.Address + "\n");
                    Console.ForegroundColor = ConsoleColor.White;

                    browser.Load("https://hybbe.top/sair"); //Logs out of the account
                    Thread.Sleep(200);
                    //browser.Load("https://hybbe.top/registro");
                    
                     

                    //Saves the accounts info on a temp text file (plain text)
                    string pathString2 = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\TempAccounts.txt";
                    using (StreamWriter sw = File.AppendText(pathString2))
                    {
                        sw.WriteLine("Name: " + name);
                        sw.WriteLine("Email: " + email);
                        sw.WriteLine("Password: " + password + "\n");
                    }

                    //Path that will load the generated accounts into the listboxes. File will be deleted as soon as they accounts are in the listboxes
                    string pathString2_t = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\TempTempAccounts.txt";
                    using (StreamWriter sw = File.AppendText(pathString2_t))
                    {
                        sw.WriteLine("Name: " + name);
                        sw.WriteLine("Email: " + email);
                        sw.WriteLine("Password: " + password + "\n");
                    }
                    File.SetAttributes(pathString2_t, File.GetAttributes(pathString2_t) | FileAttributes.Hidden);
                }
            }
            //End of for loop
        }
    }
}
