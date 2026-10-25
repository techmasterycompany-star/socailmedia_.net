using System.Text;
using System.Text.RegularExpressions;

namespace Social_Media_Platform.Infrastructure.Helpers
{
    public static class HashtagNormalizer
    {
        private static readonly Regex Pattern = new(@"(?<![\p{L}\p{M}\p{N}_])#([\p{L}\p{M}\p{N}_]+)",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static IReadOnlyDictionary<string, string> Extract(string? text)
        {
            var hashtags = new Dictionary<string, string>(StringComparer.Ordinal);
            var normalizedText = (text ?? string.Empty).Normalize(NormalizationForm.FormKC);

            foreach (Match match in Pattern.Matches(normalizedText))
            {
                var name = match.Groups[1].Value;
                var normalizedName = name.ToLowerInvariant();

                hashtags.TryAdd(normalizedName, name);
            }

            return hashtags;
        }

        public static string Normalize(string? value)
        {
            var normalized = (value ?? string.Empty).Normalize(NormalizationForm.FormKC).Trim();
            if (normalized.StartsWith('#'))
                normalized = normalized[1..];

            return Pattern.IsMatch($"#{normalized}") &&
                   Pattern.Match($"#{normalized}").Value == $"#{normalized}"
                ? normalized.ToLowerInvariant()
                : string.Empty;
        }
    }
}
