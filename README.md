# Serilog.Sinks.Unity3D

Serilog sink for Unity3D, logs to Unity Debugger

# Usage
Set up [MainThreadDispatcher.Unity](https://github.com/KuraiAndras/MainThreadDispatcher.Unity)

Then use your sink:
```c#
var logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Unity3D()
    .CreateLogger();
```