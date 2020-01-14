using UnityEngine.UIElements;
using UnityEngine.Video;
using System;

public class VideoController
{
    private VideoPlayer videoPlayer;
    private VisualElement windowRoot;
    private Slider scrubBar;
    private Slider volumeSlider;
    private Toggle muteToggle;
    private Label videoTime;
    private Label seekTime;
    private TimeSpan timeSpan;
    public VideoController(VideoPlayer activeplayer,VisualElement root)
    {
        videoPlayer = activeplayer;
        windowRoot = root;
        InitControllerButton();
        InitScrubBar();
        InitVolumeControllers();
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
        var buttonIcon = button.Q(className:"videoplayer-button-icon");
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
    private void InitScrubBar()
    {
        scrubBar = windowRoot.Query<Slider>("scrubBar");
        scrubBar.highValue = (float)videoPlayer.length;
        scrubBar.RegisterCallback<ChangeEvent<float>>(evt => OnChangeProgress(evt.newValue));
        videoTime = windowRoot.Query<Label>("videoTime");
        seekTime = windowRoot.Query<Label>("seekTime");
        InitVideoTimeText();
    }
    private void InitVolumeControllers()
    { 
        muteToggle = windowRoot.Query<Toggle>("muteToggle");
        muteToggle.RegisterCallback<ChangeEvent<bool>>(evt => OnMuteVolume(evt.newValue));
        volumeSlider = windowRoot.Query<Slider>("volumeSlider");
        volumeSlider.value = 1;
        videoPlayer.SetDirectAudioVolume(0, 1);
        volumeSlider.RegisterCallback<ChangeEvent<float>>(evt => OnChangeVolumeLevel(evt.newValue));
    }
    private void OnMuteVolume(bool ison)
    {
        videoPlayer.SetDirectAudioMute(0, ison);
        volumeSlider.SetEnabled(!ison);
    }
    private void OnChangeVolumeLevel(float value)
    {
        videoPlayer.SetDirectAudioVolume(0, value);
    }
    private void OnChangeProgress(float value)
    {
        if (videoPlayer.canSetTime)
            videoPlayer.time = value;
        UpdateSeekTime(value);
    }
    private string Seconds2String(float value)
    {
        timeSpan = TimeSpan.FromSeconds(value);
        return timeSpan.ToString(@"hh\:mm\:ss");
    }
    private void OnVideoPrepared(VideoPlayer source)
    {
        InitScrubBar();
        PlayVideo();
    }
    private void OnNewFrameReady(VideoPlayer source, long frameIdx)
    {
        UpdateSeekTime((float)source.time);
    }
    public void OnPlayVideoAtUrl(string url)
    {
        videoPlayer.url = url;
    }
}
