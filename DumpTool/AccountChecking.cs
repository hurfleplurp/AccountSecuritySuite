using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Input;

namespace DumpTool
{
    public class AccountChecking
    {
        private string url = "https://twitter.com/login";
        private string username = "someUserName";
        private string password = "somePassword";
        private string commit = "Sign+In"; //this matches the data from Tamper Data

        public void ValidateCredentials(AccountType accountType, string user, string pass)
        {
            switch (accountType)
            {
                case AccountType.twitter:
                    url = "https://twitter.com/login";
                    commit = "Sign+In";
                    break;
                default:
                    break;
            }

            username = user;
            password = pass;

            Login();
        }

        private void Login()
        {
            WebBrowser b = new WebBrowser();
            b.Navigated += new NavigatedEventHandler(b_Navigated);
            //b.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(b_DocumentCompleted);
            b.Navigate(url);
        }

        private void b_Navigated(object sender, NavigationEventArgs e)
        {
            WebBrowser b = sender as WebBrowser;
            string response = b.Document.ToString();

            // looks in the page source to find the authenticity token.
            // could also use regular exp<b></b>ressions here.
            int index = response.IndexOf("authenticity_token");
            int startIndex = index + 41;
            string authenticityToken = response.Substring(startIndex, 40);

            // unregisters the first event handler
            // adds a second event handler
            b.Navigated -= new NavigatedEventHandler(b_Navigated);
            b.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(b_DocumentCompleted2);

            // format our data that we are going to post to the server
            // this will include our post parameters.  They do not need to be in a specific
            //	order, as long as they are concatenated together using an ampersand ( & )
            string postData = string.Format("authenticity_token={2}&session[username_or_email]={0}&session[password]={1}&commit={3}", username, password, authenticityToken, commit);

            ASCIIEncoding enc = new ASCIIEncoding();

            //  we are encoding the postData to a byte array
            b.Navigate("https://twitter.com/sessions", "", enc.GetBytes(postData), "Content-Type: application/x-www-form-urlencoded\r\n");
        }

        private void b_DocumentCompleted2(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser b = sender as WebBrowser;
            string response = b.DocumentText;

            if (response.Contains("Sign out"))
            {
                MessageBox.Show("Login Successful");
            }
        }


    }
}
