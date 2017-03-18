using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


using System.Collections;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;


using System.Text;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;

using Newtonsoft.Json;
using CalendarBot;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CalendarBot
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    
    public sealed partial class MainPage : Page
    {
        //public static user = "";
        public static bool accessPage = false;
        public static string log = "";
        public static string passy = "";
        //new Windows.Security.Credentials.PasswordVault();
        //public sealed class PasswordVault;
        public MainPage()
        {
            this.InitializeComponent();
        }

        //public class UserInfo
        //{
        //    public string info { get; set; }
        //    //public string more { get; set; }
        //}

        private void logout_Click(object sender, RoutedEventArgs e)
        {
            //var Info = new UserInfo();
            //Info.info = "";
            MyFrame.Navigate(typeof(loginPage));
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Debug.WriteLine("Navigated");
            //title.Text = passy.ToString();
            if (accessPage != true)
            {
                MyFrame.Navigate(typeof(loginPage));
            }

            if (log != "")
            {
                displayAnswers();
            }
            

        }

        private async void displayAnswers()
        {
            //var firebase = new FirebaseClient("https://calendarbot-2573c.firebaseio.com/");
            //var answer = await firebase
            //      .Child("users")
            //      .Child(log)
            //      .OnceAsync<Answers>();


            //foreach (var a in answer)
            //{
            //    Answers answers = new Answers();
            //    answers.answers = a.Object.answers;
            //    if(a.Key == "answers")
            //    {
            //        ansAndQues.Children.Add(new TextBox() { Text = answers.answers.ToString() });
            //    }
                
            //}
        }

        private async void send_Click(object sender, RoutedEventArgs e)
        {

            string parameter = string.Empty;
            //NavigationContext.QueryString.TryGetValue("parameter", out parameter);
            
            //u = parameter;
            
            var firebase = new Firebase.Database.FirebaseClient("https://calendarbot-2573c.firebaseio.com/");
            //var More = new UserInfo();
            //More.info = 
            // add new item to list of data and let the client generate new key for you (done offline)
            var ques = await firebase
                .Child("users")
                .Child(log)
                .Child("questions")
                .Child(question.Text)
                .PostAsync(question.Text);

            string que = question.Text;

            ansAndQues.Children.Add(new TextBlock() { Text = question.Text });


            await AiResponse.GetResponse(question.Text);

            answer.Text = AiResponse.answer;

            //var ans = await firebase
            //    .Child("users")
            //    .Child(log)
            //    .Child("answers")
            //    .Child(answer.Text)
            //    .PostAsync(answer.Text);

            ansAndQues.Children.Add(new TextBlock() { Text = answer.Text });


            startTime.Text = AiResponse.startTime;
            endTime.Text = AiResponse.endTime;
            title.Text = AiResponse.title;
            date.Text = AiResponse.date;

            appoint.Text = AiResponse.appoint;
            question.Text = "";

        }

    }
    internal class Answers
    {
        public string answers { get; set; }
    }
}
