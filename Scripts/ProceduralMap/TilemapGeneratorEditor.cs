using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
[CustomEditor(typeof(TilemapGenerator))]
public class TilemapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TilemapGenerator generator = (TilemapGenerator)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Generation Controls", EditorStyles.boldLabel);

        if (GUILayout.Button("Generate Tilemap", GUILayout.Height(40)))
        {
            GenerateInEditor(generator);
        }
    }

    private void GenerateInEditor(TilemapGenerator generator)
    {
        if (Application.isPlaying)
        {
            generator.Generate();
        }
        else
        {
            RecordUndo(generator);
            generator.Generate();
            MarkSceneDirty();
        }
    }

    private void RecordUndo(TilemapGenerator generator)
    {
        var floor = serializedObject.FindProperty("floorTilemap")?.objectReferenceValue as Tilemap;
        var background = serializedObject.FindProperty("backgroundTilemap")?.objectReferenceValue as Tilemap;

        if (floor != null)
            Undo.RegisterCompleteObjectUndo(floor, "Generate Dungeon");

        if (background != null)
            Undo.RegisterCompleteObjectUndo(background, "Generate Dungeon");

        Undo.RegisterCompleteObjectUndo(generator, "Generate Dungeon");
    }

    private void MarkSceneDirty()
    {
        if (!Application.isPlaying)
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }
    }
}
#endif