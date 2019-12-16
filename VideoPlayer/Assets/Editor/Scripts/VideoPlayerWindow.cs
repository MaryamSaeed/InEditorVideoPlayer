using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

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
        var buttonsVisualTree = Resources.Load<VisualTreeAsset>("VideoPlayerMain");
        buttonsVisualTree.CloneTree(root);
        var videoPlayerButtons = root.Query<Button>();
        videoPlayerButtons.ForEach(SetupButton);
    }
    private void SetupButton(Button button)
    {
        var buttonIcon = button.Q(className: "videoplayer-button-icon");
        button.text = button.parent.name;
        button.clickable.clicked += () => Debug.Log("button " + button.parent.name);
    }
}
