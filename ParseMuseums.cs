using HtmlAgilityPack;

public class MuseumRequest
{
    private static readonly HttpClient client = new HttpClient();

    public static async Task<Dictionary<string, Tuple<string, List<string>>>> ParseMuseums(string numDays = "7")
    {

        //Get current date in PST
        var date = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
        string dateString = $"{date.Month}/{date.Day}/{date.Year}";

        // Number of days

        var payload = new Dictionary<string, string>
        {
            { "curOrg", "SEATTLE" },
            { "curKey1", "ALL" },
            { "curNumDays", numDays },
            { "curKey2", "AVA" },
            { "curPassStartDate", dateString }
        };


        string url = "https://www.eventkeeper.com/mars/tkflex.cfm";



        // Make the POST request
        var content = new FormUrlEncodedContent(payload);
        var responseMessage = await client.PostAsync(url, content);
        responseMessage.EnsureSuccessStatusCode();

        var response = await responseMessage.Content.ReadAsStringAsync();


        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(response);

        // Define a list to hold the input values
        var AvailableMuseumMap = new Dictionary<string, Tuple<string, List<string>>>();

        // Select all input elements of type button within the pr_container class
        var container = doc.DocumentNode.SelectNodes("//div[contains(@class, 'pr_container_left')]");
        string dateValue = "DNF";
        if (container != null)
        {
            for (int i = 1; i < container.Count; i++)
            {

                var dateNode = container[i].SelectSingleNode(".//div[1]/a");

                if (dateNode != null)
                {
                    dateValue = dateNode.GetAttributeValue("name", string.Empty);
                }

                if (!AvailableMuseumMap.ContainsKey(dateValue))
                {

                    AvailableMuseumMap[dateValue] = new Tuple<string, List<string>>(string.Empty, new List<string>());
                }

                // Form url string with curPassStartDate set to the dateValue and curNumDays set to 1 as a string
                var payload2 = new Dictionary<string, string>
                {
                    { "curOrg", "SEATTLE" },
                    { "curKey1", "ALL" },
                    { "curNumDays", "1" },
                    { "curKey2", "AVA" },
                    { "curPassStartDate", dateValue }
                };

                AvailableMuseumMap[dateValue] = new Tuple<string, List<string>>($"{url}?{string.Join("&", payload2.Select(x => $"{x.Key}={x.Value}"))}", AvailableMuseumMap[dateValue].Item2);


                var inputs = container[i].SelectNodes(".//input[@type='button']");
                if (inputs != null)
                {
                    foreach (var input in inputs)
                    {
                        var value = input.GetAttributeValue("value", string.Empty);

                        if (!string.IsNullOrEmpty(value))
                        {
                            AvailableMuseumMap[dateValue].Item2.Add(value);
                        }
                    }
                }

            }

        }
        return AvailableMuseumMap;
    }
}

