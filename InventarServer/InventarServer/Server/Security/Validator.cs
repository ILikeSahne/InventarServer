using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace InventarServer
{
    class Validator
    {
        public Validator()
        { }

        public bool ValidateUsername(string _username)
        {
            Regex r = new Regex("^(?=.{2,20}$)(?![_.])(?!.*[_.]{2})[a-zA-Z0-9._]+(?<![_.])$");
            return r.IsMatch(_username);
        }

        public bool ValidateEmail(string _email)
        {
            try
            {
                string address = new MailAddress(_email).Address;
            }
            catch (FormatException)
            {
                return false;
            }
            return true;
        }

        public bool ValidatePassword(string _password)
        {
            var input = _password;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMiniMaxChars = new Regex(@".{8,15}");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            if (!hasLowerChar.IsMatch(input))
                return false;
            else if (!hasUpperChar.IsMatch(input))
                return false;
            else if (!hasMiniMaxChars.IsMatch(input))
                return false;
            else if (!hasNumber.IsMatch(input))
                return false;
            else if (!hasSymbols.IsMatch(input))
                return false;

            return true;
        }
    }
}
