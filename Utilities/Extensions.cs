namespace TFSImportAndExport.Utilities;

public static class Extensions
{
    public static string Mask(this string text)
    {
        var maskList = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("ABC", "XYZ")
        };

        var rv = text.Trim();

        foreach (var maskString in maskList)
        {
            if (rv.Contains(maskString))
            {
                rv = rv.Replace(maskString.Key, maskString.Value);
            }
        }

        return rv;
    }
}