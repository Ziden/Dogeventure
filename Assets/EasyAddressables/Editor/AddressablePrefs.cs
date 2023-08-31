using UnityEditor;

namespace Assets.EasyAddressables.Editor
{
    public static class AddressablePrefs
    {
        /// <summary>
        /// Path that will be used where the addressables will be read from
        /// </summary>
        public static string SOURCE_PATH
        {
            get => EditorPrefs.GetString("Easy.Src", "Assets/EasyAddressables/Examples/ExampleBundles");
            set => EditorPrefs.SetString("Easy.Src", value);
        }

        /// <summary>
        /// Path that will be used to save auto-generated code
        /// </summary>
        public static string GENERATION_PATH
        {
            get => EditorPrefs.GetString("Easy.Gen", "Assets/EasyAddressables/Runtime/Generated");
            set => EditorPrefs.SetString("Easy.Gen", value);
        }
    }
}
