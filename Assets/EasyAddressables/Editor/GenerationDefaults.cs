using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Assets.EasyAddressables.Editor
{
    internal static class GenerationDefaults
    {

        // Path Contains -> Enum Name
        internal static readonly Dictionary<string, string> CONFIG = new Dictionary<string, string>()
        {
            //{".flac", "AssetSoundFX"},
            //{".wav", "AssetSoundFX"},
            //{".ogg", "AssetSoundFX"},
            //{".mp3", "AssetMusic"},
            {"/SFX", "AssetSoundEffect"},
            {"/Collectible", "CollectiblePrefab"},
            {"/Weapons", "WeaponPrefab"},
            {"/Shields", "ShieldPrefab"},
            {"/VFX", "VfxPrefab"},
            {"/Config", "ConfigAsset"},
            {"/Object", "ObjectPrefab"},
            {".png", "AssetSprite"},
            {".unity", "AssetScene"},
            {".jpeg", "AssetSprite"},
            {".jpg", "AssetSprite"},
            {".tga", "AssetTexture"},
            {".mat", "AssetMaterial"},
            {".uxml", "AssetUIScreen"},
            {"UIPanel", "AssetPanelSettings"},
        };

        internal static readonly Dictionary<string, Type> TYPES = new Dictionary<string, Type>()
        {
            {"AssetSoundEffect", typeof(AudioClip) },
            //{"AssetMusic", typeof(AudioClip)},
            {"ObjectPrefab", typeof(GameObject)},
            {"CollectiblePrefab", typeof(GameObject)},
            {"WeaponPrefab", typeof(GameObject)},
            {"AssetScene", typeof(Scene)},
            {"ShieldPrefab", typeof(GameObject)},
            {"VfxPrefab", typeof(GameObject)},
            {"ConfigAsset", typeof(ScriptableObject)},
            {"AssetSprite", typeof(Sprite)},
            {"AssetTexture", typeof(Texture2D)},
            {"AssetMaterial", typeof(Material)},
            {"AssetUIScreen", typeof(VisualTreeAsset)},
            {"AssetPanelSettings", typeof(PanelSettings)},
        };

        internal static string ResolveDefaultAsset(string address)
        {
            var type = AssetDatabase.GetMainAssetTypeAtPath(address);
            var name = $"Asset{type.Name}";
            TYPES[name] = type;
            return name;
        }
    }
}
