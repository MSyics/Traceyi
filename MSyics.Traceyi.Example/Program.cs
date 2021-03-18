using System.Threading.Tasks;

namespace MSyics.Traceyi
{
    class Program : ExampleAggregator
    {
        static async Task Main(string[] args)
        {
            await new Program().
                //Add<SetupByManual>().
                //Add<SetupByJsonFile>().
                //Add<SetupByConfiguration>().
                //Add<UsingTraceMethod>().
                //Add<UsingCustomTraceListener>().
                //Add<UsingShiftrJIS>().
                //Add<UsingScope>().
                Add<UsingAsync>().
                //Add<UsingArchive>().
                //Add<UsingExtensions>().
                //Add<UsingTest>().
                Add<UsingILogger>().

                ShowAsync();
        }
    }
}
