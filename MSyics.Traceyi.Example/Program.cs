
using System.Threading.Tasks;

namespace MSyics.Traceyi.Example
{
    class Program : ExampleAggregator
    {
        static async Task Main(string[] args)
        {
            new Program()
                //.Add<SetupByManual>()
                //.Add<SetupByConfiguration>()
                //.Add<SetupByJsonFile>()
                //.Add<UsingCustomTraceListener>()
                //.Add<UsingTraceMethod>()
                //.Add<UsingShiftrJIS>()
                //.Add<UsingScope>()
                //.Add<UsingScope2>()
                .Add<Class1>()


                .Show();

            await Task.CompletedTask;
        }
    }
}
