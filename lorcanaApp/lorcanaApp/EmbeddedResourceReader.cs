using System;
using System.IO;
using System.Reflection;

public static class EmbeddedResourceReader
{
    public static string ReadEmbeddedResourceAsString(string resourceName)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string resourceFullName = $"{assembly.GetName().Name}.{resourceName}";
        using (Stream resourceStream = assembly.GetManifestResourceStream(resourceFullName))
        {
            if (resourceStream == null)
            {
                throw new Exception($"Die eingebettete Ressource '{resourceName}' wurde nicht gefunden.");
            }
            using (StreamReader reader = new StreamReader(resourceStream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
