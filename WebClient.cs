using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;

namespace FreeConvert.Web.Client;

public class WebClient
{
    public static async Task ConvertFile(HttpClient client, string filePath, string destinationPath, string destinationFileName)
    {
        try
        {
            var response = await client.PostAsync("https://api.freeconvert.com/v1/process/import/upload", null);

            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                var task = JsonSerializer.Deserialize<UploadResponse>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var formDictionary = ToDictionary(task?.Result.Form.Parameters);

                await UploadFile(client, filePath, task?.Result?.Form?.Url, formDictionary);
                var converstionTask = await SendConvertRequest(client, task?.Id);
                var exportUrl = await GetConversionTaskUrl(client, converstionTask?.Id);
                await DownloadFile(client, exportUrl, destinationPath, destinationFileName);
            }
        }
        catch (Exception e)
        {
            //TODO - implement error handling
        }
    }

    private static Dictionary<string, string?> ToDictionary(object? obj)
    {
        return obj?.GetType().GetProperties().ToDictionary(
            propertyInfo => propertyInfo?.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? string.Empty,
            propertyInfo => propertyInfo.GetValue(obj, null)?.ToString()
        ) ?? new Dictionary<string, string?>();
    }

    private static async Task<ConversionResponse?> SendConvertRequest(HttpClient client, string? taskId)
    {
        try
        {
            var conversionRequest = new ConversionRequest
            {
                ImportTaskId = taskId,
                InputFormat = "midi",
                OutputFormat = "wav"
            };

            var content = new StringContent(JsonSerializer.Serialize(conversionRequest), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.freeconvert.com/v1/process/convert", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ConversionResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception e)
        {
            //TODO - implement error handling
        }

        return null;
    }

    private static async Task<string?> GetConversionTaskUrl(HttpClient client, string? taskId)
    {
        try
        {
            var response = await client.GetAsync($"https://api.freeconvert.com/v1/process/tasks/{taskId}/wait");
            var responseContent = await response.Content.ReadAsStringAsync();
            var task = JsonSerializer.Deserialize<ExportResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return task?.Result?.Url;
        }
        catch (Exception e)
        {
            //TODO - implement error handling
        }

        return null;
    }

    private static async Task DownloadFile(HttpClient client, string? url, string destinationPath, string destinationFileName)
    {
        string filePath = Path.Combine(destinationPath, destinationFileName);

        using HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
               fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
        {
            await contentStream.CopyToAsync(fileStream);
        }
    }

    private static async Task UploadFile(HttpClient client, string filePath, string? url, Dictionary<string, string> parameters)
    {
        try
        {
            var formData = new MultipartFormDataContent();
            foreach (var parameter in parameters)
            {
                formData.Add(new StringContent(parameter.Value), parameter.Key);
            }

            var midiFile = File.ReadAllBytes(filePath);

            formData.Add(new StreamContent(new MemoryStream(midiFile)), "file", "test.midi");

            var response = await client.PostAsync(url, formData);
            var responseContent = await response.Content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            //TODO - implement error handling
        }
    }
}
