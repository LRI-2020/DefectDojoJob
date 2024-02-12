using Newtonsoft.Json.Linq;

namespace DefectDojoJob.Helpers;

public static class DefectDojoApiDeserializer<T>
{
    public static T? Deserialize(string response)
    {
        var results = JObject.Parse(response)["results"];
        if (results == null || ((JArray)results).Count == 0) return default;

        return ((JArray)results)[0].ToObject<T>();
    }
}