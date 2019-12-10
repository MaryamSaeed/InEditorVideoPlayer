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
    }

    //private Button CreatButton(string buttontext,float width,float height)
    //{
    //    var newButton = new Button() { text = buttontext };
    //    newButton.style.width = width;
    //    newButton.style.height = height;
    //    return newButton;
    //}
}
