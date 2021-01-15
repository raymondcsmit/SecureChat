using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Chats.FunctionalTests.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<object> DeserializeAsync(this HttpResponseMessage message, Type type)
        {
            var contentString = await message.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject(contentString, type);
        }

        public static async Task<T> DeserializeAsync<T>(this HttpResponseMessage message)
        {
            var contentString = await message.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(contentString);
        }

        public static async Task<object> DeserializeAsync(this HttpResponseMessage message)
        {
            var contentString = await message.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject(contentString);
        }
    }
}
