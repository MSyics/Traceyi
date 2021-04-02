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
                //Add<UsingShiftJIS>().
                //Add<UsingScope>().

                //Add<UsingArchive>().
                //Add<UsingExtensions>().
                //Add<UsingTest>().

                Add<UsingILogger>().

                Add<UsingAsync>().
                Add<UsingAsync>().
                Add<UsingAsync>().

                ShowAsync();
        }
    }
}
