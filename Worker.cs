using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TestApplication
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly HttpClient _client;
        private readonly int _requestInterval;
        private readonly Uri _requestUri;
        private Timer _timer;

        public Worker(ILogger<Worker> logger, HttpClient client, IConfiguration config)
        {
            _logger = logger;
            _client = client;
            _requestUri = config.GetUriConfig("RequestUri");

            var requestsPerMinute = config.GetIntConfig("RequestsPerMinute");
            _requestInterval = 60_000 / requestsPerMinute; // in ms
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(Start, stoppingToken, 0, _requestInterval);

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);

            return Task.CompletedTask;
        }

        private async void Start(object state)
        {
            var stoppingToken = (CancellationToken)state;

            if (stoppingToken.IsCancellationRequested)
                return;

            await _client.GetAsync(_requestUri);
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        }
    }
}
