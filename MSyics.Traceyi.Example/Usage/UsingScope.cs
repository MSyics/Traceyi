﻿using MSyics.Traceyi.Layout;
using MSyics.Traceyi.Listeners;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSyics.Traceyi.Example
{
    class UsingScope : ITrecyiExample
    {
        public Tracer Tracer { get; set; }

        public void Setup()
        {
            Traceable.Add("Traceyi.json");
            Tracer = Traceable.Get();
        }

        public void Test()
        {
            using (Tracer.Scope())
            {
                using (Tracer.Scope())
                {
                    Tracer.Information("hogehoge");
                    Tracer.Start();
                    Tracer.Information("hogehoge");
                }
            }
        }
    }
}
