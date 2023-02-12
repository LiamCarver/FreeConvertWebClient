using System.Text.Json.Serialization;

namespace FreeConvert.Web.Client;

public class UploadResponse
{
    public UploadResult? Result { get; set; }

    public string? Id { get; set; }
}

public class UploadForm
{
    public string? Url { get; set; }
    public UploadParameters? Parameters { get; set; }
}

public class UploadParameters
{
    [JsonPropertyName("expires")]
    public long Expires { get; set; }

    [JsonPropertyName("size_limit")]
    public long SizeLimit { get; set; }

    [JsonPropertyName("max_file_count")]
    public int MaxFileCount { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public class UploadResult
{
    public UploadForm? Form { get; set; }
}