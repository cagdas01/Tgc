using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Tgc.Core.Extensions;
public static class ParsingExtensions
{
    public static Dictionary<string, (string type, bool isRequired, int maxLength)> ParseProperties(this string mappingInfo)
    {
        var excludedProperties = new HashSet<string>();
        var propertyDict = new Dictionary<string, (string type, bool isRequired, int maxLength)>();
        var matches = Regex.Matches(mappingInfo, @"Property<(\w+)>\(x => x\.(\w+)\).+HasColumnName\(@""\w+""\).*?(IsRequired\(\))?.*?(HasMaxLength\((\d+)\))?;");

        foreach (Match match in matches)
        {
            var type = match.Groups[1].Value;
            var propertyName = match.Groups[2].Value;
            var isRequired = !string.IsNullOrEmpty(match.Groups[3].Value);
            var maxLength = string.IsNullOrEmpty(match.Groups[5].Value) ? 0 : int.Parse(match.Groups[5].Value);

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
}
