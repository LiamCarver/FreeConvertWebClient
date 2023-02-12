using System.Text.Json.Serialization;

namespace FreeConvert.Web.Client;

public class ExportRequest
{
    [JsonPropertyName("input")]
    public string[]? TaskIds { get; set; }
}
