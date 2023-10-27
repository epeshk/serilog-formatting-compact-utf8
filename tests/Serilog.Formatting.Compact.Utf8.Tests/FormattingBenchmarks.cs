// using BenchmarkDotNet.Attributes;
// using Serilog.Events;
// using Serilog.Formatting.Compact.Tests.Support;
// using Serilog.Formatting.Compact.Utf8;
// using Serilog.Formatting.Json;
//
// namespace Serilog.Formatting.Compact.Tests
// {
//     [Config(typeof(FormattingBenchmarksConfig))]
//     public class FormattingBenchmarks
//     {
//         readonly LogEvent _evt;
//
//         readonly ITextFormatter _jsonFormatter = new JsonFormatter(),
//             _renderedJsonFormatter = new JsonFormatter(renderMessage: true);
//         readonly IUtf8TextFormatter
//             _compactFormatter = new CompactUtf8JsonFormatter(),
//             _renderedCompactFormatter = new RenderedCompactUtf8JsonFormatter();
//
//         public FormattingBenchmarks()
//         {
//             var collectorSink = new CollectorSink();
//
//             new LoggerConfiguration()
//                 .WriteTo.Sink(collectorSink)
//                 .CreateLogger()
//                 .Information("Hello, {@User}, {N:x8} at {Now}", new { Name = "nblumhardt", Tags = new[] { 1, 2, 3 } }, 123, DateTime.Now);
//
//             _evt = collectorSink.LastCollected;
//         }
//
//         StringWriter _buffer;
//         byte[] _utf8;
//
//         [GlobalSetup]
//         public void InitBuffer()
//         {
//             _buffer = new StringWriter();
//             _utf8 = new byte[64 * 1024];
//         }
//
//         [Benchmark(Baseline = true)]
//         public void JsonFormatter()
//         {
//             _buffer.GetStringBuilder().Clear();
//             _jsonFormatter.Format(_evt, _buffer);
//         }
//
//         [Benchmark]
//         public void CompactJsonFormatter()
//         {
//             _compactFormatter.Format(_evt, _utf8);
//         }
//
//         [Benchmark]
//         public void RenderedJsonFormatter()
//         {
//             _buffer.GetStringBuilder().Clear();
//             _renderedJsonFormatter.Format(_evt, _buffer);
//         }
//
//         [Benchmark]
//         public void RenderedCompactJsonFormatter()
//         {
//             _renderedCompactFormatter.Format(_evt, _utf8);
//         }
//     }
// }