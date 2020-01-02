using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(PlaylistAsset))]
public class PlaylistInspector : Editor
{
    private VisualElement root;
    private PlaylistAsset targetAsset;
    void OnEnable()
    {
        targetAsset = (PlaylistAsset)target;
    }

    public override VisualElement CreateInspectorGUI()
    {
        root = new VisualElement();
        root.Clear();
        root.Add(new PropertyField(serializedObject.FindProperty("PlaylistTitle")));
        root.Add(new Label("Drag videos directly to the list"));
        root.Add(new PropertyField(serializedObject.FindProperty("VideoClipList")));
        root.Bind(serializedObject);
        targetAsset.PlaylistTitle = serializedObject.targetObject.name;
        serializedObject.ApplyModifiedProperties();
        return root;
    }
    private void OnChenge()
    { }

}
