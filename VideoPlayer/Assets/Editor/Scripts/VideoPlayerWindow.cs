using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.Video;
using System.IO;

public class VideoPlayerWindow :EditorWindow
{
    [MenuItem("Video/VideoPlayerWindow _%p")]
    public static void ShowVideoPlayerWindow()
    {
        var window = GetWindow<VideoPlayerWindow>("Unity Video Player");
    }

    private void OnEnable()
    {
        var root = rootVisualElement;
        root.styleSheets.Add(Resources.Load<StyleSheet>("VideoPlayerStyle"));
        InitButtonContainer(root);
        InitVideoArea(root);
    }
    private void SetupButton(Button button)
    {
        var buttonIcon = button.Q(className: "videoplayer-button-icon");
        button.text = button.parent.name;
        button.clickable.clicked += () => Debug.Log("button " + button.parent.name);
    }
    private void InitButtonContainer(VisualElement root)
    {
        var buttonsVisualTree = Resources.Load<VisualTreeAsset>("VideoPlayerMain");
        buttonsVisualTree.CloneTree(root);
        var videoPlayerButtons = root.Query<Button>();
        videoPlayerButtons.ForEach(SetupButton);
    }
    VideoPlayer videoPlayer;
    Texture TargetDisplay;
    private void InitVideoArea(VisualElement root)
    {
        var videoPlayerImage = root.Query<Image>();
        TargetDisplay = videoPlayerImage.First().image;
        videoPlayer = FindObjectOfType<VideoPlayer>();
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = string.Concat(Directory.GetCurrentDirectory(), "/videoSmple/movie1.mp4");
        videoPlayer.renderMode = VideoRenderMode.APIOnly;
        videoPlayer.sendFrameReadyEvents = true;
        videoPlayer.frameReady += OnNewFrameReady;
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.Prepare();
    }

    private void OnNewFrameReady(VideoPlayer source, long frameIdx)
    {
        if (source.texture)
            TargetDisplay = (Texture)source.texture;
        Debug.Log(source.texture as Texture);
    }
    private void OnVideoPrepared(VideoPlayer source)
    {
        Debug.Log("VideoPrepared");
        //videoPlayer.Play();
    }
}
