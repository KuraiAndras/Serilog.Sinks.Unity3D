#nullable enable
using System;
using System.Diagnostics;
using System.Threading;
using Serilog;
using Serilog.Sinks.Unity3D;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable RedundantDefaultMemberInitializer

namespace Sample.Scripts
{
    public class SimpleLogger : MonoBehaviour
    {
        [SerializeField] private Button _infoButton = default!;
        [SerializeField] private Button _warningButton = default!;
        [SerializeField] private Button _errorButton = default!;
        [SerializeField] private Button _threadButton = default!;

        private Serilog.ILogger _logger = default!;

        private void Awake() =>
            _logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Unity3D()
                .CreateLogger();

        private void Start()
        {
            _infoButton.onClick.AddListener(() => _logger.ForContext(this).WithUnityTag("Info tag").Information("This is an info"));
            _warningButton.onClick.AddListener(() => _logger.ForContext(this).WithUnityTag("Warning tag").Warning("This is a warning"));
            _errorButton.onClick.AddListener(() =>
            {
                try
                {
                    throw new InvalidOperationException("Invalid stuff");
                }
                catch (Exception e)
                {
                    _logger.ForContext(this).WithUnityTag("Error tag").Error(e, "This is an error");
                }
            });
            _threadButton.onClick.AddListener(() =>
            {
                var stopWatch = Stopwatch.StartNew();

                ThreadPool.QueueUserWorkItem(state =>
                {
                    stopWatch.Stop();
                    _logger.ForContext(this).WithUnityTag("Thread tag").Information("Log from thread {Id}, Invoke took: {Elapsed}", Thread.CurrentThread.ManagedThreadId, stopWatch.Elapsed);
                });
            });
        }
    }
}