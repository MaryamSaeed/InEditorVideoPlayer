using System.Collections;
using System.Collections.Generic;
using WindowsForms = System.Windows.Forms;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;

[CustomEditor(typeof(PlaylistAsset))]
public class PlaylistInspector : Editor
{
    private VisualElement root;
    private PlaylistAsset targetAsset;
    private WindowsForms.OpenFileDialog filePicker;
    void OnEnable()
    {
        targetAsset = (PlaylistAsset)target;
    }
    public override VisualElement CreateInspectorGUI()
    {
        root = new VisualElement();
        root.Clear();
        AddPropertyFields();
        AddPlaylistUtilities();
        return root;
    }
    private void AddPropertyFields()
    {
        root.Add(new PropertyField(serializedObject.FindProperty("PlaylistTitle")));
        Label inspectorText = new Label("Add videoClips Names and URl");
        inspectorText.style.alignSelf = Align.Center;
        root.Add(inspectorText);
        root.Add(new PropertyField(serializedObject.FindProperty("VideoClipList")));
        root.Bind(serializedObject);
        targetAsset.PlaylistTitle = serializedObject.targetObject.name;
        serializedObject.ApplyModifiedProperties();
    }
    private void AddPlaylistUtilities()
    {
        filePicker = new WindowsForms.OpenFileDialog();
        filePicker.Filter = "All Videos Files |*.mp4;";
        filePicker.Multiselect = true;
        Button addVideo = new Button();
        addVideo.text = "Add video from HD";
        addVideo.style.alignSelf = Align.Center;
        addVideo.clickable.clicked += () => OnAddVideoClicked();
        root.Add(addVideo);
    }
    private void AddPlaylistListview()
    { 
    
    }
    private void OnAddVideoClicked()
    {
        string path = Directory.GetCurrentDirectory();
        WindowsForms.DialogResult result = filePicker.ShowDialog();
        if (result == WindowsForms.DialogResult.OK)
        {
            var filesCount = filePicker.FileNames.Length;
            if (filesCount > 0)
                for (int i = 0; i < filesCount; i++)
                {
                    VideoClipData videoData = new VideoClipData();
                    videoData.URL = filePicker.FileNames[i];
                    videoData.Name = filePicker.SafeFileNames[i];
                    targetAsset.VideoClipList.Add(videoData);
                }
        }
    }

}
