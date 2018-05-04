
namespace MSyics.Traceyi.Example
{
    class Program : Examplar
    {
        static void Main(string[] args)
        {
            new Program()
                .Add<SetupByManual>()
                .Add<SetupByConfiguration>()
                .Add<SetupByJsonFile>()
                .Add<UsingCustomTraceListener>()
                .Add<UsingTraceMethod>()
                .Add<UsingShiftrJIS>()
                .Add<UsingScope>()

                .Add<_>()
                
                .Test();
        }
    }
}
