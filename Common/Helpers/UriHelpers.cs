using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Helpers
{
    public static class UriHelpers
    {
        public static Uri BuildUri(string root, NameValueCollection query)
        {
            var uriBuilder = new UriBuilder(root);
            var queryString = HttpUtility.ParseQueryString(uriBuilder.Query);

            foreach (var key in query.Cast<string>().Where(key => !string.IsNullOrEmpty(query[key])))
            {
                queryString[key] = query[key];
            }

            uriBuilder.Query = queryString.ToString();
            return uriBuilder.Uri;
        }

        public static Uri BuildUri(string root, object query)
        {
            return BuildUri(root, GetNameValueCollection(query));
        }

        private static NameValueCollection GetNameValueCollection(object query)
        {
            var collection = new NameValueCollection();
            foreach (var prop in query.GetType().GetProperties())
            {
                collection.Add(prop.Name, prop.GetValue(query).ToString());
            }
            return collection;
        }
    }
}
