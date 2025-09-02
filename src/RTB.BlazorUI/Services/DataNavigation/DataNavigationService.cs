using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Services.DataNavigation
{
    /// <summary>
    /// Provides a simple, in-memory handoff of data between navigations in Blazor and
    /// a thin wrapper around <see cref="NavigationManager"/> for page/component navigation.
    /// </summary>
    /// <remarks>
    /// - Keys and values supplied to <see cref="NavigateTo(string, bool, bool, IDictionary{string, object?}?)"/> are stored in-memory
    ///   and are not encoded into the URL. Retrieve them on the destination via <see cref="TryGetData{T}"/>.
    /// - The service is not thread-safe; it relies on Blazor's typical single-threaded UI usage.
    /// - Lifetime of the stored data matches the service's DI lifetime (commonly <c>Scoped</c> in Blazor).
    /// </remarks>
    public interface IDataNavigationService
    {
        /// <summary>
        /// Navigates to the specified <paramref name="uri"/> and optionally attaches a set of in-memory parameters
        /// that can be retrieved on the destination via <see cref="TryGetData{T}"/>.
        /// </summary>
        /// <param name="uri">The target URI (relative or absolute) to navigate to.</param>
        /// <param name="forceLoad">
        /// If true, bypasses client-side routing and forces the browser to load the new page from the server.
        /// </param>
        /// <param name="replace">
        /// If true, replaces the current entry in the history stack (does not create a new history entry).
        /// </param>
        /// <param name="parameter">
        /// Optional key-value pairs to store in-memory prior to navigation. Keys are case-sensitive and
        /// are not included in the URL. Values are available on the destination via <see cref="TryGetData{T}"/>.
        /// </param>
        void NavigateTo(string uri, bool forceLoad = false, bool replace = false, IDictionary<string, object?>? parameter = null);

        /// <summary>
        /// Attempts to retrieve a previously stored value for <paramref name="key"/> and cast it to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Expected type of the stored value.</typeparam>
        /// <param name="key">The key used when storing the value.</param>
        /// <param name="value">
        /// When this method returns true, contains the value cast to <typeparamref name="T"/>. Otherwise, set to default.
        /// </param>
        /// <param name="remove">
        /// If true (default), removes the value from the store upon successful retrieval. Set false to keep it for subsequent reads.
        /// </param>
        /// <returns>
        /// True if a value exists for <paramref name="key"/> and is of type <typeparamref name="T"/>; otherwise false.
        /// </returns>
        bool TryGetData<T>(string key, [NotNullWhen(true)] out T? value, bool remove = true);

        /// <summary>
        /// Determines whether a value exists for the given <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key exists; otherwise false.</returns>
        bool HasData(string key);

        /// <summary>
        /// Clears stored values.
        /// </summary>
        /// <param name="prefix">
        /// If null, clears all stored data. If specified, removes keys that start with the prefix using
        /// <see cref="string.StartsWith(string)"/> default semantics (culture-sensitive, case-sensitive).
        /// </param>
        void Clear(string? prefix = null);
    }

    /// <summary>
    /// Default implementation of <see cref="IDataNavigationService"/> that stores parameters in-memory
    /// and delegates navigation to <see cref="NavigationManager"/>.
    /// </summary>
    /// <remarks>
    /// - Not thread-safe; intended for typical Blazor UI usage.
    /// - The dictionary is in-memory only and not persisted across app restarts or different circuits/sessions.
    /// </remarks>
    /// <example>
    /// Inject and use in a component:
    /// <code>
    /// @inject IDataNavigationService Nav
    ///
    /// // Source component
    /// Nav.NavigateTo("/details", parameter: new Dictionary&lt;string, object?&gt;
    /// {
    ///     ["itemId"] = 42
    /// });
    ///
    /// // Destination component
    /// if (Nav.TryGetData&lt;int&gt;("itemId", out var id))
    /// {
    ///     // use id
    /// }
    /// </code>
    /// </example>
    public class DataNavigationService(NavigationManager navigationManager) : IDataNavigationService
    {
        private readonly Dictionary<string, object?> _data = [];

        /// <summary>
        /// Adds or overwrites a value for the specified <paramref name="key"/>.
        /// </summary>
        private void SetData(string key, object? value)
        {
            if (!_data.TryAdd(key, value))
            {
                _data[key] = value;
            }
        }

        /// <inheritdoc/>
        public bool TryGetData<T>(string key, [NotNullWhen(true)] out T? value, bool remove = true)
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void NavigateTo(string uri, bool forceLoad = false, bool replace = false, IDictionary<string, object?>? parameter = null)
        {
            if (parameter is { Count: > 0 })
            {
                foreach (var param in parameter)
                {
                    SetData(param.Key, param.Value);
                }
            }

            navigationManager.NavigateTo(uri, forceLoad, replace);
        }

        /// <inheritdoc/>
        public bool HasData(string key) => _data.ContainsKey(key);
    }
}
