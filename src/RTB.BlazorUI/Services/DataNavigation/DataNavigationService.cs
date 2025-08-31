using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Services.DataNavigation
{
    public interface IDataNavigationService
    {
        void NavigateTo(string uri, bool forceLoad = false, bool replace = false, IDictionary<string, object?>? parameter = null);
        bool TryGetData<T>(string key, out T? value, bool remove = true);
        bool HasData(string key);
        void Clear(string? prefix = null);
    }

    public class DataNavigationService(NavigationManager navigationManager) : IDataNavigationService
    {
        private readonly Dictionary<string, object?> _data = [];

        private void SetData(string key, object? value)
        {
            if (!_data.TryAdd(key, value))
            {
                _data[key] = value;
            }
        }

        public bool TryGetData<T>(string key, out T? value, bool remove = true)
        {
            if (_data.TryGetValue(key, out var raw) && raw is T t)
            {
                value = t;
                if (remove) _data.Remove(key);
                return true;
            }

            value = default;
            return false;
        }

        public void Clear(string? prefix = null)
        {
            if (prefix is null)
            {
                _data.Clear();
            }
            else
            {
                var keys = _data.Keys.Where(k => k.StartsWith(prefix)).ToList();
                foreach (var key in keys)
                    _data.Remove(key);
            }
        }

        public void NavigateTo(string uri, bool forceLoad = false, bool replace = false, IDictionary<string, object?>? parameter = null)
        {
            if (parameter is { Count: > 0})
            {
                foreach (var param in parameter)
                {
                    SetData(param.Key, param.Value);
                }
            }

            navigationManager.NavigateTo(uri, forceLoad, replace);
        }

        public bool HasData(string key) => _data.ContainsKey(key);
    }
}
