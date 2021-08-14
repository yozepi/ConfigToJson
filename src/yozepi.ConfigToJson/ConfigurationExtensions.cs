/* Copyright 2021 Joe Cleland (yozepi)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
namespace ConfigToJson
{
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Linq;

    /// <summary>
    /// Extension methods for converting <see cref="IConfiguration"/> instances into JSON./>
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Convert an <see cref="IConfiguration"/> instance into a JSON string.
        /// </summary>
        /// <param name="config">The <see cref="IConfiguration"/> instance that is to be converted.</param>
        /// <returns>
        /// Returns a JSON formatted string of the configuration and sub configuration elements.
        /// </returns>
        public static string ToJsonString(this IConfiguration config)
        {
            var json = config.ToJToken();
            return json.ToString();
        }

        /// <summary>
        /// Convert an <see cref="IConfiguration"/> instance into a <see cref="JToken"/> instance.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static JToken ToJToken(this IConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            switch (config)
            {
                case IConfigurationRoot root:
                    return BuildFromRoot(root);

                case IConfigurationSection section:
                    return BuildFromSection(section);

                default:
                    throw new ArgumentException($"Unknown configuration type: {config.GetType().FullName}", nameof(config));
            }
         }

        private static JToken BuildFromRoot(IConfigurationRoot root)
        {
            var result = new JObject();
            foreach(var section in root.GetChildren())
            {
                result.Add(section.Key, section.ToJToken());
            }

            return result;
        }

        private static JToken BuildFromSection(IConfigurationSection section)
        {
            if (section.Value == null)
            {
                if (section.IsArray())
                {
                    return BuildArray(section);
                }

                return BuildObject(section);
            }

            return BuildValue(section.Value);
        }

        private static JToken BuildValue(string value)
        {
            JToken result;
            try
            {
                if ("true".Equals(value, StringComparison.OrdinalIgnoreCase)
                    || "false".Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    // boolean only parses when it's lower case.
                    value = value.ToLowerInvariant();
                }
                result = JValue.Parse(value);
            }
            catch (JsonReaderException)
            {
                // parsing failure, try parsing it as a string.
                result = JValue.Parse("\"" + value + "\"");
            }

            return result;
        }

        private static JToken BuildObject(IConfigurationSection section)
        {
            var result = new JObject();
            foreach (var subSection in section.GetChildren())
            {
                result.Add(subSection.Key, subSection.ToJToken());
            }

            return result;
        }

        private static JToken BuildArray(IConfigurationSection section)
        {
            var result = new JArray();
            foreach (var subSection in section.GetChildren())
            {
                result.Add(subSection.ToJToken());
            }

            return result;
        }

        private static bool IsArray(this IConfigurationSection section)
        {
            // Array items have keys that are string values of their index.
            // If all the keys can be parsed into an int then it's a good bet this should be parsed as an array.
            return section.GetChildren().All(subSection => int.TryParse(subSection.Key, out int _));
        }
    }
}
