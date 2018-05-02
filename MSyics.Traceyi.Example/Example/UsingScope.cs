﻿using MSyics.Traceyi.Layout;
using MSyics.Traceyi.Listeners;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSyics.Traceyi.Example
{
    class UsingScope : IExample
    {
        public Tracer Tracer { get; set; }

        public void Setup()
        {
            Traceable.Add(@"config\settings.json");
            Tracer = Traceable.Get();
        }

        public void Shutdown()
        {
            Traceable.Shutdown();
        }

        public void Test()
        {
            using (Tracer.Scope())
            {
                using (Tracer.Scope())
                {
                    Tracer.Start();
                }
            }
        }
    }
}
