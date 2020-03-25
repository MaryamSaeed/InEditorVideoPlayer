using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class PlaylistController
{
    public UnityEvent PlaylistChanged;
    public PlaylistEvent PlayVideoAtPath;
    private static PlaylistAsset nowPlaying;
    private VisualElement windowRoot;
    private VisualElement videoListArea;
    private ListView videoListView;
    private Label playlistTitle;
    private Label PlaylistStatus;
    private int currentVideoId = 0;
    private const int itemHeight = 20;
    public PlaylistController(VisualElement root)
    {
        windowRoot = root;
        PlaylistChanged = new UnityEvent();
        PlayVideoAtPath = new PlaylistEvent();
        InitPlaylistVisualElements();
        InitControllerButtons();
    }
    private void InitPlaylistVisualElements()
    {
        PlaylistStatus = windowRoot.Q<Label>("PlaylistStatus");
        videoListArea = windowRoot.Q(className: "video-list-area");
        nowPlaying = ScriptableObject.CreateInstance<PlaylistAsset>();
        nowPlaying = Resources.Load<PlaylistAsset>("Playlists/DefultPlaylist");
        var picker = windowRoot.Q<ObjectField>("PlaylistPicker");
        picker.objectType = typeof(PlaylistAsset);
        picker.SetValueWithoutNotify(nowPlaying);
        picker.label = "Now Playing";
        picker.RegisterCallback<ChangeEvent<Object>>((evt) => OnPlaylistChanged(evt.newValue));
        videoListView = InitVideoListView(nowPlaying.VideoClipList);
        UpdatePlaylistStatus();
    }
    private void UpdatePlaylistStatus()
    {
        PlaylistStatus.text = "This playlist is empty";
        if (nowPlaying.VideoClipList != null)
            if (nowPlaying.VideoClipList.Count > 0)
                PlaylistStatus.visible = false;
            else
                PlaylistStatus.visible = true;
    }
    private System.Func<VisualElement> makeItem = () => new Label();
    private System.Action<VisualElement, int> bindListItem = (e, i) =>
    {
        (e as Label).text = nowPlaying.VideoClipList[i].Name;
    };
    private ListView InitVideoListView(List<VideoClipData> source)
    {
        var videoListView = new ListView(source, itemHeight, makeItem, bindListItem);
        videoListView.selectionType = SelectionType.Single;
        videoListView.onItemChosen += obj => OnVideoChosen(obj);
        videoListView.style.flexGrow = 1.0f;
        videoListArea.Add(videoListView);
        return videoListView;
    }
    private void InitControllerButtons()
    {
        var videoPlayerButtons = windowRoot.Query<Button>();
        videoPlayerButtons.ForEach(SetupControllerButtons);
    }
    private void SetupControllerButtons(Button button)
    {
        var buttonIcon = button.Q(className: "videoplayer-button-icon");
        button.text = button.parent.name;
        switch (button.parent.name)
        {
            case "Next":
                button.clickable.clicked += () => NextVideo();
                break;
            case "Prev":
                button.clickable.clicked += () => PreviousVideo();
                break;
        }
    }
    private void DestroyOldListView()
    {
        if (videoListArea.Q<ListView>() != null)
            videoListArea.Remove(videoListView);
    }
    private void OnPlaylistChanged(Object value)
    {
        if (playlistTitle == null)
            playlistTitle = windowRoot.Q<Label>("PlayListTitle");
        if (value != null)
            NewPlaylistValue(value);
        else
            NonePlaylistValue();
        if (PlaylistChanged != null)
            PlaylistChanged.Invoke();
    }
    private void NonePlaylistValue()
    {
        playlistTitle.text = "Playlist";
        PlaylistStatus.text = "No playlist selected";
        PlaylistStatus.visible = true;
        videoListArea.Remove(videoListView);
    }
    private void NewPlaylistValue(Object newvalue)
    {
        DestroyOldListView();
        nowPlaying = (PlaylistAsset)newvalue;
        videoListView = InitVideoListView(nowPlaying.VideoClipList);
        playlistTitle.text = nowPlaying.PlaylistTitle;
        UpdatePlaylistStatus();
    }
    private void PlayVideoWithId(int id)
    {
        videoListView.selectedIndex = currentVideoId;
        var url = nowPlaying.VideoClipList[id].URL;
        if (PlayVideoAtPath != null)
            PlayVideoAtPath.Invoke(url);
    }
    private void NextVideo()
    {
        if (currentVideoId < nowPlaying.VideoClipList.Count - 1)
        {
            currentVideoId++;
            PlayVideoWithId(currentVideoId);
        }
    }
    private void PreviousVideo()
    {
        if (currentVideoId > 0)
        {
            currentVideoId--;
            PlayVideoWithId(currentVideoId);
        }
    }
    private void OnVideoChosen(System.Object chosen)
    {
        if (PlayVideoAtPath != null)
            PlayVideoAtPath.Invoke(((VideoClipData)chosen).URL);
        currentVideoId = videoListView.selectedIndex;
    }
}