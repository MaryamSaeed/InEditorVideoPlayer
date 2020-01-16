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
        EditorUtility.SetDirty(targetAsset);
    }
    public override VisualElement CreateInspectorGUI()
    {
        serializedObject.Update();
        root = new VisualElement();
        root.Clear();
        root.style.flexDirection = FlexDirection.Column;
        root.style.justifyContent = Justify.SpaceBetween;
        AddPlaylistInpectorVisualElements();
        return root;
    }
    private void InitFilePicker()
    {
        filePicker = new WindowsForms.OpenFileDialog();
        filePicker.Filter = "All Videos Files |*.mp4;";
        filePicker.Multiselect = true;
    }
    private void AddPropertyFields()
    {
        root.Add(new PropertyField(serializedObject.FindProperty("PlaylistTitle")));
        Label inspectorText = new Label("List of video Clips");
        inspectorText.style.alignSelf = Align.Center;
        root.Add(inspectorText);
        targetAsset.PlaylistTitle = serializedObject.targetObject.name;
        serializedObject.ApplyModifiedProperties();
    }
    private void AddRemovalInstructions()
    {
        Label removalText = new Label("double click on video to remove");
        removalText.style.alignSelf = Align.Center;
        removalText.style.color = Color.red;
        root.Add(removalText);
    }
    private void AddPlaylistInpectorVisualElements()
    {
        InitFilePicker();
        AddPropertyFields();
        AddPlaylistVisualElement();
        SetupPlaylistInspectorButtons();
        AddRemovalInstructions();
    }
    private void SetupPlaylistInspectorButtons()
    {
        //Video Addition Button
        AddButton2InspectorRoot("Add video from HD",OnAddVideoClicked);
        //Video Removal Button
        removeVideoButton = AddButton2InspectorRoot("Remove video from list", OnRemoveVideoClicked);
        removeVideoButton.visible = false;
    }
    private System.Func<VisualElement> makeItem = () => new Label();
    private System.Action<VisualElement, int> bindListItem = (e, i) =>
    {
        (e as Label).text = targetAsset.VideoClipList[i].Name;
    };
    private Button AddButton2InspectorRoot(string buttontext, System.Action action)
    {
        Button newButton = new Button();
        newButton.text = buttontext;
        newButton.style.alignSelf = Align.Center;
        newButton.clickable.clicked += action;
        root.Add(newButton);
        return newButton;
    }
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
    private void AddListView2Container(VisualElement container)
    {
        playlistView = new ListView(targetAsset.VideoClipList, itemHeight, makeItem, bindListItem);
        playlistView.selectionType = SelectionType.Single;
        playlistView.onItemChosen += obj => OnItemChosen(obj);
        playlistView.style.flexGrow = 1.0f;
        container.Add(playlistView);
    }
    private void OnItemChosen(System.Object obj)
    {
        selectedItem = (VideoClipData)obj;
        removeVideoButton.visible = true;
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
            serializedObject.ApplyModifiedProperties();
        }
    }
    private void OnRemoveVideoClicked()
    {
        targetAsset.VideoClipList.Remove(selectedItem);
        playlistView.Refresh();
        removeVideoButton.visible = false;
        serializedObject.ApplyModifiedProperties();
    }
}
