using System.Windows;
using Newtonsoft.Json.Linq;
using System.Net;

namespace BulkRegister
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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

        }
    }
}
