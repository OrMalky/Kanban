using IntroSE.Kanban.Backend.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public class Response
    {
        [JsonPropertyName("ErrorMessage")]
        public string ErrorMessage { get; set; }

        [JsonPropertyName("ReturnValue")]
        public object ReturnValue { get; set; }

        public Response()
        {
            ErrorMessage = null;
            ReturnValue = null;
        }

        public Response(string ErrorMsg)
        {
            ErrorMessage = ErrorMsg;
            ReturnValue = null;
        }

        public Response(string ErrorMsg, object obj)
        {
            ErrorMessage = ErrorMsg;
            ReturnValue = obj;
        }

        /// <summary>
        /// Parses this Response object into JSON, including its content.
        /// </summary>
        /// <returns>A JSON string representing this Response object.</returns>
        public string ToJson()
        {
            try
            {
                var options = new JsonSerializerOptions();
                options.WriteIndented = true;
                options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                return JsonSerializer.Serialize(this, GetType(), options);
            }
            catch
            {
                return "Error : Failed to serialize Response object";
            }
        }
    }
}
