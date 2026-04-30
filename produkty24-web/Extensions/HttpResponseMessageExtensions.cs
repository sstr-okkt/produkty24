namespace Produkty24_Web.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        public static void ThrowOnHttpError(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode) {
                var message = $"{(int)response.StatusCode}, {response.ReasonPhrase}";
                throw new ApiException(message);
            }
        }
    }
}
