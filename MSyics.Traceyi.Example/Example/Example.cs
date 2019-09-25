using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSyics.Traceyi
{
    abstract class Example : IExample
    {
        public Tracer Tracer { get; set; }

        public abstract string Name { get; }

        public abstract void Setup();

        void IExample.Show() { }

        public abstract Task ShowAsync();

        public abstract void Teardown();
    }
}
