using System;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;
/// <summary>
/// Class that holds the logic behind the screen UI,
/// Buttons , Sliders and images
/// </summary>
public class VideoController
{
    //video player game object
    private VideoPlayer videoPlayer;
    // arefernce to the root visual element of the window
    private VisualElement windowRoot;
    //UI video scrub Bar/Progress Bar
    private Slider scrubBar;
    //UI volume slider
    private Slider volumeSlider;
    //UI mute toggle
    private Toggle muteToggle;
    //UI text for video time
    private Label videoTime;
    //UI text for seektime
    private Label seekTime;
    //UI text indecating video validity
    private Label videoValidity;
    //UI image holding the toggle
    private VisualElement toggelImage;
    //UI image with mute graphic
    private StyleBackground soundMute;
    //UI image with unmute graphic
    private StyleBackground soundUp;
    private TimeSpan timeSpan;

    //cot
    public VideoController(VideoPlayer activeplayer, VisualElement root)
    {
        videoPlayer = activeplayer;
        windowRoot = root;
        InitControllerButtons();
        InitScrubBarArea();
        InitVolumeControllers();
        videoValidity = windowRoot.Q<Label>("VideoValidity");
        videoValidity.visible = false;
        videoPlayer.frameReady += OnNewFrameReady;
        videoPlayer.prepareCompleted += OnVideoPrepared;
    }
    /// <summary>
    /// checks if the given url exists
    /// if so it plays the video
    /// if not displays the video not valid
    /// </summary>
    /// <param name="url">video path</param>
    public void OnPlayVideoAtUrl(string url)
    {
        if (File.Exists(url))
        {
            videoPlayer.url = url;
            videoPlayer.Prepare();
            videoValidity.visible = false;
        }
        else
        {
            videoValidity.visible = true;
        }
    }
    /// <summary>
    /// when the currently played playlist change
    /// stop the current video
    /// </summary>
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
    /// <summary>
    /// updates the seek bar with progress time
    /// and updates the seek time text
    /// </summary>
    /// <param name="value">progress time value</param>
    private void UpdateSeekTime(float value)
    {
        scrubBar.value = value;
        seekTime.text = Seconds2String(value);
    }
    /// <summary>
    /// stets seek time to 0
    /// sets video time to video length
    /// </summary>
    private void InitVideoTimeText()
    {
        videoTime.text = Seconds2String((float)videoPlayer.length);
        scrubBar.lowValue = 0;
    }
    /// <summary>
    /// linking UI buttons to their corresponding 
    /// functionalities 
    /// </summary>
    /// <param name="button">current button object</param>
    private void SetupControllerButton(Button button)
    {
        //query by class name
        VisualElement buttonIcon = button.Q(className: "videoplayer-button-icon");
        button.style.backgroundImage = null;
        button.text = string.Empty;
        ButtonSetupSwitch(button);
    }
    /// <summary>
    /// decides the button functionality
    /// based on its name in the structure
    /// </summary>
    /// <param name="button">button in setup</param>
    private void ButtonSetupSwitch(Button button)
    {
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
    /// <summary>
    /// initialize the UI controller buttons
    /// by query them from the root
    /// </summary>
    public void InitControllerButtons()
    {
        UQueryBuilder<Button> videoPlayerButtons = windowRoot.Query<Button>();
        videoPlayerButtons.ForEach(SetupControllerButton);
    }
    /// <summary>
    /// initializes Scrub bar area with 
    /// sliders and text labels
    /// </summary>
    private void InitScrubBarArea()
    {
        if (scrubBar == null)
            scrubBar = windowRoot.Query<Slider>("scrubBar");
        scrubBar.highValue = (float)videoPlayer.length;
        //regeisters the changes in scrub bar slider
        scrubBar.RegisterCallback<ChangeEvent<float>>(evt => OnChangeProgress(evt.newValue));
        if (videoTime == null)
            videoTime = windowRoot.Query<Label>("videoTime");
        if (seekTime == null)
            seekTime = windowRoot.Query<Label>("seekTime");
        InitVideoTimeText();
    }
    /// <summary>
    /// loads the necessary graphics
    /// for the mute button
    /// </summary>
    private void LoadMuteToggleImages()
    {
        //Reference to toggle checkmarck
        toggelImage = windowRoot.Q("unity-checkmark");
        //loading the mute texture from folder
        soundMute = new StyleBackground(Resources.Load<Texture2D>("Icons/Sound_mute"));
        //loading the unmute texture folder
        soundUp = new StyleBackground(Resources.Load<Texture2D>("Icons/Sound_Up"));
    }
    /// <summary>
    /// initializes the sound control buttons and sliders
    /// </summary>
    private void InitVolumeControllers()
    {
        LoadMuteToggleImages();
        muteToggle = windowRoot.Query<Toggle>("muteToggle");
        // rgisters the mute action with toggle
        muteToggle.RegisterCallback<ChangeEvent<bool>>(evt => OnMuteVolume(evt.newValue));
        volumeSlider = windowRoot.Query<Slider>("volumeSlider");
        volumeSlider.value = 1;
        videoPlayer.SetDirectAudioVolume(0, 1);
        //registers sound level change with slider value change action
        volumeSlider.RegisterCallback<ChangeEvent<float>>(evt => OnChangeVolumeLevel(evt.newValue));
    }
    /// <summary>
    /// handles mute action
    /// </summary>
    /// <param name="ison">toggle status</param>
    private void OnMuteVolume(bool ison)
    {
        videoPlayer.SetDirectAudioMute(0, ison);
        volumeSlider.SetEnabled(!ison);
        if (ison)
        {
            toggelImage.style.backgroundImage = soundMute;
        }
        else
        {
            toggelImage.style.backgroundImage = soundUp;
        }
    }
    /// <summary>
    /// handles volume level change action
    /// </summary>
    /// <param name="value">the slider value</param>
    private void OnChangeVolumeLevel(float value)
    {
        videoPlayer.SetDirectAudioVolume(0, value);
    }
    /// <summary>
    /// handles the scrub bar value change
    /// </summary>
    /// <param name="value">the slider value</param>
    private void OnChangeProgress(float value)
    {
        if (videoPlayer.canSetTime)
            videoPlayer.time = value;
        UpdateSeekTime(value);
    }
    /// <summary>
    /// drows second in the format hh\:mm\:ss
    /// </summary>
    /// <param name="value">time in seconds</param>
    /// <returns>string indecating the time</returns>
    private string Seconds2String(float value)
    {
        timeSpan = TimeSpan.FromSeconds(value);
        return timeSpan.ToString(@"hh\:mm\:ss");
    }
    /// <summary>
    /// handles the action of video preparations and plays the video
    /// </summary>
    /// <param name="source"></param>
    private void OnVideoPrepared(VideoPlayer source)
    {
        InitScrubBarArea();
        PlayVideo();
    }
    /// <summary>
    /// handles the frame ready action by updating seek time text
    /// </summary>
    /// <param name="source">video player object on the scene</param>
    /// <param name="frameIdx">the index of the current frame</param>
    private void OnNewFrameReady(VideoPlayer source, long frameIdx)
    {
        UpdateSeekTime((float)source.time);
    }
}