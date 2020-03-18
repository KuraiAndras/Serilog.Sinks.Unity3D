using Serilog;
using Serilog.Sinks.Unity3D;
using UnityEngine;
using UnityEngine.UI;

namespace Samples.SimpleUsage.Scripts
{
    [RequireComponent(typeof(Button))]
    public class SimpleLogger : MonoBehaviour
    {
        private Button _button;
        private Serilog.ILogger _logger;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Unity3D()
                .CreateLogger();
        }

        private void Start() => _button.onClick.AddListener(() => _logger.Information("Hello"));
    }
}
