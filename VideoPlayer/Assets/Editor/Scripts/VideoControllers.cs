using UnityEngine.UIElements;
using UnityEngine.Video;
using System;

public class VideoController
{
    private VideoPlayer videoPlayer;
    private VisualElement windowRoot;
    private Slider scrubBar;
    private Label videoTime;
    private Label seekTime;
    private TimeSpan timeSpan;
    public VideoController(VideoPlayer activeplayer,VisualElement root)
    {
        videoPlayer = activeplayer;
        windowRoot = root;
        InitControllerButton();
        InitControllerSlider();
        videoPlayer.frameReady += OnNewFrameReady;
        videoPlayer.prepareCompleted += OnVideoPrepared;
    }
    public void UpdateSeekTime(float value)
    {
        scrubBar.value = (float)value;
        seekTime.text = Seconds2String(value);
    }
    public void InitControllerButton()
    {
        var videoPlayerButtons = windowRoot.Query<Button>();
        videoPlayerButtons.ForEach(SetupControllerButtons);
    }
    private void PlayVideo()
    {
        videoPlayer.Play();
    }
    private void PauseVideo()
    {
        videoPlayer.Pause();
    }
    private void StopVideo()
    {
        videoPlayer.Stop();
    }
    private void FastForwardVideo()
    {
        if (videoPlayer.canStep)
            videoPlayer.StepForward();
    }
    private void InitVideoTimeText()
    {
        videoTime.text = Seconds2String((float)videoPlayer.length);
        scrubBar.lowValue = 0;
    }
    private void SetupControllerButtons(Button button)
    {
        var buttonIcon = button.Q(className: "videoplayer-button-icon");
        button.text = button.parent.name;
        switch (button.parent.name)
        {
            case "Play":
                button.clickable.clicked += () => PlayVideo();
                break;
            case "Pause":
                button.clickable.clicked += () => PauseVideo();
                break;
            case "Stop":
                button.clickable.clicked += () => StopVideo();
                break;
            case "Next":
                button.clickable.clicked += () => FastForwardVideo();
                break;
        }
    }
    private void InitControllerSlider()
    {
        scrubBar = windowRoot.Query<Slider>("scrubBar");
        scrubBar.highValue = (float)videoPlayer.length;
        scrubBar.RegisterCallback<ChangeEvent<float>>(evt => OnChangeEvent(evt.newValue));
        videoTime = windowRoot.Query<Label>("videoTime");
        seekTime = windowRoot.Query<Label>("seekTime");
        InitVideoTimeText();
    }
    private void OnChangeEvent(float value)
    {
        UpdateSeekTime(value);
    }
    private string Seconds2String(float value)
    {
        timeSpan = TimeSpan.FromSeconds(value);
        return timeSpan.ToString(@"hh\:mm\:ss");
    }
    private void OnVideoPrepared(VideoPlayer source)
    {
        InitControllerSlider();
    }
    private void OnNewFrameReady(VideoPlayer source, long frameIdx)
    {
        UpdateSeekTime((float)source.time);
    }
}
