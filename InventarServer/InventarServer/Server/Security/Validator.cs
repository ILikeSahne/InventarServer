using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace InventarServer
{
    /// <summary>
    /// Validates usernames, emails and passwords
    /// </summary>
    class Validator
    {
        /// <summary>
        /// Validates a Password
        /// Requirements:
        ///     2-20 chars
        ///     Only upper + lower chars, numbers and _
        ///     Isn't allowed to start with _
        /// </summary>
        /// <param name="_username">The Username to validate</param>
        /// <returns>True if the Username is valid</returns>
        public static bool ValidateUsername(string _username)
        {
            Regex r = new Regex("^(?=.{2,20}$)(?![_.])(?!.*[_.]{2})[a-zA-Z0-9._]+(?<![_.])$");
            return r.IsMatch(_username);
        }

        /// <summary>
        /// Validates an Email
        /// </summary>
        /// <param name="_email">The Email to validate</param>
        /// <returns>True if the Email is valid</returns>
        public static bool ValidateEmail(string _email)
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

        /// <summary>
        /// Validates a Password
        /// Requirements:
        ///     Has Numbers
        ///     Has Lower Chars
        ///     Has Upper Chars
        ///     Has at least 6 Chars and at most 20 chars
        ///     Has at least on of those Symbols: !@#$%^&*()_+=\[{\]};:<>|./?,-
        /// </summary>
        /// <param name="_password">The Password to validate</param>
        /// <returns>True if the Password is valid</returns>
        public static bool ValidatePassword(string _password)
        {
            var input = _password;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMiniMaxChars = new Regex(@".{6,20}");
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

    enum ValidateError
    {
        NONE, EMAIL_WRONG, USERNAME_WRONG, PASSWORD_WRONG, EMAIL_USED, USERNAME_USED
    }
}
