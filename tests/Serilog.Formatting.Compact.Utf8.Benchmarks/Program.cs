using System.Buffers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Compact.Utf8;
using Serilog.Formatting.Json;

BenchmarkRunner.Run<FormattingBenchmarks>();

[MemoryDiagnoser]
public class FormattingBenchmarks
{
  readonly LogEvent _evt;

  readonly ITextFormatter
    _jsonFormatter = new JsonFormatter(),
    _renderedJsonFormatter = new JsonFormatter(renderMessage: true),
    _originalCompactFormatter = new CompactJsonFormatter(),
    _originalRenderedJsonFormatter = new RenderedCompactJsonFormatter();
  readonly IBufferWriterFormatter
    _compactFormatter = new CompactUtf8JsonFormatter(),
    _renderedCompactFormatter = new RenderedCompactUtf8JsonFormatter();

  readonly ArrayBufferWriter<byte> abw = new ArrayBufferWriter<byte>();

  public FormattingBenchmarks()
  {
    var collectorSink = new CollectorSink();

    new LoggerConfiguration()
      .WriteTo.Sink(collectorSink)
      .CreateLogger()
      .Information("Hello, {@User}, {N:x8} at {Now}", new { Name = "nblumhardt", Tags = new[] { 1, 2, 3 } }, 123, DateTime.Now);

    _evt = collectorSink.LastCollected;
  }

  StringWriter _buffer;

  [GlobalSetup]
  public void InitBuffer()
  {
    _buffer = new StringWriter();
  }

  [Benchmark(Baseline = true)]
  public void JsonFormatter()
  {
    _buffer.GetStringBuilder().Clear();
    _jsonFormatter.Format(_evt, _buffer);
  }

  [Benchmark]
  public void OriginalCompactFormatter()
  {
    _buffer.GetStringBuilder().Clear();
    _originalCompactFormatter.Format(_evt, _buffer);
  }

  [Benchmark]
  public void CompactJsonFormatter()
  {
    abw.ResetWrittenCount();
    _compactFormatter.Format(_evt, abw);
  }

  [Benchmark]
  public void RenderedJsonFormatter()
  {
    _buffer.GetStringBuilder().Clear();
    _renderedJsonFormatter.Format(_evt, _buffer);
  }
  [Benchmark]
  public void OriginalRenderedCompactJsonFormatter()
  {
    _buffer.GetStringBuilder().Clear();
    _originalRenderedJsonFormatter.Format(_evt, _buffer);
  }
  
  [Benchmark]
  public void RenderedCompactJsonFormatter()
  {
    abw.ResetWrittenCount();
    _renderedCompactFormatter.Format(_evt, abw);
  }
}

public class CollectorSink : ILogEventSink
{
  public LogEvent LastCollected { get; private set; }

  public void Emit(LogEvent logEvent)
  {
    LastCollected = logEvent;
  }
}
