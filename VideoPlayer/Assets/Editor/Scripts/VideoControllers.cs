using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;

public class VideoControllers
{
    private VideoPlayer videoPlayer;
    public VideoControllers(VideoPlayer activeplayer)
    {
        videoPlayer = activeplayer;
    }
    private void PlayVideo()
    {
        videoPlayer.Play();
    }
    private  void PauseVideo()
    {
        videoPlayer.Pause();
    }
    private  void StopVideo()
    {
        videoPlayer.Stop();
    }
    private  void FastForwardVideo()
    {
        if(videoPlayer.canStep)
            videoPlayer.StepForward();
    }
    public void InitControllerButton(VisualElement root)
    {
        var videoPlayerButtons = root.Query<Button>();
        videoPlayerButtons.ForEach(SetupControllerButtons);
    }
    private void SetupControllerButtons(Button button)
    {
        var buttonIcon = button.Q(className: "videoplayer-button-icon");
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

}
