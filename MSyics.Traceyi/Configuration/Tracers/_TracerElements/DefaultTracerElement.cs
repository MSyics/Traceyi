using System;

namespace MSyics.Traceyi.Configuration
{
    internal sealed class DefaultTracerElement : TracerElementBase
    {
        public override string Name
        {
            get { return "Default"; }
            protected set { throw new NotImplementedException(); }
        }
    }
}
