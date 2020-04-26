# Serilog.Sinks.Unity3D

[![openupm](https://img.shields.io/npm/v/com.serilog.sinks.unity3d?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.serilog.sinks.unity3d/)

Serilog sink for Unity3D, logs to Unity Debugger

# Usage
Set up [MainThreadDispatcher.Unity](https://github.com/KuraiAndras/MainThreadDispatcher.Unity)

Since version 1.0.1 you need to provide the following dlls:

- Serilog
- MainThreadDispatcher

Then use your sink:
```c#
var logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Unity3D()
    .CreateLogger();
```