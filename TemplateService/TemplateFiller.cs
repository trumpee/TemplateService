namespace TemplateService;

public class TemplateFiller
{
    public static string FillTemplate(
        string dest, Dictionary<string, string> variables)
    {
        const string pattern = "${{0}}";
        foreach (var item in variables)
        {
            var replace = "${{" + item.Key + "}}";
            dest = dest.Replace(replace, item.Value);
        }
        return dest;
    }
}
