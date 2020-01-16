using UnityEngine.UIElements;
using UnityEngine.Video;
using System;
using UnityEngine;
using System.IO;

public class VideoController
{
    private VideoPlayer videoPlayer;
    private VisualElement windowRoot;
    private Slider scrubBar;
    private Slider volumeSlider;
    private Toggle muteToggle;
    private Label videoTime;
    private Label seekTime;
    private Label videoValidity;
    private VisualElement toggelImage;
    private StyleBackground soundMute;
    private StyleBackground soundUp;
    private TimeSpan timeSpan;

    public VideoController(VideoPlayer activeplayer, VisualElement root)
    {
        videoPlayer = activeplayer;
        windowRoot = root;
        InitControllerButtons();
        InitScrubBar();
        InitVolumeControllers();
        videoValidity = windowRoot.Q<Label>("VideoValidity");
        videoValidity.visible = false;
        videoPlayer.frameReady += OnNewFrameReady;
        videoPlayer.prepareCompleted += OnVideoPrepared;
    }
    public void OnPlayVideoAtUrl(string url)
    {
        if (File.Exists(url))
        {
            videoPlayer.url = url;
            videoPlayer.Prepare();
            videoValidity.visible = false;
        }
        else
            videoValidity.visible = true;
    }
    public void OnPlaylistChanged()
    {
        StopVideo();
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
    private void UpdateSeekTime(float value)
    {
        scrubBar.value = (float)value;
        seekTime.text = Seconds2String(value);
    }
    private void InitVideoTimeText()
    {
        videoTime.text = Seconds2String((float)videoPlayer.length);
        scrubBar.lowValue = 0;
    }
    private void SetupControllerButton(Button button)
    {
        var buttonIcon = button.Q(className: "videoplayer-button-icon");
        button.style.backgroundImage = null;
        button.text = string.Empty;
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
        }
    }
    public void InitControllerButtons()
    {
        var videoPlayerButtons = windowRoot.Query<Button>();
        videoPlayerButtons.ForEach(SetupControllerButton);
    }
    private void InitScrubBar()
    {
        if (scrubBar == null)
            scrubBar = windowRoot.Query<Slider>("scrubBar");
        scrubBar.highValue = (float)videoPlayer.length;
        scrubBar.RegisterCallback<ChangeEvent<float>>(evt => OnChangeProgress(evt.newValue));
        if (videoTime == null)
            videoTime = windowRoot.Query<Label>("videoTime");
        if (seekTime == null)
            seekTime = windowRoot.Query<Label>("seekTime");
        InitVideoTimeText();
    }
    private void LoadMuteToggleImages()
    {
        //Reference to toggle checkmarck
        toggelImage = windowRoot.Q("unity-checkmark");
        soundMute = new StyleBackground(Resources.Load<Texture2D>("Icons/Sound_mute"));
        soundUp = new StyleBackground(Resources.Load<Texture2D>("Icons/Sound_Up"));
    }
    private void InitVolumeControllers()
    {
        LoadMuteToggleImages();
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
        if (ison)
            toggelImage.style.backgroundImage = soundMute;
        else
            toggelImage.style.backgroundImage = soundUp;
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
}
