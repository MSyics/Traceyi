
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
                .Add<UsingTraceMethod>()
                .Add<UsingShiftrJIS>()
                .Add<UsingCustomTraceListener>()
                .Add<UsingScope>()

                //.AddExample<_>()

                .Test();
        }
    }
}
