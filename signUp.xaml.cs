using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Windows.Input;
using Windows.UI.Popups;
using Firebase.Database;
using Firebase.Database.Query;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CalendarBot
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class signUp : Page
    {
        public signUp()
        {
            this.InitializeComponent();
            int num = 0;
        }

        

        private async void Submit_Click(object sender, RoutedEventArgs e)

        {
            //    //UserName Validation   
            if (!Regex.IsMatch(TxtUserName.Text.Trim(), @"^[A-Za-z_][a-zA-Z0-9_\s]*$"))
            {
                //MessageBox.Show("Invalid UserName");
                var dialog = new MessageDialog("Invalid UserName");
                await dialog.ShowAsync();
            }
            //Password length Validation   
            else if (TxtPwd.Password.Length < 6)
            {
                //MessageBox.Show("Password length should be minimum of 6 characters!");
                var dialog = new MessageDialog("Password length should be minimum of 6 characters!");
                await dialog.ShowAsync();
            }
            //    //EmailID validation   
            else if (!Regex.IsMatch(TxtEmail.Text.Trim(), @"^([a-zA-Z_])([a-zA-Z0-9_\-\.]*)@(\[((25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\.){3}|((([a-zA-Z0-9\-]+)\.)+))([a-zA-Z]{2,}|(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\])$"))
            {
                //MessageBox.Show("Invalid EmailId");
                var dialog = new MessageDialog("Invalid Email");
                await dialog.ShowAsync();
            }
            //    //After validation success ,store user detials and also check information  
            else if (TxtUserName.Text != "" && TxtPwd.Password != "" && TxtEmail.Text != "")
            {
                var firebase = new FirebaseClient("https://calendarbot-2573c.firebaseio.com/");
                var check = await firebase
                  .Child("users")
                  .OnceAsync<Users>();

                int num = 0;
                foreach (var c in check)
                {
                    if (c.Key == TxtUserName.Text)
                    {
                        var dialog = new MessageDialog("UserName is already taken");
                        await dialog.ShowAsync();
                    }
                    else if (num == 0)
                    {
                        var username = await firebase
                        .Child("users")
                        .Child(TxtUserName.Text)
                        .Child("email")
                        .PostAsync(TxtEmail.Text);

                        var password = await firebase
                        .Child("users")
                        .Child(TxtUserName.Text)
                        .Child("password")
                        .Child(TxtPwd.Password)
                        .PostAsync(TxtPwd.Password);

                        var dialog = new MessageDialog("You have successfully signed up");
                        await dialog.ShowAsync();

                        var vault = new Windows.Security.Credentials.PasswordVault();
                        vault.Add(new Windows.Security.Credentials.PasswordCredential(
                            "CalendarBot", TxtUserName.Text, TxtPwd.Password));

                        SignUpFrame.Navigate(typeof(loginPage));
                        num++;
                    }
                    
                }

            }
        }


    }

    internal class Users
    {
        public string username { get; set; }
    }
}
