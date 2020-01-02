using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(PlaylistAsset))]
public class PlaylistInspector : Editor
{
    private PlaylistAsset playlistAsset;
    private VisualElement root;
    private VisualTreeAsset visualTree;
    private SerializedProperty playlistTitle;
    void OnEnable()
    {
        playlistAsset = (PlaylistAsset)target;
        root = new VisualElement();
        visualTree = Resources.Load<VisualTreeAsset>("UXMLs/PlaylistInspector");
    }

    public override VisualElement CreateInspectorGUI()
    {
        //Clear the visual element
        root.Clear();
        ////Clone the visual tree into our Visual Element so it can be drawn
        visualTree.CloneTree(root);
        var prop = serializedObject.FindProperty("PlaylistTitle");
        var propfiled = root.Q<PropertyField>("playlist-title-field");
        propfiled.Bind(serializedObject);
        Debug.Log("Title: " + playlistAsset.PlaylistTitle);
        serializedObject.ApplyModifiedProperties();
        //Add A Callback For Each Button
        //UQueryBuilder<VisualElement> builder = _RootElement.Query(classes: new string[] { "prefab-button" });
        //builder.ForEach(AddButtonIcon);

        return root;
    }
    //public override void OnInspectorGUI()
    //{
    //    serializedObject.Update();
    //    EditorGUILayout.PropertyField(playlistTitle);
    //    Debug.Log(playlistTitle.ToString());
    //    serializedObject.ApplyModifiedProperties();
    //}
}
