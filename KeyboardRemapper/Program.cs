using System;
using System.Threading.Tasks;

namespace KeyboardRemapper
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var cli = new CommandLineInterface();
                await cli.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Environment.Exit(1);
            }
        }
    }
}
