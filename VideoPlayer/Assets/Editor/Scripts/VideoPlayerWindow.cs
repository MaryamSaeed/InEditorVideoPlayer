using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.Video;
using System.IO;
using UnityEditor.UIElements;

public class VideoPlayerWindow : EditorWindow
{
    private VideoPlayer videoPlayer;
    private VideoController videoController;
    private PlaylistController playlistController;
    private VisualElement windowRoot;
    [MenuItem("Video/VideoPlayerWindow _%vp")]
    public static void ShowVideoPlayerWindow()
    {
        var window = GetWindow<VideoPlayerWindow>();
        window.titleContent = new GUIContent("Unity Video Player");
    }
    private void OnEnable()
    {
        windowRoot = rootVisualElement;
        ApplyWindowStyle(windowRoot);
        InitSubscribers();
    }
    private void ApplyWindowStyle(VisualElement root)
    {
        root = rootVisualElement;
        root.styleSheets.Add(Resources.Load<StyleSheet>("StyleSheets/VideoPlayerStyle"));
        var buttonsVisualTree = Resources.Load<VisualTreeAsset>("UXMLs/VideoPlayerMain");
        buttonsVisualTree.CloneTree(root);
        InitVideoArea(root);
        InitPlaylistArea(root);
    }
    private void InitPlaylistArea(VisualElement root)
    {
        playlistController = new PlaylistController(root);
    }
    private void InitVideoArea(VisualElement root)
    {
        string path = "Assets/Editor/Resources/Prefabs/VideoPlayer.prefab";
        GameObject prefabObject = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(path));
        videoPlayer = prefabObject.GetComponent<VideoPlayer>();
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = InitVideoRendererTexture(root);
        videoPlayer.sendFrameReadyEvents = true;
        videoPlayer.frameReady += OnNewFrameReady;
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoController = new VideoController(videoPlayer, windowRoot);
    }
    private void InitSubscribers()
    {
        playlistController.PlayVideoAtUrl.AddListener(videoController.OnPlayVideoAtUrl);
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
        var videoPlayerImage = root.Query<Image>("lanscapeImage");
        RenderTexture targetRt = new RenderTexture(new RenderTextureDescriptor(1024, 1024));
        videoPlayerImage.First().image = targetRt;
        return targetRt;
    }
    private void OnDisable()
    {
        videoPlayer.Stop();
        videoPlayer.targetTexture.Release();
        DestroyImmediate((Object)videoPlayer.gameObject);
    }
}
