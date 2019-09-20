
using System.Threading.Tasks;

namespace MSyics.Traceyi.Example
{
    class Program : ExampleAggregator
    {
        static void Main(string[] args)
        {
            new Program().
                //Add<SetupByManual>().
                //Add<SetupByJsonFile>().
                //Add<SetupByConfiguration>().
                //Add<UsingTraceMethod>().
                //Add<UsingCustomTraceListener>().
                //Add<UsingShiftrJIS>().
                //Add<UsingScope>().
                Add<UsingAsync>().

                Show();
        }
    }
}
