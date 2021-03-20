using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Nunoguz_Boilerplate.Shared.Utilities
{
    public static class TextContentReplacer
    {
        // html replacer
        public static async Task<string> TextContentReplacerAsync(string filePath, Dictionary<string, string> replacements)
        {
            var content = await File.ReadAllTextAsync(filePath);
            var newContent = /*new StreamReader(content).ReadToEnd();*/content;
            foreach (var r in replacements)
            {
                newContent = newContent.Replace(r.Key, r.Value);
            }
            return newContent;
        }
    }
}
