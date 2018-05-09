using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace AutoScribeClient.ServerAccess {
    public static class HttpMessageHelper {
        /// <summary>
        /// The access key of the app service
        /// </summary>
        private const string MOBILE_SERVICE_ACCESS_KEY = "LOCAL_ACCESS_KEY"; //yMSZvAzB6Fo0AsuzQW8X2lkp91aRdJvA

        /// <summary>
        /// Sends a request to the api
        /// </summary>
        /// <param name="method">The HTTP method to use</param>
        /// <param name="request">The request to send</param>
        /// <param name="parameters">The parameters to use</param>
        /// <param name="stream">The data to include (optional)</param>
        /// <returns>The response of the api</returns>
        public static async Task<HttpResponseMessage> RequestApiAsync(MobileServiceClient client, HttpMethod method, string request, Dictionary<string, object> parameters, HttpContent content = null)
        {
            Dictionary<string, string> stringParameters = new Dictionary<string, string>()
            {
                { "accessKey", MOBILE_SERVICE_ACCESS_KEY }
            };

            foreach (KeyValuePair<string, object> parameter in parameters) {
                if (parameter.Value is string)
                    stringParameters.Add(parameter.Key, parameter.Value as string);
                else
                    stringParameters.Add(parameter.Key, Serialize(parameter.Value));
            }

            return await client.InvokeApiAsync("request/" + request, content, method, new Dictionary<string, string>(), stringParameters);
        }

        /// <summary>
        /// Serializes an object to a JSON string
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="value">The object to serialize</param>
        /// <returns>The JSON string with the serialized object</returns>
        public static string Serialize<T>(T value)
        {
            JsonSerializer serializer = new JsonSerializer();

            StringWriter writer = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(writer);

            serializer.Serialize(jsonWriter, value);

            return writer.ToString();
        }
        /// <summary>
        /// Deserializes the response content (JSON)
        /// </summary>
        /// <typeparam name="T">The type to deserialize to</typeparam>
        /// <param name="response">The server response</param>
        /// <returns>The deserialized object</returns>
        public static async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
        {
            string contentString = await response.Content.ReadAsStringAsync();

            StringReader reader = new StringReader(contentString);
            JsonTextReader jsonReader = new JsonTextReader(reader);

            return new JsonSerializer().Deserialize<T>(jsonReader);
        }
        /// <summary>
        /// Creates a stream with the response content
        /// </summary>
        /// <param name="response">The server response</param>
        /// <returns>The stream with the response content</returns>
        public static async Task<Stream> CreateContentStream(HttpResponseMessage response)
        {
            if (response.Content == null)
                return null;

            Stream stream = new MemoryStream();

            await response.Content.CopyToAsync(stream);

            return stream;
        }
    }
}
