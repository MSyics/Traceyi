﻿using System.Threading.Tasks;

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
                //Add<UsingScope>().
                //Add<UsingExtensions>().
                //Add<UsingLayout>().
                //Add<UsingCustomTraceListener>().
                //Add<UsingShiftJIS>().
                //Add<UsingAsync>().
                //Add<UsingArchive>().

                Add<UsingILogger>().

                ShowAsync();
        }
    }
}
