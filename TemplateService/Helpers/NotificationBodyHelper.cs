namespace TemplateService.Helpers;

/// <summary>
/// Provides helper methods for notification content.
/// </summary>
public static class NotificationContentHelper
{
    /// <summary>
    /// Gets a set of unique variable names from the specified string.
    /// </summary>
    /// <param name="str">The string to search for variable names.</param>
    /// <returns>A read-only set of unique variable names.</returns>
    public static IReadOnlySet<string> GetUniqueVariableNames(string str)
    {
        var allMatches = RegexHelper.VariableRegex.Matches(str);
        return allMatches.Count != 0
            ? new HashSet<string>(allMatches.Select(m => m.Value))
            : [];
    }
}
