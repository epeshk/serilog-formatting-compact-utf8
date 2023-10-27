using System.Buffers;
using System.Text;
using Serilog.Events;
using Serilog.Utf8.Commons;

namespace Serilog.Formatting.Compact.Utf8;

/// <summary>
/// An <see cref="IBufferWriterFormatter"/> that writes events in a compact JSON format.
/// </summary>
public class CompactUtf8JsonFormatter : IBufferWriterFormatter
{
  static readonly byte[][] logLevels = {
    ",\"@l\":\"Verbose\""u8.ToArray(),
    ",\"@l\":\"Debug\""u8.ToArray(),
    ",\"@l\":\"Information\""u8.ToArray(),
    ",\"@l\":\"Warning\""u8.ToArray(),
    ",\"@l\":\"Error\""u8.ToArray(),
    ",\"@l\":\"Fatal\""u8.ToArray()
  };

  readonly Utf8JsonValueFormatter valueFormatter = new();
  public void Format(LogEvent logEvent, IBufferWriter<byte> bufferWriter)
  {
    var writer = new Utf8Writer(bufferWriter);
    writer.Write("{\"@t\":\""u8);
    writer.Format(logEvent.Timestamp.UtcDateTime);
    writer.Write("\",\"@mt\":"u8);
    var message = Utf8MessageTemplateCache.Get(logEvent.MessageTemplate);
    writer.Write(message.JsonEscaped);
    var isFirstTokenWithFormat = true;
    var delim = (byte)0;
    foreach (var token in message.Tokens)
    {
      if (token is not Utf8PropertyToken propertyToken)
        continue;
      if (propertyToken.Format is null)
        continue;

      if (isFirstTokenWithFormat)
      {
        writer.Write(",\"@r\":["u8);
        isFirstTokenWithFormat = false;
      }
      
      if (delim != 0)
        writer.Write(delim);
      delim = (byte)',';

      if (!logEvent.Properties.TryGetValue(propertyToken.PropertyName, out var propertyValue))
      {
        writer.Write(propertyToken.JsonEscapedQuotedName);
        continue;
      }

      valueFormatter.TryFormat(propertyValue, ref writer);
    }
    if (!isFirstTokenWithFormat)
      writer.Write((byte)']');

    var level = (int)logEvent.Level;
    if (level != (int)LogEventLevel.Information)
    {
      if (level is >= 0 and < 6)
        writer.Write(logLevels[level]);
      else
        WriteLevelNever(logEvent, ref writer);
    }
    
    if (logEvent.Exception != null)
    {
      writer.Write(",\"@x\":"u8);
      writer.WriteChars(JsonEscaper.Escape(logEvent.Exception.ToString()));
    }

    // TODO: wait for Serilog release
    // if (logEvent.TraceId != null)
    // {
    //   writer.Write(",\"@tr\":\""u8);
    //   writer.WriteChars(logEvent.TraceId.Value.ToHexString());
    //   writer.Write((byte)'\"');
    // }
    //
    // if (logEvent.SpanId != null)
    // {
    //   writer.Write(",\"@sp\":\""u8);
    //   writer.WriteChars(logEvent.SpanId.Value.ToHexString());
    //   writer.Write((byte)'\"');
    // }

    foreach (var property in logEvent.Properties)
    {
      var name = PropertyNameStringCache.Get(property.Key);
      
      writer.Write(name);
      valueFormatter.TryFormat(property.Value, ref writer);
    }

    writer.Write((byte)'}');
    writer.WriteNewLine();
    writer.Flush();
  }

  public Encoding Encoding => Encoding.UTF8;

  static void WriteLevelNever(LogEvent logEvent, ref Utf8Writer writer)
  {
    writer.WriteChars(logEvent.Level.ToString());
  }
}