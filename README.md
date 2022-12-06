# Serilog.Sinks.Unity3D

[![openUPM](https://img.shields.io/npm/v/com.serilog.sinks.unity3d?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.serilog.sinks.unity3d/)

Serilog sink for Unity3D, logs to Unity Debugger

## Usage

### Installation
Install it through OpenUPM or use the UnityPackage from the [Releases](https://github.com/KuraiAndras/Serilog.Sinks.Unity3D/releases) page.

```bash
openupm add com.serilog.sinks.unity3d
```

### Dependencies

You need add Serilog to your project. Your usual options:
- [Xoofx's UnityNuGet server](https://github.com/xoofx/UnityNuGet) (preferred)
- Manual add the Serilog DLL to your assets folder
- [NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity)

### Creating the logger

```c#
var logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Unity3D()
    .CreateLogger();
```

If you have a custom implementation of Unity's `ILogger` interface, then you can log to that:

```c#
ILogger myCustomLogger = new MyCustomLogger();

var logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Unity3D(unityLogger: myCustomLogger)
    .CreateLogger();
```

If no logger is provided the library will use `UnityEngine.Debug.unityLogger` (which is equivalent of using `UnityEngine.Debug.Log()` methods)

## Migration guide

### For versions before 2.0.0

Set up [MainThreadDispatcher.Unity](https://github.com/KuraiAndras/MainThreadDispatcher.Unity)

### Upgrade from 1.0.0 to 1.0.1

You need to provide the following DLLs:

- Serilog
- MainThreadDispatcher
