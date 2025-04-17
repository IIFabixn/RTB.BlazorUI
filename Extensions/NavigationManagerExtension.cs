using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace RTB.BlazorUI.Extensions
{
    public static class NavigationManagerExtension
    {
        public static TValue? GetUriParams<TValue>(this NavigationManager navigationManager, string key)
        {
            var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            var jsonValue = query[key];
            if (string.IsNullOrEmpty(jsonValue))
                return default;

            try
            {
                return JsonSerializer.Deserialize<TValue>(jsonValue);
            }
            catch (JsonException)
            {
                return default;
            }
        }

        public static void NavigateWithParams(this NavigationManager navigationManager, string uri, params KeyValuePair<string, object>[] parameters)
        {
            var uriBuilder = new UriBuilder(navigationManager.ToAbsoluteUri(uri));
            var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
            foreach (var parameter in parameters)
            {
                query[parameter.Key] = JsonSerializer.Serialize(parameter.Value);
            }
            uriBuilder.Query = query.ToString();
            navigationManager.NavigateTo(uriBuilder.ToString());
        }
    }
}
