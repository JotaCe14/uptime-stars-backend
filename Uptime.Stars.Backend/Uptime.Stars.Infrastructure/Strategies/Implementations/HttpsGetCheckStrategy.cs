using Uptime.Stars.Application.Core.Abstractions.Time;
using Uptime.Stars.Domain.Entities;

namespace Uptime.Stars.Infrastructure.Strategies.Implementations;
internal sealed class HttpsGetCheckStrategy(HttpClient httpClient, IClockTimer timer) : ICheckStrategy
{
    public async Task<CheckResult> CheckAsync(ComponentMonitor monitor, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, monitor.Target);

        foreach (var header in monitor.RequestHeaders)
        {
            var parts = header.Split(':', 2);
            if (parts.Length == 2)
                request.Headers.TryAddWithoutValidation(parts[0].Trim(), parts[1].Trim());
        }

        httpClient.Timeout = TimeSpan.FromMilliseconds(monitor.TiemoutInMilliseconds);

        try
        {
            timer.Start();

            var response = await httpClient.SendAsync(request, cancellationToken);

            timer.Stop();

            if (response is null)
            {
                return CheckResult.Down("No response");
            }

            if (!response.IsSuccessStatusCode)
            {
                return CheckResult.Down($"Failed with status code: {(int)response.StatusCode}");
            }

            if (!string.IsNullOrWhiteSpace(monitor.ExpectedText))
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (monitor.SearchMode is Domain.Enums.TextSearchMode.Include)
                {
                    if (!content.Contains(monitor.ExpectedText, StringComparison.OrdinalIgnoreCase))
                    {
                        return CheckResult.Down($"Expected text not found: {monitor.ExpectedText}");
                    }
                }
                else if (monitor.SearchMode is Domain.Enums.TextSearchMode.NotInclude)
                {
                    if (content.Contains(monitor.ExpectedText, StringComparison.OrdinalIgnoreCase))
                    {
                        return CheckResult.Down($"Unexpected text found: {monitor.ExpectedText}");
                    }
                }
            }

            return CheckResult.Up(timer.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            return CheckResult.Down(ex.Message);
        }
    }
}