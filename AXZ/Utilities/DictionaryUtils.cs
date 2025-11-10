using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Windows.Input;

namespace AXZ.Utilities
{
    public static class DictionaryUtils
    {
        /// <summary>
        /// Sorts a dictionary by its keys in ascending order, optionally placing specified keys at the front in the given order.
        /// </summary>
        /// <param name="dictionary">The dictionary to sort.</param>
        /// <param name="preferredFrontKeys">Optional list of keys to prioritize at the beginning of the result.</param>
        /// <returns>A new dictionary with keys sorted and prioritized as specified.</returns>
        public static void SortDictionary(
            this Dictionary<dynamic, dynamic> dictionary,
            List<string> preferredFrontKeys = null)
        {
            var result = new Dictionary<dynamic, dynamic>();

            if (preferredFrontKeys != null)
            {
                foreach (var key in preferredFrontKeys)
                {
                    if (dictionary.ContainsKey(key) && !result.ContainsKey(key))
                    {
                        result[key] = dictionary[key];
                    }
                }
            }

            foreach (var key in dictionary.Keys.OrderBy(k => k))
            {
                if (!result.ContainsKey(key))
                {
                    result[key] = dictionary[key];
                }
            }
            dictionary.Clear();
            foreach(dynamic key in result.Keys)
            {
                dictionary.Add(key, result[key]);
            }
        }
        /// <summary>
        /// Sorts a dictionary by its keys in ascending order, optionally placing specified keys at the front in the given order.
        /// </summary>
        /// <param name="dictionary">The dictionary to sort.</param>
        /// <param name="preferredFrontKeys">Optional list of keys to prioritize at the beginning of the result.</param>
        /// <returns>A new dictionary with keys sorted and prioritized as specified.</returns>
        public static Dictionary<string, dynamic> SortDictionary(
            this Dictionary<string, dynamic> dictionary,
            List<string> preferredFrontKeys = null)
        {
            Dictionary<dynamic, dynamic> dynamicDict = ParseDictionaryToType<dynamic, dynamic>(dictionary);
            dynamicDict.SortDictionary();
            Dictionary<string, dynamic> result = ParseDictionaryToType<string, dynamic>(dynamicDict);
            return result;
        }

        private static Dictionary<TKey, TValue> ParseDictionaryToType<TKey, TValue>(IDictionary source)
        {
            var result = new Dictionary<TKey, TValue>();

            foreach (var key in source.Keys)
            {
                dynamic newKey = key;
                dynamic newValue = source[key];
                result.Add(newKey, newValue);
            }

            return result;
        }

    }
}
