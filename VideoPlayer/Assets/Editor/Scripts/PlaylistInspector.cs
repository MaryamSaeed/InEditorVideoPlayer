using WindowsForms = System.Windows.Forms;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;
using System.Collections.Generic;

[CustomEditor(typeof(PlaylistAsset))]
public class PlaylistInspector : Editor
{
    private VisualElement root;
    private ListView playlistView;
    private VideoClipData selectedItem;
    private const int itemHeight = 20;
    private const int listViewHeight = 200;
    private static PlaylistAsset targetAsset;
    private static Button removeVideoButton;
    private WindowsForms.OpenFileDialog filePicker;
    void OnEnable()
    {
        targetAsset = (PlaylistAsset)serializedObject.targetObject;
        if (targetAsset.VideoClipList == null)
            targetAsset.VideoClipList = new List<VideoClipData>();
        EditorUtility.SetDirty(targetAsset);
    }
    /// <summary>
    /// clearing original inspector structure
    /// building the new structure
    /// </summary>
    /// <returns></returns>
    public override VisualElement CreateInspectorGUI()
    {
        root = new VisualElement();
        root.Clear();
        root.style.flexDirection = FlexDirection.Column;
        root.style.justifyContent = Justify.SpaceBetween;
        AddPlaylistInspectorVisualElements();
        return root;
    }
    /// <summary>
    /// creates file picker instance that
    /// picks only videos in MP4 format
    /// </summary>
    private void InitFilePicker()
    {
        filePicker = new WindowsForms.OpenFileDialog();
        filePicker.Filter = "All Videos Files |*.mp4;";
        filePicker.Multiselect = true;
    }
    /// <summary>
    /// draws property fields corresponding to playlist properties
    /// to edit and sa
    /// </summary>
    private void AddPropertyFields()
    {
        root.Add(new PropertyField(serializedObject.FindProperty("PlaylistTitle")));
        Label inspectorText = new Label("List of video Clips");
        inspectorText.style.alignSelf = Align.Center;
        root.Add(inspectorText);
        targetAsset.PlaylistTitle = serializedObject.targetObject.name;
        serializedObject.ApplyModifiedProperties();
    }
    /// <summary>
    /// writes popup instructions to help remove video 
    /// from list
    /// </summary>
    private void AddRemovalInstructions()
    {
        Label removalText = new Label("double click on video to remove");
        removalText.style.alignSelf = Align.Center;
        removalText.style.color = Color.red;
        root.Add(removalText);
    }
    /// <summary>
    /// adds the visual elements of the new inspector structure
    /// </summary>
    private void AddPlaylistInspectorVisualElements()
    {
        InitFilePicker();
        AddPropertyFields();
        AddPlaylistVisualElement();
        SetupPlaylistInspectorButtons();
        AddRemovalInstructions();
    }
    /// <summary>
    /// configure tha buttons with its corresponding functionality
    /// </summary>
    private void SetupPlaylistInspectorButtons()
    {
        //Video Addition Button
        AddButton2InspectorRoot("Add video from HD", OnAddVideoClicked);
        //Video Removal Button
        removeVideoButton = AddButton2InspectorRoot("Remove video from list", OnRemoveVideoClicked);
        removeVideoButton.visible = false;
    }
    /// <summary>
    /// draws labels of the Listview elements
    /// </summary>
    private System.Func<VisualElement> makeItem = () => new Label();
    /// <summary>
    /// binds the list viewelement label with the corresponding list element
    /// </summary>
    private System.Action<VisualElement, int> bindListItem = (e, i) =>
    {
        (e as Label).text = targetAsset.VideoClipList[i].Name;
    };
    /// <summary>
    /// creates a button with a name and functionality
    /// </summary>
    /// <param name="buttontext">button screen name</param>
    /// <param name="action">the functionality of the button</param>
    /// <returns></returns>
    private Button AddButton2InspectorRoot(string buttontext, System.Action action)
    {
        Button newButton = new Button();
        newButton.text = buttontext;
        newButton.style.alignSelf = Align.Center;
        newButton.clickable.clicked += action;
        root.Add(newButton);
        return newButton;
    }
    /// <summary>
    /// creats a container to hold the listviwe visual elemnt
    /// </summary>
    private void AddPlaylistVisualElement()
    {
        VisualElement playlistContainer = new VisualElement();
        AddListView2Container(playlistContainer);
        ApplyContainerStyle(playlistContainer);
        root.Add(playlistContainer);
    }
    private void ApplyContainerStyle(VisualElement container)
    {
        container.style.borderTopWidth = 2;
        container.style.borderLeftWidth = 2;
        container.style.borderRightWidth = 2;
        container.style.borderBottomWidth = 2;
        container.style.height = listViewHeight;
        container.style.borderColor = Color.grey;
        container.style.backgroundColor = Color.white;
    }
    /// <summary>
    /// initilizes the playlist's listview at the given container
    /// </summary>
    /// <param name="container">list view holder element</param>
    private void AddListView2Container(VisualElement container)
    {
        playlistView = new ListView(targetAsset.VideoClipList, itemHeight, makeItem, bindListItem);
        playlistView.selectionType = SelectionType.Single;
        playlistView.onItemChosen += obj => OnItemChosen(obj);
        playlistView.style.flexGrow = 1.0f;
        container.Add(playlistView);
    }
    /// <summary>
    /// shows the Remove video button when the user highlights a video
    /// fom the playlist's listview
    /// </summary>
    /// <param name="obj"></param>
    private void OnItemChosen(System.Object obj)
    {
        selectedItem = (VideoClipData)obj;
        removeVideoButton.visible = true;
    }
    /// <summary>
    /// triggers the Filepicker and enables the user to select video(s)
    /// to be added to the playlist
    /// </summary>
    public void OnAddVideoClicked()
    {
        string path = Directory.GetCurrentDirectory();
        WindowsForms.DialogResult result = filePicker.ShowDialog();
        if (result == WindowsForms.DialogResult.OK)
        {
            var filesCount = filePicker.FileNames.Length;
            if (filesCount > 0)
                for (int i = 0; i < filesCount; i++)
                    AddPickedVideo2Playlist(i);
        }
    }
    /// <summary>
    /// extracts the video data from the returned picker object
    /// </summary>
    /// <param name="i"></param>
    private void AddPickedVideo2Playlist(int i)
    {
        VideoClipData videoData = new VideoClipData();
        videoData.URL = filePicker.FileNames[i];
        videoData.Name = filePicker.SafeFileNames[i];
        AddVideoData2ListView(videoData);
    }
    /// <summary>
    /// saves the desired video'uri and at HD to paylist
    /// </summary>
    /// <param name="videoData">the object contaning the video location on HD</param>
    public void AddVideoData2ListView(VideoClipData videoData)
    {
        targetAsset.VideoClipList.Add(videoData);
        playlistView.Refresh();
    }
    /// <summary>
    /// delets the highlighted video from the playlist
    /// </summary>
    private void OnRemoveVideoClicked()
    {
        targetAsset.VideoClipList.Remove(selectedItem);
        playlistView.Refresh();
        removeVideoButton.visible = false;
    }
}