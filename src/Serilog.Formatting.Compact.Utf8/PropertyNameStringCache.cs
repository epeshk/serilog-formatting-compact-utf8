using System.Collections;
using System.Text;
using Serilog.Utf8.Commons;

namespace Serilog.Formatting.Compact.Utf8;

static class PropertyNameStringCache
{
  const int MaxCacheItems = 5000;

  static readonly Hashtable templates = new(ByRefEqComparer.Instance);
  static readonly object sync = new();
  public static byte[] Get(string s)
  {
    var result = (byte[]?)templates[s];
    if (result is not null)
      return result;
    var key = s;

    if (s.StartsWith('@'))
      s = ",@" + s + ":";
    else
      s = "," + s + ":";
    result = Encoding.UTF8.GetBytes(JsonEscaper.Escape(s));

    lock (sync)
    {
      if (templates.Count == MaxCacheItems)
        templates.Clear();

      templates[key] = result;
    }

    return result;
  }
}