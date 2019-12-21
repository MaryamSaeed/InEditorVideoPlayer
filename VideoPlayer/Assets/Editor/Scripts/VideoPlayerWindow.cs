using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.Video;
using System.IO;
public class VideoPlayerWindow :EditorWindow
{
    static VideoPlayerWindow window;
    [MenuItem("Video/VideoPlayerWindow _%p")]
    public static void ShowVideoPlayerWindow()
    {
        window = GetWindow<VideoPlayerWindow>("Unity Video Player");
        window.maxSize = new Vector2(800, 800);
        
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
    private void InitVideoArea(VisualElement root)
    {
        var videoPlayerImage = root.Query<Image>();
        videoPlayerImage.First().image = Resources.Load<RenderTexture>("Video");
        videoPlayer = FindObjectOfType<VideoPlayer>();
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = string.Concat(Directory.GetCurrentDirectory(), "/videoSmple/movie1.mp4");
        videoPlayer.sendFrameReadyEvents = true;
        videoPlayer.frameReady += OnNewFrameReady;
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.Prepare();
    }
 

    private void OnNewFrameReady(VideoPlayer source, long frameIdx)
    {
       window.Repaint();
    }
    private void OnVideoPrepared(VideoPlayer source)
    {
        Debug.Log("VideoPrepared");
        videoPlayer.Play();
    }
}
