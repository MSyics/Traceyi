using System;
using System.Collections.Generic;
using System.Text;

namespace MSyics.Traceyi.Example
{
    interface ITrecyiExample
    {
        Tracer Tracer { get; set; }
        void Setup();
        void Test();
    }
}
