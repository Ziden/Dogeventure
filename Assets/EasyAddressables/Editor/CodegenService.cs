using Code.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.EasyAddressables.Editor
{
    internal static class CodegenService
    {
        internal static void GenerateService()
        {
            var imports = new HashSet<string>();
            foreach (var t in GenerationDefaults.TYPES)
            {
                imports.Add(t.Value.Namespace);
            }

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(AddressableGeneration.HEADER);
            stringBuilder.AppendLine("using System.Collections.Generic;");
            stringBuilder.AppendLine("using System;");
            stringBuilder.AppendLine("using System.Threading.Tasks;");
            stringBuilder.AppendLine("using EasyAddressables;");

            foreach (var import in imports) stringBuilder.AppendLine($"using {import};");

            stringBuilder.AppendLine($"namespace GameAddressables");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("\tpublic class AssetService : IAssetService");
            stringBuilder.AppendLine("\t{");

            foreach (var t in GenerationDefaults.TYPES)
            {
                var enumName = t.Key;
                var classType = t.Value.Name;
                stringBuilder.AppendLine($"\t\tprivate AssetContainer<{enumName},{classType}> _{enumName.ToLower()} = new AssetContainer<{enumName},{classType}>();");
            }

            stringBuilder.AppendLine($"\t\t");

            foreach (var t in GenerationDefaults.TYPES)
            {
                var enumName = t.Key;
                var classType = t.Value.Name;
                stringBuilder.AppendLine($"\t\tpublic async Task Get{classType}Async({enumName} id, Action<{classType}> onComplete=null)");
                stringBuilder.AppendLine("\t\t{");
                stringBuilder.AppendLine($"\t\t\tawait _{enumName.ToLower()}.LoadAsync(id, onComplete);");
                stringBuilder.AppendLine("\t\t}");
                stringBuilder.AppendLine("\t\t");

                if (typeof(GameObject).IsAssignableFrom(t.Value))
                {
                    stringBuilder.AppendLine($"\t\tpublic async Task Instantiate{enumName}Async({enumName} id, Action<{classType}> onComplete)");
                    stringBuilder.AppendLine("\t\t{");
                    stringBuilder.AppendLine($"\t\t\tawait _{enumName.ToLower()}.InstantiateAsync(id, onComplete);");
                    stringBuilder.AppendLine("\t\t}");
                    stringBuilder.AppendLine("\t\t");
                }
            }

            stringBuilder.AppendLine("\t}");
            stringBuilder.AppendLine("}");
            File.WriteAllText($"{AddressablePrefs.GENERATION_PATH}/AssetService.cs", stringBuilder.ToString());
        }

    }
}
