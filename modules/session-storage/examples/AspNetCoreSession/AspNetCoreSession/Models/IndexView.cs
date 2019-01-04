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
                if (ShoppingCart != null)
                    obj.ShoppingCart = ShoppingCart;
                if (Location != null)
                    obj.Location = Location;

                return JsonConvert.SerializeObject(obj, Formatting.Indented);
            }
        }

        public object User { get; set; }
        public object ShoppingCart { get; set; }
        public object Location { get; set; }
        public int SessionCount { get; set; }
    }
}