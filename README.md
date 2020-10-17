# Serilog.Sinks.Unity3D

[![openupm](https://img.shields.io/npm/v/com.serilog.sinks.unity3d?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.serilog.sinks.unity3d/)

Serilog sink for Unity3D, logs to Unity Debugger

# Usage

Install it through OpenUPM or use the Unitypackage from the Releases.

```bash
openupm add com.serilog.sinks.unity3d
```

Place Serilog.dll to your assets folder, then use the library:

```c#
var logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Unity3D()
    .CreateLogger();
```

## For versions before 2.0.0

Set up [MainThreadDispatcher.Unity](https://github.com/KuraiAndras/MainThreadDispatcher.Unity)

## Upgrade from 1.0.0 to 1.0.1

You need to provide the following dlls:

- Serilog
- MainThreadDispatcher
