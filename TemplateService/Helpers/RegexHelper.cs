using System.Text.RegularExpressions;

namespace TemplateService.Helpers;

/// <summary>
/// Provides regular expressions for use within the application.
/// </summary>
internal static partial class RegexHelper
{
    /// <summary>
    /// Gets the regular expression for matching variable names.
    /// </summary>
    internal static Regex VariableRegex = VariableRegexPrecompiled();

    /// <summary>
    /// Compiles the regular expression for matching variable names.
    /// </summary>
    /// <returns>The compiled regular expression.</returns>
    [GeneratedRegex(@"\${{\s*[^{}]+\s*}}", RegexOptions.Compiled)]
    private static partial Regex VariableRegexPrecompiled();
}