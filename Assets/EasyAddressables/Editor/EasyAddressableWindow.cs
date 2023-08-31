using UnityEngine;
using UnityEditor;
using Code.Editor;
using Assets.EasyAddressables.Editor;
using UnityEditor.AddressableAssets.Build.AnalyzeRules;
using UnityEditor.AddressableAssets;
using UnityEditor.EasyAddressables.Bundler;

namespace EasyAddressables.Editor
{
    public class EasyAddressables : EditorWindow
    {
        private string _src = "/Assets/Examples/Addressables/";
        private string _gen = "/Assets/EasyAddressables/Runtime/Generated/";

        private bool _ignoreSmall = false;
        private bool _organize = true;
        private bool _removeDependencies = false;

        [MenuItem("Window/Easy Addressables")]
        [MenuItem("EasyAddressables/Open Window")]
        static void Init()
        {
            EasyAddressables window = (EasyAddressables)EditorWindow.GetWindow(typeof(EasyAddressables));
            window._src = AddressablePrefs.SOURCE_PATH;
            window._gen = AddressablePrefs.GENERATION_PATH;
            window.Show();
        }

        [MenuItem("EasyAddressables/Generate Addressable Map")]
        public static void GenerateAddressableIds()
        {
            AddressableGeneration.EasyAddressablesCodegen();
        }

        void GuiLine(int i_height = 1)

        {

            Rect rect = EditorGUILayout.GetControlRect(false, i_height);

            rect.height = i_height;

            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));

        }

        void OnGUI()
        {
            GUILayout.Label("Easy Addressables", EditorStyles.boldLabel);

            GUILayout.Label("Auto generate addressable service on the below folder");
            _gen = EditorGUILayout.TextField("Destination Folder", _gen);
            if (GUILayout.Button("Generate Classes From Addressables"))
            {
                AddressablePrefs.GENERATION_PATH = _gen;
                AddressableGeneration.EasyAddressablesCodegen();
            }

            GuiLine();

            GUILayout.Label("Auto makes all files inside the given folder addressables");
            _src = EditorGUILayout.TextField("Addressables Folder", _src);

            if (GUILayout.Button("Create Addressables"))
            {
                AddressablePrefs.SOURCE_PATH = _src;

                AddressableGeneration.CreateMissingAddressables();

                if (_organize)
                {
                    var bundler = new EasyAddressableBundler();
                    EasyAddressableBundler.FOLDER_NAME = _src;
                    EasyAddressableBundler.REMOVE_UNUSED = _removeDependencies;
                    bundler.RefreshAnalysis(AddressableAssetSettingsDefaultObject.Settings);
                }
            }

            GuiLine();

            GUILayout.Label("Addressable Organizer");
            _src = EditorGUILayout.TextField("Addressables Folder", _src);

            GUILayout.Label("Remove unused/unlinked addressables. Requires another addressable to be dependant on.");
            _removeDependencies = EditorGUILayout.ToggleLeft("Remove Unused", _removeDependencies);

            GUILayout.Label("Ignores small addressables. Handy im games with high volume of low size assets.");
            _ignoreSmall = EditorGUILayout.ToggleLeft("Ignore Small", _ignoreSmall);

            if (GUILayout.Button("Organize Addressables"))
            {
                Debug.Log("Organizing");
                AddressablePrefs.SOURCE_PATH = _src;

                var bundler = new EasyAddressableBundler();
                EasyAddressableBundler.IGNORE_SMALL = _ignoreSmall;
                EasyAddressableBundler.FOLDER_NAME = _src;
                EasyAddressableBundler.REMOVE_UNUSED = _removeDependencies;
                var rules = bundler.RefreshAnalysis(AddressableAssetSettingsDefaultObject.Settings);
                bundler.FixIssues(AddressableAssetSettingsDefaultObject.Settings);
            }

        }
    }
}

   