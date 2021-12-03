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
        if (Members.Count is 0) return string.Empty;

        StringBuilder sb = new();
        Format(Members.First());
        foreach (var item in Members.Skip(1))
        {
            sb.Append('\t');
            Format(item);
        }
        return sb.ToString();

        void Format(KeyValuePair<string, object> item)
        {
            switch (item.Value)
            {
                case TimeSpan:
                    sb.AppendFormat("{0:d\\.hh\\:mm\\:ss\\.fffffff}", item.Value);
                    break;
                case DateTimeOffset:
                    sb.AppendFormat("{0:yyyy-MM-ddTHH:mm:ss.fffffffzzz}", item.Value);
                    break;
                default:
                    sb.AppendFormat("{0}", item.Value);
                    break;
            }
        }
    }
}
