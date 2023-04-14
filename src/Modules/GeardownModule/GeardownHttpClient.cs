
using System.Net.Http.Headers;
using Newtonsoft.Json;

public class GeardownHttpClient
{
    private HttpClient _client;

    private String _apiBaseUrl = "https://api.geardown.gg";

    private String _token = "Wfq1QvDtqWBZaMwyOi9ouKMYTXZyH6Ac";

    public GeardownHttpClient()
    {
        _client = new HttpClient();
    }
    
    private String makePath(String path)
    {
      return this._apiBaseUrl + path + "?token=" + this._token;
    }

    private ByteArrayContent convertObjectToHttpContent(object data)
    {
        var myContent = JsonConvert.SerializeObject(data);
        var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
        var byteContent = new ByteArrayContent(buffer);
        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        
        return byteContent;
    }

    public async Task<String> Post(String path, object data)
    {
        HttpResponseMessage response = await _client.PostAsync(this.makePath(path), convertObjectToHttpContent(data));
        response.EnsureSuccessStatusCode();

        // return URI of the created resource.
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<String> Put(String path, object data)
    {
        HttpResponseMessage response = await _client.PutAsync(this.makePath(path), convertObjectToHttpContent(data));
        response.EnsureSuccessStatusCode();

        // return URI of the created resource.
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<String> Get(String path, KeyValuePair<string, string>[]? parameters)
    {
        var requestPath = this.makePath(path);

        if (parameters != null) {
          foreach (var param in parameters) {
            requestPath += ("&" + param.Key + "=" + param.Value);
          }
        }

        System.Console.WriteLine(requestPath);
        HttpResponseMessage response = await _client.GetAsync(requestPath);
        response.EnsureSuccessStatusCode();

        // return URI of the created resource.
        return await response.Content.ReadAsStringAsync();
    }
}
