using System;

namespace BulkRegister
{
    public class Account
    {
        private static Account _instance;
        public static Account Instance { get { return _instance; } }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool Gender { get; set; }
        public DateTime DoB { get; set; }

        public override string ToString()
        {
            return Username;
        }

        private Account(string username, string password, string email, bool gender, DateTime dob)
        {
            Username = username;
            Password = password;
            Email = email;
            Gender = gender;
            DoB = dob;
        }

        public static Account CreateAccount(string username, string password, string email, bool gender, DateTime dob)
        {
            var newUser = new Account(username, password, email, gender, dob);
            return newUser;
        }

        public static Account GetInstance(string username, string password, string email, bool gender, DateTime dob)
        {
            if (_instance == null)
            {
                _instance = new Account(username, password, email, gender, dob);
            }

            return _instance;
        }
    }
}
