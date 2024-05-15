using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Tgc.Core.Extensions
{
    public static class ParsingExtensions
    {
        public static Dictionary<string, (string type, bool isRequired, int maxLength)> ParseProperties(this string mappingInfo)
        {
            var excludedProperties = new HashSet<string>(new string[] { "TriggerConversionStatus" }, StringComparer.OrdinalIgnoreCase);
            var propertyDict = new Dictionary<string, (string type, bool isRequired, int maxLength)>();
            var matches = Regex.Matches(mappingInfo, @"Property<([\w?.]+)>\(x => x\.(\w+)\).*?HasColumnName\(@""\w+""\).*?(IsRequired\(\))?.*?(HasMaxLength\((\d+)\))?;");

            foreach (Match match in matches)
            {
                var isRequired = false;
                var type = match.Groups[1].Value;
                var propertyName = match.Groups[2].Value;
                var maxLength = string.IsNullOrEmpty(match.Groups[5].Value) ? 0 : int.Parse(match.Groups[5].Value);

                if (type.Contains("System."))
                    type = type.Replace("System.", "");

                if (match.Value.Contains("IsRequired()"))
                {
                    isRequired = true;
                }

                if (!excludedProperties.Contains(propertyName))
                    propertyDict.Add(propertyName, (type, isRequired, maxLength));
            }

            return propertyDict;
        }

        public static string ExtractEntityName(this string mappingInfo)
        {
            var match = Regex.Match(mappingInfo, @"modelBuilder\.Entity<(\w+)>");
            return match.Groups[1].Value;
        }
        public static string ExtractPrimaryKey(this string properties)
        {
            var keyMatch = Regex.Match(properties, @"modelBuilder\.Entity<\w+>\(\)\.HasKey\(@""(\w+)""\);");
            return keyMatch.Success ? keyMatch.Groups[1].Value : null;
        }

        public static Dictionary<string, string> ExtractDefaultValues(string mappingInfo, Dictionary<string, (string type, bool isRequired, int maxLength)> properties)
        {
            var defaultValueDict = new Dictionary<string, string>();
            var defaultValueMatches = Regex.Matches(mappingInfo, @"Property<[\w?.]+>\(x => x\.(\w+)\).*?HasDefaultValueSql\(@""([^""]+)""\);");

            foreach (Match match in defaultValueMatches)
            {
                var propertyName = match.Groups[1].Value;
                var defaultValue = match.Groups[2].Value;

                if (properties.ContainsKey(propertyName))
                {
                    var type = properties[propertyName].type;
                    defaultValue = FormatDefaultValue(defaultValue, type.TrimEnd('?'));
                    defaultValueDict.Add(propertyName, defaultValue);
                }
            }

            return defaultValueDict;
        }

        private static string FormatDefaultValue(string defaultValue, string type)
        {
            if (type == "string")
            {
                return $"\"{defaultValue.Trim('\'')}\"";
            }
            else if (type == "int" || type == "long" || type == "short")
            {
                return defaultValue;
            }
            else if (type == "decimal")
            {
                return $"{defaultValue}m";
            }
            else if (type == "bool")
            {
                return defaultValue.ToLower() == "1" ? "true" : "false";
            }
            else
            {
                return defaultValue;
            }
        }

        public static string GetContextName(this string moduleName) => moduleName.Replace("Management", "Context");
    }
}