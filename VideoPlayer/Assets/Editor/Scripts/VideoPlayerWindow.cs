using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.Video;
using System.IO;
public class VideoPlayerWindow : EditorWindow
{
    private VideoPlayer videoPlayer;
    private VideoController videoController;
    private VisualElement windowRoot;
    [MenuItem("Video/VideoPlayerWindow _%p")]
    public static void ShowVideoPlayerWindow()
    {
        var window = GetWindow<VideoPlayerWindow>();
        window.titleContent = new GUIContent("Unity Video Player");
    }
    private void OnEnable()
    {
        windowRoot = rootVisualElement;
        ApplyWindowStyle(windowRoot);
        videoController = new VideoController(videoPlayer,windowRoot);
    }
    private void ApplyWindowStyle(VisualElement root)
    {
        root = rootVisualElement;
        root.styleSheets.Add(Resources.Load<StyleSheet>("StyleSheets/VideoPlayerStyle"));
        var buttonsVisualTree = Resources.Load<VisualTreeAsset>("UXMLs/VideoPlayerMain");
        buttonsVisualTree.CloneTree(root);
        InitVideoArea(root);
    }
    private void InitVideoArea(VisualElement root)
    {
        videoPlayer = FindObjectOfType<VideoPlayer>();
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = InitVideoRendererTexture(root);
        videoPlayer.url = string.Concat(Directory.GetCurrentDirectory(), "/videoSmple/movie1.mp4");
        videoPlayer.sendFrameReadyEvents = true;
        videoPlayer.frameReady += OnNewFrameReady;
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.Prepare();
    }
    private void OnNewFrameReady(VideoPlayer source, long frameIdx)
    {
        this.Repaint();
    }
    private void OnVideoPrepared(VideoPlayer source)
    {
       
    }
    private RenderTexture InitVideoRendererTexture(VisualElement root)
    {
        var videoPlayerImage = root.Query<Image>();
        RenderTexture targetRt = new RenderTexture(new RenderTextureDescriptor(1024, 1024));
        videoPlayerImage.First().image = targetRt;
        return targetRt;
    }
    private void OnDisable()
    {
        videoPlayer.Stop();
        videoPlayer.targetTexture.Release();
    }
}
