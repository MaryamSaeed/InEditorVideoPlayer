using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.Video;
/// <summary>
/// the window class which initilize and terminates the video Player UI and Logic
/// </summary>
public class VideoPlayerWindow : EditorWindow
{
    //video player game object
    private VideoPlayer videoPlayer;
    // controls video through UI
    private VideoController videoController;
    //controls the Playlist through UI
    private PlaylistController playlistController;
    //root element of the window visual tree
    private VisualElement windowRoot;
    /// <summary>
    ///show the videoplayer window Upon Selecting 
    ///the UI menu item Video\VideoPlayer or by Pressing Ctrl+K
    /// </summary>
    [MenuItem("Video/VideoPlayerWindow _%k")]
    public static void ShowVideoPlayerWindow()
    {
        //reference to the window object
        var window = GetWindow<VideoPlayerWindow>();
        //setting the window Object Title
        window.titleContent = new GUIContent("Unity Video Player");
    }
    /// <summary>
    /// called when a window 
    /// Object is enabled
    /// </summary>
    private void OnEnable()
    {
        // arefernce to the root visual element of the window
        windowRoot = rootVisualElement;
        ApplyWindowStyle();
        InitVideoPlayerUtilities();
        RegisterSubscriptions();
    }
    /// <summary>
    ///adding Window visualtree structure
    ///and stylesheets
    /// </summary>
    private void ApplyWindowStyle()
    {
        windowRoot.styleSheets.Add(Resources.Load<StyleSheet>("StyleSheets/VideoPlayerStyle"));
        var buttonsVisualTree = Resources.Load<VisualTreeAsset>("UXMLs/VideoPlayerMain");
        buttonsVisualTree.CloneTree(windowRoot);
    }
    /// <summary>
    /// creates the required utilities for 
    /// the videoplayer window
    /// </summary>
    private void InitVideoPlayerUtilities()
    {
        playlistController = new PlaylistController(windowRoot);
        InitVideoControlUtility();
    }
    /// <summary>
    /// inistanitaes a new video player prefab in the scene
    /// adds the required reneder textures and creates a new 
    /// video Controller.
    /// </summary>
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
    /// <summary>
    /// makes the necessary sunscriptions
    /// </summary>
    private void RegisterSubscriptions()
    {
        // notify the video Controller with the new video's path
        playlistController.PlayVideoAtPath.AddListener(videoController.OnPlayVideoAtUrl);
        //notify the video controller when the playlist change
        playlistController.PlaylistChanged.AddListener(videoController.OnPlaylistChanged);
    }
    /// <summary>
    /// unscbscribe the registered events
    /// </summary>
    private void UnRegisterSubscriptions()
    {
        playlistController.PlayVideoAtPath.RemoveAllListeners();
        playlistController.PlaylistChanged.RemoveAllListeners();
    }
    /// <summary>
    /// repaints the window with new frame when ready
    /// </summary>
    /// <param name="source">video player object on the scene</param>
    /// <param name="frameIdx">the index of the current frame</param>
    private void OnNewFrameReady(VideoPlayer source, long frameIdx)
    {
        this.Repaint();
    }
    /// <summary>
    /// initilizing the render texture 
    /// used for video rendering
    /// </summary>
    /// <param name="root"> widow root Visual element</param>
    /// <returns> video redering Render texture</returns>
    private RenderTexture InitVideoRendererTexture(VisualElement root)
    {
        var videoPlayerImage = root.Query<Image>("lanscapeImage");
        RenderTexture targetRt = new RenderTexture(new RenderTextureDescriptor(1024, 1024));
        videoPlayerImage.First().image = targetRt;
        return targetRt;
    }
    /// <summary>
    /// called on closing the window
    /// </summary>
    private void OnDisable()
    {
        // stop the currently running Video
        videoPlayer.Stop();
        // release the target texture memory
        videoPlayer.targetTexture.Release();
        // remove the old subscriptions
        UnRegisterSubscriptions();
        // Destroy the video player Gameobject
        DestroyImmediate((Object)videoPlayer.gameObject);
    }
}