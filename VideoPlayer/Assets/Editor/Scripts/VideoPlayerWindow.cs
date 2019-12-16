using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.Video;

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
    private void InitVideoArea(VisualElement root)
    {
        var videoVisualTree = new VisualTreeAsset();
        videoVisualTree.CloneTree(root);
        var videoPlayerComponent = new VideoPlayer();
        videoPlayerComponent.url = "";
    }
}
