using System;
using System.IO;
using System.Reflection;

namespace lorcanaApp
{
    public class EmbeddedResources
    {
        public static Stream GetResourceStream(string resPath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "lorcanaApp." + resPath;

            return assembly.GetManifestResourceStream(resourceName);
        }

        public static string GetResourceString(string resPath)
        {
            using (Stream stream = GetResourceStream(resPath))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    return result;
                }
            }
        }
    }
}
