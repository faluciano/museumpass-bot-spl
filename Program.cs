using System.Threading.Tasks;
class Program
{
    static Timer? _timer;
    static async Task Main()
    {
        // Run a web server in a separate task

        // Run email service logic
        _timer = new Timer(async _ => await RunEmailService(), null, TimeSpan.Zero, TimeSpan.FromDays(1));

        // Wait for the web server to complete

    }

    static async Task RunEmailService()
    {
        DotNetEnv.Env.Load();
        var locations = await MuseumRequest.ParseMuseums("30");

        await EmailService.EmailService.SendEmail(locations);
    }

    static void Print_results(Dictionary<string, Tuple<string, List<string>>> values)
    {
        foreach (var value in values)
        {
            Console.WriteLine("--------------------");
            Console.WriteLine($"{value.Key}: ");
            Console.Write("URL: " + value.Value.Item1 + "\nValues: ");
            foreach (var item in value.Value.Item2)
            {
                Console.Write($"{item}, ");
            }
            Console.WriteLine();
        }
    }
}
