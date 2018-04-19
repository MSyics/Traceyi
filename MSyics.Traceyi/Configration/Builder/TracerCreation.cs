/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using MSyics.Traceyi.Listeners;
using System;

namespace MSyics.Traceyi.Configration
{
    internal sealed class TracerCreation : IBuildTracerSettings, IBuildTraceListener, IBuildTracer
    {
        private Tracer Product { get; } = new Tracer();

        public IBuildTraceListener Settings(Action<TracerSettings> settings)
        {
            if (settings == null) return this;

            settings(Product.Settings);
            return this;
        }

        public IBuildTracer Attach(params ITraceListener[] listeners)
        {
            foreach (var item in listeners)
            {
                Product.Tracing += item.OnTracing;
            }
            return this;
        }

        public IBuildTracer Attach(params Action<TraceEventArg>[] listeners)
        {
            foreach (var item in listeners)
            {
                Product.Tracing += (sender, e) => item(e);
            }
            return this;
        }

        public Tracer Get() => Product;
    }

    public interface IBuildTracerSettings
    {
        IBuildTraceListener Settings(Action<TracerSettings> settings);
    }

    public interface IBuildTraceListener
    {
        IBuildTracer Attach(params ITraceListener[] listeners);
        IBuildTracer Attach(params Action<TraceEventArg>[] listeners);
    }

    public interface IBuildTracer
    {
        Tracer Get();
    }
}
