using System.Net;

namespace Wex.Purchases.IntegrationTests.Configurations;

public sealed class FakeTreasuryHttpMessageHandler : HttpMessageHandler
{
    private readonly Queue<Func<HttpRequestMessage, HttpResponseMessage>> _responses;

    public FakeTreasuryHttpMessageHandler(params Func<HttpRequestMessage, HttpResponseMessage>[] responses)
    {
        _responses = new Queue<Func<HttpRequestMessage, HttpResponseMessage>>(responses);
    }

    public int RequestCount { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        RequestCount++;

        var responseFactory = _responses.Count == 0
            ? _ => new HttpResponseMessage(HttpStatusCode.OK)
            : _responses.Dequeue();

        var response = responseFactory(request);
        response.RequestMessage = request;

        return Task.FromResult(response);
    }
}
