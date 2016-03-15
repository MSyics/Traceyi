using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSyics.Traceyi;

namespace Example.Traceyi
{
    class Program
    {
        static void Main(string[] args)
        {
            var exp = new Example();
            exp.Fire();
        }
    }

    class Example
    {
        private Tracer Tracer = Traceable.Get();

        public void Fire()
        {
            using (this.Tracer.Scope())
            {
                this.Tracer.Information("hogehoge");
            }
        }
    }
}
