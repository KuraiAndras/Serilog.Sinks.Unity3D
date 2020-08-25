using Serilog;
using Serilog.Sinks.Unity3D;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SimpleLogger : MonoBehaviour
{
    [SerializeField] private Button _infoButton;
    [SerializeField] private Button _warningButton;
    [SerializeField] private Button _errorButton;
    private Serilog.ILogger _logger;

    private void Awake()
    {
        _logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Unity3D()
            .CreateLogger();
    }

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
    }
}