using System.Dynamic;
using Newtonsoft.Json;

namespace AspNetCoreSession.Models
{
    public class IndexView
    {
        public string RawJson
        {
            get
            {
                dynamic obj = new ExpandoObject();

                if (User != null)
                    obj.User = User;
                if (Prefs != null)
                    obj.Prefs = Prefs;
                if (Location != null)
                    obj.Location = Location;

                return JsonConvert.SerializeObject(obj, Formatting.Indented);
            }
        }

        public object User { get; set; }
        public object Prefs { get; set; }
        public object Location { get; set; }
    }
}