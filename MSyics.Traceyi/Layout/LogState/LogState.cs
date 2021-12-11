using System.Text;
using System.Text.Json.Serialization;

namespace MSyics.Traceyi.Layout;

/// <summary>
/// 記録データを表します。
/// </summary>
public class LogState
{
    /// <summary>
    /// 記録データのメンバー一覧を取得または設定します。
    /// </summary>
    [JsonExtensionData]
    public IDictionary<string, object> Members { get; set; }

    public override string ToString()
    {
        if (Members.Count is 0) return "";

        StringBuilder sb = new();
        foreach (var item in Members)
        {
            Format(item);
        }
        return sb.ToString();

        void Format(KeyValuePair<string, object> item)
        {
            switch (item.Value)
            {
                case TimeSpan:
                    sb.AppendFormat("[{0},{1:d\\.hh\\:mm\\:ss\\.fffffff}]", item.Key, item.Value);
                    break;
                case DateTimeOffset:
                    sb.AppendFormat("[{0},{1:yyyy-MM-ddTHH:mm:ss.fffffffzzz}]", item.Key, item.Value);
                    break;
                default:
                    sb.AppendFormat("[{0},{1}]", item.Key, item.Value);
                    break;
            }
        }
    }
}
