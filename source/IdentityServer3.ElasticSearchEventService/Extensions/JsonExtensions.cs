using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace IdentityServer3.ElasticSearchEventService.Extensions
{
    public static class JsonExtensions
    {
        public static string ToJsonSuppressErrors(this object item)
        {
            return item.ToJson(false, true);
        }

        public static string ToJson(this object item, bool indented = false, bool suppressErrors = false)
        {
            var settings = new JsonSerializerSettings();
            if (suppressErrors)
            {
                settings.Error = SuppressErrors;
            }
            var formatting = indented ? Formatting.Indented : Formatting.None;
            return JsonConvert.SerializeObject(item, formatting, settings);
        }

        private static void SuppressErrors(object sender, ErrorEventArgs e)
        {
            e.ErrorContext.Handled = true;
        }
    }
}