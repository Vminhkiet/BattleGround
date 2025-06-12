#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HexGrid))]
public class HexGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        HexGrid grid = (HexGrid)target;

        if (GUILayout.Button("Build Grid"))
        {
            grid.BuildGrid();
        }
        if (GUILayout.Button("Clear"))
        {
            ClearChildren(grid.transform);
        }
    }

    private void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            GameObject.DestroyImmediate(child.gameObject);
        }
    }
}
#endif