namespace FreeConvert.Web.Client;

public class ExportResponse
{
    public ExportResult? Result { get; set; }
}

public class ExportResult
{
    public string? Url { get; set; }
}
