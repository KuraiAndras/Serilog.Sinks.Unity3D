using Serilog;
using Serilog.Sinks.Unity3D;
using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class SimpleLogger : MonoBehaviour
{
    [SerializeField] private Button _infoButton;
    [SerializeField] private Button _warningButton;
    [SerializeField] private Button _errorButton;
    [SerializeField] private Button _threadButton;

    private Serilog.ILogger _logger;

    private void Awake() =>
        _logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Unity3D()
            .CreateLogger();

    private void Start()
    {
        _infoButton.onClick.AddListener(() => _logger.Information("This is an info"));
        _warningButton.onClick.AddListener(() => _logger.Warning("This is a warning"));
        _errorButton.onClick.AddListener(() =>
        {
            try
            {
                throw new InvalidOperationException("Invalid stuff");
            }
            catch (Exception e)
            {
                _logger.Error(e, "This is an error");
            }
        });
        _threadButton.onClick.AddListener(() =>
        {
            var stopWatch = Stopwatch.StartNew();

            ThreadPool.QueueUserWorkItem(state =>
            {
                stopWatch.Stop();
                _logger.Information("Log from thread {Id}, Invoke took: {Elapsed}", Thread.CurrentThread.ManagedThreadId, stopWatch.Elapsed);
            });
        });
    }
}