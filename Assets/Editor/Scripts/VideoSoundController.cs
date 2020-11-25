using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;
/// <summary>
/// Class that Controld the video player sound
/// </summary>
public class VideoSoundController
{
    //UI volume slider
    private Slider volumeSlider;
    //UI mute toggle
    private Toggle muteToggle;
    //UI image holding the toggle
    private VisualElement toggelImage;
    //UI image with mute graphic
    private StyleBackground soundMute;
    //UI image with unmute graphic
    private StyleBackground soundUp;
    //video player game object
    private VideoPlayer videoPlayer;

    //cotr
    public VideoSoundController(VideoPlayer activeplayer, VisualElement root)
    {
        videoPlayer = activeplayer;
        InitVolumeControllers(root,activeplayer);
    }   
    /// <summary>
    /// loads the necessary graphics
    /// for the mute button
    /// </summary>
    private void LoadMuteToggleImages(VisualElement windowRoot)
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
    private void InitVolumeControllers(VisualElement windowRoot, VideoPlayer videoPlayer)
    {
        LoadMuteToggleImages(windowRoot);
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
            toggelImage.style.backgroundImage = soundMute;
        else
            toggelImage.style.backgroundImage = soundUp;
    }
    /// <summary>
    /// handles volume level change action
    /// </summary>
    /// <param name="value">the slider value</param>
    private void OnChangeVolumeLevel(float value)
    {
        videoPlayer.SetDirectAudioVolume(0, value);
    }
}
