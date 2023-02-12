using System.Text.Json.Serialization;

namespace FreeConvert.Web.Client;

public class ConversionRequest
{
    [JsonPropertyName("input")]
    public string? ImportTaskId { get; set; }

    [JsonPropertyName("input_format")]
    public string? InputFormat { get; set; }

    [JsonPropertyName("output_format")]
    public string? OutputFormat { get; set; }
}