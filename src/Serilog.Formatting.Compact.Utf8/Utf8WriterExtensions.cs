using System.Buffers.Text;
using Serilog.Utf8.Commons;

namespace Serilog.Formatting.Compact.Utf8;

static class Utf8WriterExtensions
{
  
  public static void Format(this ref Utf8Writer writer, DateTimeOffset timestamp)
  {
    writer.Reserve(64);
    Utf8Formatter.TryFormat(timestamp, writer.Span, out int bw, 'O');
    writer.Advance(bw);
  }
}