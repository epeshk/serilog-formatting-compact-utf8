using System.Buffers;
using System.IO;
using System.Text;
using Serilog.Core;
using Serilog.Events;
using Serilog.Utf8.Commons;

namespace Serilog.Formatting.Compact.Tests.Support
{
    public class TextWriterSink : ILogEventSink
    {
        readonly StringWriter _output;
        readonly IBufferWriterFormatter _formatter;
        readonly ArrayBufferWriter<byte> buffer = new ArrayBufferWriter<byte>();

        public TextWriterSink(StringWriter output, IBufferWriterFormatter formatter)
        {
            _output = output;
            _formatter = formatter;
        }

        public void Emit(LogEvent logEvent)
        {
            buffer.ResetWrittenCount();
            _formatter.Format(logEvent, buffer);
            _output.Write(Encoding.UTF8.GetString(buffer.WrittenSpan));
        }
    }
}