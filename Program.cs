class Program
{
    static async Task Main()
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
