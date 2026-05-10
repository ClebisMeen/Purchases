using System.Net;
using System.Net.Http.Json;

namespace Wex.Purchases.IntegrationTests.Fixtures;

public sealed class TreasuryApiFixture
{
    public HttpResponseMessage CreateSuccessfulTreasuryResponse()
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new
            {
                data = new[]
                {
                    new
                    {
                        country = "Brazil",
                        currency = "Real",
                        country_currency_desc = "Brazil-Real",
                        exchange_rate = "5.00",
                        record_date = "2026-05-01"
                    }
                }
            })
        };

        return response;
    }
}
