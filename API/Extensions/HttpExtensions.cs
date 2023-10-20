using System.Text.Json;
using API.Helper;

namespace API.Extentions;

public static class HttpExtensions
{
    public static void AddPaginationHeader(this HttpResponse response, PaginationHeader header) {
        var jsonOptions = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
        response.Headers.Add("Pagination", JsonSerializer.Serialize(header, jsonOptions));
        // our clients will not be able to access the data inside this header
        response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
    }
}