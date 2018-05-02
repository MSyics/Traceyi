using System;
using System.Collections.Generic;
using System.Text;

namespace MSyics.Traceyi.Example
{
    interface IExample
    {
        Tracer Tracer { get; set; }
        void Setup();
        void Test();
        void Shutdown();
    }
}
