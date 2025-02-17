using System.Net.Http.Json;

namespace MoveitApiClient.Extensions
{
    public static class JsonTransformerExtension
    {
        public static JsonContent CreateJsonContent(this object obj)
        {
            return JsonContent.Create(obj);
        }

        public static async Task<T> ReadFromJsonAsync<T>(this HttpResponseMessage response, CancellationToken cancellationToken)
        {
            try
            {
                return await response.Content.ReadFromJsonAsync<T>(cancellationToken)
                       ?? throw new InvalidOperationException($"Could not read expected body of type {typeof(T).Name}.");
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                throw new InvalidOperationException($"Could not read expected body of type {typeof(T).Name}.", ex);
            }
        }
    }
}
