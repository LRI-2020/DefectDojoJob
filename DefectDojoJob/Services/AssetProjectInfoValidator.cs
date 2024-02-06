using DefectDojoJob.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace DefectDojoJob.Services;

public class AssetProjectInfoValidator
{
    public JSchema GetValidationSchema()
    {
        string schemaJson = @"{
  'description': 'An assetProjectInfo',
  'type': 'object',
  'properties': {
    'Id': {'type': 'number'},
    'Name': {'type': 'string'}
  },
    'required': ['Id','Name']

}";

        return JSchema.Parse(schemaJson);
    }
    
    public class AssetProjectInfoJsonValidator : JsonValidator
    {
        public override void Validate(JToken value, JsonValidatorContext context)
        {
            if (value.Type == JTokenType.String)
            {
                string s = value.ToString();

                try
                {
                    
                    // test whether the string is a known culture, e.g. en-US, fr-FR
                }
                catch (Exception e)
                {
                    context.RaiseError($"Text '{s}' is not a valid culture name.");
                }
            }
        }

        public override bool CanValidate(JSchema schema)
        {
            // validator will run when a schema has a format of culture
            return (schema.Format == "culture");
        }
    }

    public bool HasRequiredProperties(AssetProjectInfo projectInfo)
    {
        return projectInfo.Id > 0
               && !string.IsNullOrEmpty(projectInfo.Name.Trim())
               && (!string.IsNullOrEmpty(projectInfo.ShortDescription?.Trim())
                   || !string.IsNullOrEmpty(projectInfo.DetailedDescription?.Trim()));
    }
}