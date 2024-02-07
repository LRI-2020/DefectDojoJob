namespace DefectDojoJob.Services;

public class DefectDojoConnector
{
    private readonly HttpClient httpClient;

    public DefectDojoConnector(IConfiguration configuration, HttpClient httpClient)
    {
        this.httpClient = httpClient;
        httpClient.DefaultRequestHeaders.Add("Authorization", configuration["ApiToken"]);
        var url = configuration["DefectDojoBaseUrl"];
        if (url != null) httpClient.BaseAddress = new Uri(url);
    }

    public async Task<HttpResponseMessage> GetDefectDojoGroupByName()
    {
        var url = "dojo_groups/";
        return await httpClient.GetAsync(url);
    }
}