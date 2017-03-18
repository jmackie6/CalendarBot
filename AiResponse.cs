using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.ApplicationModel.Appointments;

namespace CalendarBot
{

    public class AiResponse
    {
        private static string URL = "https://api.api.ai/v1/query";
        private static string urlParameters = "";
        private static string language = "&lang=en";
        private static string queryString = "&query=";
        //private string developerAccessToken = "204591ee637d4460a5179c06fc038c5a";
        private static int v = 20170304;
        private static string sessionId = "&sessionId=f982db7e-97c3-4cd8-91ec-11c121fe90d6";
        public static string timestamp = "";

        public static string answer = "";
        public static string error = "";

        public static string startTime = "";
        public static string endTime = "";
        public static string title = "";
        public static string date = "";
        public static string appoint = "";
        //public static string param = "&parameters=startTime";


        private static string clientAccessToken = "efeec4bb5e9f43fa836537751ca674d5";

        public static async Task<String> GetResponse(string question)
        {
            //AIService aiService;
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(URL);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", clientAccessToken);
            string query = question;

            urlParameters += "?v=" + v + queryString + query + language + sessionId;
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;

            string responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                dynamic json = JObject.Parse(responseBody);
                answer = json.result.fulfillment.speech;

                if ((string)json["result"]["action"] == "ScheduleEvent")
                {
                    if ((string)json["result"]["parameters"]["startTime"] != null)
                        startTime = (string)json["result"]["parameters"]["startTime"];

                    if ((string)json["result"]["parameters"]["endTime"] != null)
                        endTime = (string)json["result"]["parameters"]["endTime"];

                    if ((string)json["result"]["parameters"]["title"] != null)
                        title = (string)json["result"]["parameters"]["title"];

                    if ((string)json["result"]["parameters"]["date"] != null)
                        date = (string)json["result"]["parameters"]["date"];
                }

                if ((string)json["result"]["action"] == "getCalendarEvents")
                {

                    if ((string)json["result"]["parameters"]["date"] != null)
                        date = (string)json["result"]["parameters"]["date"];
                }

                if ((bool)json["result"]["actionIncomplete"] == false && (string)json["result"]["action"] == "ScheduleEvent")
                {
                    // Create an Appointment that should be added the user' s appointments provider app.
                    var appointment = new Windows.ApplicationModel.Appointments.Appointment();

                    string dateTime = AiResponse.date + " " + AiResponse.startTime;
                    DateTime myDate = DateTime.ParseExact(dateTime, "yyyy-MM-dd HH:mm:ss",
                                               System.Globalization.CultureInfo.InvariantCulture) + TimeSpan.FromDays(1);
                    appointment.StartTime = myDate;

                    if (AiResponse.endTime != "")
                    {
                        DateTime dt = DateTime.Parse(endTime);
                        endTime = dt.ToString("HH:mm");
                        string start = startTime.Substring(0, 2);
                        string end = endTime.Substring(0, 2);
                        int startT = Convert.ToInt32(start);
                        int endT = Convert.ToInt32(end);
                        int duration = endT - startT;
                        appointment.Duration = TimeSpan.FromHours(duration);
                    }
                    else
                    {
                        appointment.Duration = TimeSpan.FromHours(1);
                    }

                    appointment.Subject = AiResponse.title;
                    appointment.Reminder = TimeSpan.FromMinutes(20);

                    String appointmentId = await AppointmentManager.ShowEditNewAppointmentAsync(appointment);

                    //if (appointmentId != String.Empty)
                    //{
                    //    appoint = "Appointment Id: " + appointmentId;
                    //}
                    //else
                    //{
                    //    appoint = "Appointment not added.";
                    //}
                }

                if ((bool)json["result"]["actionIncomplete"] == false && (string)json["result"]["action"] == "getCalendarEvents")
                {
                    AppointmentStore appointmentStore = await AppointmentManager.RequestStoreAsync(AppointmentStoreAccessType.AllCalendarsReadOnly);

                    //var dateToShow = DateTime.Now.AddDays(0);
                    string dateT = AiResponse.date;
                    DateTime myDate = DateTime.ParseExact(dateT, "yyyy-MM-dd",
                                               System.Globalization.CultureInfo.InvariantCulture) + TimeSpan.FromDays(0);
                    var duration = TimeSpan.FromHours(24);

                    var appCalendars = await appointmentStore.FindAppointmentsAsync(myDate, duration);
                }

            }

            urlParameters = "";
            return answer;

        }

    }
}