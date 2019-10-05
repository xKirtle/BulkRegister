using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.IO;

namespace AccountsGenerator
{
    class AccountsGenerator
    {
        static void Main(string[] args)
        {
            //Parses the data from our temp file, converts it into a number and deletes the temp file.
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
                Console.WriteLine("Please don't open this application directly");
                Console.ReadKey();
            }
        }

        public static void GenData(int times)
        {
            for (int i = 0; i < times; i++)
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
                    var email = email_u + "@" + email_d;

                    //Prints info on the console
                    Console.WriteLine("Name: " + name);
                    Console.WriteLine("Email: " + email);
                    Console.WriteLine("Password: " + password + "\n");

                    //Saves the accounts info on a temp text file (plain text)
                    string pathString2 = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\TempAccounts.txt";
                    using (StreamWriter sw = File.AppendText(pathString2))
                    {
                        sw.WriteLine("Name: " + name);
                        sw.WriteLine("Email: " + email);
                        sw.WriteLine("Password: " + password + "\n");
                    }
                }
            }
        }
    }
}
