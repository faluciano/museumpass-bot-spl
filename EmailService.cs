
using System.Text.Json;
using MailKit.Net.Smtp;
using MimeKit;

// Send an email to a list of addresses with the available museums

namespace EmailService
{
    public class EmailService
    {

        public static async Task SendEmail(Dictionary<string, Tuple<string, List<string>>> museums)
        {
            var botEmail = Environment.GetEnvironmentVariable("EMAIL_USERNAME");
            var botPassword = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Museum Availability Bot", botEmail));

            LoadEmailList(message);

            var date = DateTime.Now;
            message.Subject = $"Museum Availability for {date.Month}/{date.Day}/{date.Year}";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = "<h1>Museum Availability</h1><h2>Here are the museums available for the next 30 days:</h2>"
            };

            foreach (var museum in museums)
            {
                // Add day of week to date based on dateValue
                string dayOfWeek = GetDayOfWeek(museum.Key);
                string listTitle = $"{museum.Key} ({dayOfWeek})";

                bodyBuilder.HtmlBody += $"<h2 style='color: blue;'><a href='{museum.Value.Item1}'>{listTitle}</a></h2>";
                bodyBuilder.HtmlBody += "<ul>";
                foreach (var item in museum.Value.Item2)
                {
                    bodyBuilder.HtmlBody += $"<li>{item}</li>";
                }
                bodyBuilder.HtmlBody += "</ul>";
            }

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync(botEmail, botPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }

        }

        private static string GetDayOfWeek(string date)
        {
            return DateTime.Parse(date).DayOfWeek.ToString();
        }

        // Reads emails from a json file
        private static void LoadEmailList(MimeMessage message)
        {
            using StreamReader r = new StreamReader("emaillist.json");
            string json = r.ReadToEnd();
            if (json == null)
            {
                return;
            }
            List<Dictionary<string, string>>? array = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(json);
            if (array == null)
            {
                return;
            }
            foreach (var item in array)
            {
                message.To.Add(new MailboxAddress(item["name"], item["email"]));
            }


        }
    }
}