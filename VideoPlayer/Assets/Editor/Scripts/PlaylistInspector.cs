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
    private static PlaylistAsset targetAsset;
    private WindowsForms.OpenFileDialog filePicker;
    private ListView playlistView;
    private VideoClipData selectedItem;
    private const int itemHeight = 20;
    private const int listViewHeight = 200;
    void OnEnable()
    {
        targetAsset = (PlaylistAsset)target;
    }
    public override VisualElement CreateInspectorGUI()
    {
        root = new VisualElement();
        root.Clear();
        AddPropertyFields();
        AddPlaylistview();
        AddPlaylistUtilities();
        return root;
    }
    private void AddPropertyFields()
    {
        root.Add(new PropertyField(serializedObject.FindProperty("PlaylistTitle")));
        Label inspectorText = new Label("Add videoClips to this List");
        inspectorText.style.alignSelf = Align.Center;
        root.Add(inspectorText);
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
    private System.Func<VisualElement> makeItem = () => new Label();
    private System.Action<VisualElement, int> bindListItem = (e, i) => {
        (e as Label).text = targetAsset.VideoClipList[i].Name;
    };
    private void AddPlaylistview()
    {
        var playlistContainer = new VisualElement();
        playlistContainer.style.height = listViewHeight;
        playlistContainer.style.backgroundColor = Color.white;
        playlistContainer.style.borderBottomWidth = 2;
        playlistContainer.style.borderTopWidth = 2;
        playlistContainer.style.borderLeftWidth = 2;
        playlistContainer.style.borderRightWidth = 2;
        playlistContainer.style.borderColor = Color.grey;
        playlistView = new ListView(targetAsset.VideoClipList, itemHeight, makeItem, bindListItem);
        playlistView.selectionType = SelectionType.Single;
        playlistView.onItemChosen += obj => { selectedItem = (VideoClipData)obj; };
        playlistView.style.flexGrow = 1.0f;
        playlistContainer.Add(playlistView);
        root.Add(playlistContainer);
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
                    playlistView.Refresh();
                }
        }
    }
}
