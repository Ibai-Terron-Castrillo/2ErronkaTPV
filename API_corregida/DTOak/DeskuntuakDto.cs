using System.Text.Json.Serialization;

namespace JatetxeaApi.DTOak
{
    public class DeskuntuKodeaDto
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("order_id")]
        public int? OrderId { get; set; }
    }

    public class DeskuntuEmaitzaDto
    {
        [JsonPropertyName("valid")]
        public bool Valid { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("percentage")]
        public double Percentage { get; set; }

        [JsonPropertyName("code_id")]
        public int? CodeId { get; set; }
    }
}
