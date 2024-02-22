using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace DefectDojoJob.Helpers;

public static class DefectDojoApiDeserializer<T>
{
    public static T? DeserializeFirstItemOfResults(string response)
    {
        try
        {
            var results = JObject.Parse(response)["results"];
            if (results == null || ((JArray)results).Count == 0) return default;
            return ((JArray)results)[0].ToObject<T>();
        }
        catch (Exception e)
        {
            throw new Exception($"Could not retrieve {nameof(T)}");
        }
        
    }
    
    public static T DeserializeSingleItem(string response, string errorMessage)
    {
        try
        {
           return JObject.Parse(response).ToObject<T>()??throw new Exception(errorMessage);
        }
        catch (Exception e)
        {
            throw new Exception(errorMessage);
        }
    }
}