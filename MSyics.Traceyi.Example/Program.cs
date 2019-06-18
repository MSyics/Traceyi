
using System.Threading.Tasks;

namespace MSyics.Traceyi.Example
{
    class Program : ExampleAggregator
    {
        static void Main(string[] args)
        {
            new Program()
                //.Add<SetupByManual>()
                //.Add<SetupByConfiguration>()
                //.Add<SetupByJsonFile>()
                //.Add<UsingCustomTraceListener>()
                //.Add<UsingTraceMethod>()
                //.Add<UsingShiftrJIS>()
                .Add<UsingScope>()

                .Show();
        }
    }
}
