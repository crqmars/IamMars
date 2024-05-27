using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http.Json
{
    public static partial class HttpClientJsonExtensions
    {
        public static Task<HttpResponseMessage> PostAsJsonAsync<TValue>(this HttpClient client, string requestUri, TValue value, IEnumerable<KeyValuePair<string, string>> headers, JsonSerializerOptions options = null, CancellationToken cancellationToken = default)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            JsonContent content = JsonContent.Create(value, mediaType: null, options);
            foreach (var kv in headers)
            {
                content.Headers.Add(kv.Key, kv.Value);
            }
            return client.PostAsync(requestUri, content, cancellationToken);
        }
    }
}
