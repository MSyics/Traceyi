
using System.Threading.Tasks;

namespace MSyics.Traceyi
{
    class Program : ExampleAggregator
    {
        static Task Main(string[] args)
        {
            return new Program().
                //Add<SetupByManual>().
                //Add<SetupByJsonFile>().
                //Add<SetupByConfiguration>().
                //Add<UsingTraceMethod>().
                //Add<UsingCustomTraceListener>().
                //Add<UsingShiftrJIS>().
                //Add<UsingScope>().
                //Add<UsingAsync>().
                Add<UsingArchive>().
                //Add<UsingExtensions>().

                ShowAsync();
        }
    }
}
