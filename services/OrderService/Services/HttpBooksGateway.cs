using System.Net;

namespace OrderService.Services;

public sealed class HttpBooksGateway : IBooksGateway
{
    private readonly HttpClient _http;

    public HttpBooksGateway(HttpClient http) => _http = http;

    public async Task<bool> ExistsAsync(Guid bookId, CancellationToken ct = default)
    {
        using var resp = await _http.GetAsync($"/books/{bookId}", ct);
        return resp.StatusCode == HttpStatusCode.OK;
    }
}

