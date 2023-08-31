using Code.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameAddressables;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.EasyAddressables.Editor
{
    internal static class CodegenEnum
    {

        internal static void GenerateEnum()
        {
            var old = new Dictionary<string, int>();
            foreach (var kp in AddressIdMap.IdMap)
            {
                old[kp.Value] = kp.Key;
            }
            
            IDictionary<string, List<AddressableAssetEntry>> Categorized = new Dictionary<string, List<AddressableAssetEntry>>();
            foreach (var cfg in GenerationDefaults.CONFIG.Values) Categorized[cfg] = new List<AddressableAssetEntry>();
            Categorized["AssetGeneric"] = new List<AddressableAssetEntry>();

            foreach (var a in AddressableGeneration.GetAddressables())
            {
                bool fit = false;
                foreach (var cfg in GenerationDefaults.CONFIG)
                {
                    if (a.AssetPath.Contains(cfg.Key))
                    {
                        Categorized[cfg.Value].Add(a);
                        fit = true;
                    }
                }
                if (!fit)
                {
                    var category = GenerationDefaults.ResolveDefaultAsset(a.AssetPath);
                    if (!Categorized.ContainsKey(category)) Categorized[category] = new List<AddressableAssetEntry>();
                    Categorized[category].Add(a);
                }
            }
            Dictionary<int, string> _idMap = new();
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(AddressableGeneration.HEADER);
            stringBuilder.AppendLine("using System.Collections.Generic;");
            stringBuilder.AppendLine($"namespace GameAddressables");
            stringBuilder.AppendLine("{");
            foreach (var kp in Categorized)
            {
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine($"\tpublic enum {kp.Key}");
                stringBuilder.AppendLine("\t{");
                
                foreach (var a in kp.Value)
                {
                    int id = 0;
                    if (old.TryGetValue(a.address, out var existingId))
                    {
                        id = existingId;
                    }
                    else
                    {
                        id = Mathf.Max(old.Values.Max() + 1, _idMap.Keys.Max() + 1);
                    }

                    stringBuilder.AppendLine($"\t\t{AddressableGeneration.FormatEnumName(a.MainAsset.name)} = {id},");
                    _idMap[id] = a.address;
                    old[a.address] = id;
                }
                stringBuilder.AppendLine("\t}");
                stringBuilder.AppendLine("");
            }
            stringBuilder.AppendLine($"\tpublic static class AddressIdMap");
            stringBuilder.AppendLine("\t{");
            stringBuilder.AppendLine("\t\tpublic static IReadOnlyDictionary<int, string> IdMap = new Dictionary<int, string>() {");
            foreach (var kp in _idMap)
                stringBuilder.AppendLine($"\t\t\t{{ {kp.Key}, \"{kp.Value}\"}},");
            stringBuilder.AppendLine("\t\t};");
            stringBuilder.AppendLine("\t}");
            stringBuilder.AppendLine("}");
            File.WriteAllText($"{AddressablePrefs.GENERATION_PATH}/Addresses.cs", stringBuilder.ToString());
            Debug.Log($"Addressable Enum generated in {AddressablePrefs.GENERATION_PATH}/Addresses.cs");
        }
    }
}
