using UnityEditor;
using UnityEngine;


#if UNITY_EDITOR
namespace Src.Tools
{
    public class HierarchyContext
    {
        [MenuItem("GameObject/Yolo/Remove Colliders", false, 0)]
        public static void RemoveColliders(MenuCommand menuCommand)
        {
            var selected = Selection.gameObjects;

            foreach (var gameObject in selected)
            {
                var colliders = gameObject.GetComponentsInChildren<Collider>();

                foreach (var collider in colliders)
                {
                    Object.DestroyImmediate(collider);
                }
            }
        }

        [MenuItem("GameObject/Yolo/Remove Colliders", true,0)]
        public static bool ValidateRemoveColliders()
        {
            return Selection.gameObjects != null && Selection.gameObjects.Length >= 1;
        }
    }
}
#endif