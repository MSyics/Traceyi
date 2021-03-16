using MSyics.Traceyi.Layout;
using MSyics.Traceyi.Listeners;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSyics.Traceyi
{
    class Hoge
    {
        public int Value { get; set; }
    }

    class UsingTest : Example
    {
        public override string Name => nameof(UsingTest);

        public override void Setup()
        {
            Traceable.Add(@"Example\UsingTest\traceyi.json");
            //Traceable.Add(listeners: async e =>
            //{
            //    //Console.WriteLine($"{e.Action}\t{e.Message}\t{e.args}");

            //    //if (string.IsNullOrWhiteSpace(e.message)) return;

            //    //foreach (var item in Convert(e.message))
            //    //{
            //    //    Console.WriteLine($"-{item}-");
            //    //}

            //    //if(e.args.Length > 0)
            //    //{
            //    //    var parts = Convert(e.message, e.args.Length).Select(x => new LogLayoutPart
            //    //    {
            //    //        Name = x,
            //    //        CanFormat = true
            //    //    }).ToArray();

            //    //    var format = new LogLayoutConverter(parts).Convert(e.message);
            //    //    var formattedMessage = string.Format(new LogLayoutFormatProvider(), format, e.args);
            //    //    Console.WriteLine(formattedMessage);
            //    //    if (e.args.Length > 0)
            //    //    {
            //    //        Dictionary<string, object> items = new Dictionary<string, object>();
            //    //        for (int i = 0; i < Math.Max(parts.Length, e.args.Length); i++)
            //    //        {
            //    //            if (parts.Length < i + 1)
            //    //            {
            //    //                items.Add($"Unkown{i}", e.args[i]);
            //    //            }
            //    //            else
            //    //            {
            //    //                var key = parts[i].Name.Trim();
            //    //                var k = 0;
            //    //                while (true)
            //    //                {
            //    //                    if (items.ContainsKey(key))
            //    //                    {
            //    //                        key = $"{key}-{k++}";
            //    //                    }
            //    //                    else
            //    //                    {
            //    //                        items.Add(key, e.args[i]);
            //    //                        break;
            //    //                    }
            //    //                }
            //    //            }
            //    //        }
            //    //        var json = System.Text.Json.JsonSerializer.Serialize(items);
            //    //        Console.WriteLine(json);
            //    //    }
            //    //}

            //    //if (e.action1 != null)
            //    //{
            //    //    Tracer.Piyo hoge = new Tracer.Piyo();
            //    //    e.action1(hoge);

            //    //    foreach (var item in hoge.Items)
            //    //    {
            //    //        Console.WriteLine($"{item.Key} {item.Value}");
            //    //    }
            //    //}

            //    if (e.payload != null)
            //    {
            //        //dynamic x = new ExpandoObject();
            //        //e.payload?.Invoke(x);
            //        //IDictionary<string, object> a = x;

            //        Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(e.Payload));

            //        var parts = e.Payload.Select(x => new LogLayoutPart
            //        {
            //            Name = x.Key,
            //            CanFormat = true
            //        }).ToArray();
            //        var format = new LogLayoutConverter(parts).Convert(e.message);
            //        Console.WriteLine(format);
            //        var formattedMessage = string.Format(new LogLayoutFormatProvider(), format, e.Payload.Values.ToArray());
            //        Console.WriteLine(formattedMessage);
            //        Console.WriteLine($"{e.Traced:yyyy-MM-dd HH:mm:ss.fffffffzzz}");
            //    }
            //});

            Tracer = Traceable.Get();
        }

        public override void Teardown()
        {
            Traceable.Shutdown();
        }

        public override async Task ShowAsync()
        {
            var ex = new ApplicationException("hgoehgoe", new NullReferenceException("piyopiyo"));

            //object i = 1;
            //using (Tracer.Scope())
            //{
            //    //Tracer.Trace("hoge:{{hoge1 , 8:D8|_, 12:R}} {value:000000}", 16, 1, 2, "3");
            //    //Tracer.Trace("hoge:{{hoge|_,20:R}}, value:{value}, piyo:{hoge,4:D4|_,8:L} {hoge}hoge", "hoge1094", 1, 2, 3);

            //    Tracer.Information("case1 a:{hoge}  b:{piyo|_,16:R}  c:{hogehoge|_,20:R}", x =>
            //    {
            //        x.hoge = new Hoge { Value = 999 };
            //        x.piyo = "3";
            //        x.fuga = 4;
            //    });
            //    Tracer.Information(x =>
            //    {
            //        x.case2 = 10;
            //    });
            //    Tracer.Information($"case3 {10}");
            //    Tracer.Information("case4 test", x => { x.hoge = 1000; });
            //    Tracer.Information(ex);
            //    //Tracer.Information("case6 ex:{ex}", x => { x.ex = ex; });
            //    Tracer.Information("{hoge} {ex}", x =>
            //    {
            //        x.hoge = "test";
            //        x.ex = ex;
            //    });
            //}

            //await Task.Delay(1);

            //using (Tracer.Scope("hogehoge", message: "start"))
            //{
            //    Tracer.Information($"case3 {10}");
            //    Tracer.Information($"case3 {10}");
            //    Tracer.Information($"case3 {10}");
            //    Tracer.Information($"case3 {10}");
            //    Tracer.Information($"case3 {10}");
            //    Tracer.Information($"case3 {10}");
            //    Tracer.Information(x =>
            //    {
            //        x.hoge = new Hoge { Value = 999 };
            //        x.piyo = "3";
            //        x.fuga = 4;
            //        x.a = 4;
            //        x.b = 4;
            //    });
            //}

            //var hoge = new Hoge { Value = 1 };
            //Tracer.Context.ActivityId = "activity";
            //using (Tracer.Scope("operationId", "test {hoge}", x => x.hoge = hoge))
            //{
            //    //hoge.Value += 1;
            //    //Tracer.Information(x => x.hoge = hoge.Value);
            //    //hoge.Value += 1;
            //    //Tracer.Information(x => x.hoge = hoge.Value);
            //    //hoge.Value += 1;
            //    //Tracer.Information(x => x.hoge = hoge.Value);
            //    //hoge.Value += 1;
            //    Tracer.Information(x => x.hoge = hoge);
            //    //hoge.Value += 1;
            //    Tracer.Information(x => x.hoge = hoge);
            //    //hoge.Value += 1;
            //    Tracer.Information(x => x.piyo = hoge);
            //}
            //Tracer.Information(x => x = hoge);

            //Tracer.Stop("stop");
            //using (Tracer.Scope(1))
            //{
            //    using (Tracer.Scope(2))
            //    {
            //        Tracer.Stop();
            //        Tracer.Stop();

            //        Tracer.Start(3);
            //        Tracer.Information(x => x.piyo = hoge);

            //        Tracer.Start(4);
            //        Tracer.Information(x => x.piyo = hoge);

            //        Tracer.Information(x => x.piyo = hoge);
            //    }
            //}
            //i = null;

            using var ts = Tracer.Scope();

            Tracer.Information("hogehoge");
            Tracer.Information("{hoge}", x => x.hoge = "hogehoge");

            Console.WriteLine("{0                                        :000}", 1);
            Tracer.Information(
                "{hoge:000|_,12:L}, {piyo=>json}, {hoge :000}",
                x =>
                {
                    x.hoge = 1;
                    x.piyo = new Hoge { Value = 100 };
                });

            await Task.CompletedTask;
        }

        private void Case001()
        {
            Tracer.Stop("stop");
            Tracer.Start("start");
            using (var scope = Tracer.Scope(1, operationId: "scope"))
            {
                Tracer.Information("hogehoge");

                Tracer.Stop("stop");

                Tracer.Start("002");
                Tracer.Information("piyopiyo");
                Tracer.Stop("002");
                Tracer.Stop("002-2");

                Tracer.Start("003");
                Tracer.Start("004");
                Tracer.Stop("004");

                scope.Stop("stop scope");
            }

            using (Tracer.Scope("001"))
            {
                using (Tracer.Scope("002"))
                {

                }
            }
            Tracer.Information("piyopiyo");
            Tracer.Stop("stop");
            Tracer.Information("piyopiyo");

            Tracer.Start(operationId: "hoge");
        }

        public List<string> Convert(string layout, int count)
        {
            var names = new List<string>();
            var span = layout.AsSpan();
            for (int layoutIndex = 0; layoutIndex < span.Length; layoutIndex++)
            {
                if (span[layoutIndex] == '{')
                {
                    var startIndex = layoutIndex + 1;
                    var length = span[(startIndex + 1)..].IndexOf('}') + 1;

                    if (length > 0)
                    {
                        var convertString = span.Slice(startIndex, length);
                        if (GetName(convertString, out var name))
                        {
                            names.Add(name);
                            if (names.Count == count) break;
                            layoutIndex = startIndex + length;
                            continue;
                        }
                    }
                }
            }
            return names;
        }

        private static bool GetName(ReadOnlySpan<char> format, out string name)
        {
            int i = 0;
            foreach (var c in format)
            {
                if (c == '{' || c == '}' || c == ':' || c == ',' || c == '|') break;
                ++i;
            }

            if (i == 0)
            {
                name = string.Empty;
                return false;
            }

            name = format[0..i].ToString();
            return true;
        }
    }
}
