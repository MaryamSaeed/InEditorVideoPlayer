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
        ApplyWindowStyle();
        InitVideoPlayerUtilities();
        InitSubscribers();
    }
    private void ApplyWindowStyle()
    {
        windowRoot = rootVisualElement;
        windowRoot.styleSheets.Add(Resources.Load<StyleSheet>("StyleSheets/VideoPlayerStyle"));
        var buttonsVisualTree = Resources.Load<VisualTreeAsset>("UXMLs/VideoPlayerMain");
        buttonsVisualTree.CloneTree(windowRoot);
    }
    private void InitVideoPlayerUtilities()
    {
        playlistController = new PlaylistController(windowRoot);
        InitVideoControlUtility();
    }
    private void InitVideoControlUtility()
    {
        string path = "Assets/Editor/Resources/Prefabs/VideoPlayer.prefab";
        GameObject prefabObject = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(path));
        videoPlayer = prefabObject.GetComponent<VideoPlayer>();
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = InitVideoRendererTexture(windowRoot);
        videoPlayer.sendFrameReadyEvents = true;
        videoPlayer.frameReady += OnNewFrameReady;
        videoController = new VideoController(videoPlayer, windowRoot);
    }
    private void InitSubscribers()
    {
        playlistController.PlayVideoAtUrl.AddListener(videoController.OnPlayVideoAtUrl);
        playlistController.PlaylistChanged.AddListener(videoController.OnPlaylistChanged);
    }
    private void OnNewFrameReady(VideoPlayer source, long frameIdx)
    {
        this.Repaint();
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
