using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoControllers
{
    private VideoPlayer videoPlayer;
    public VideoControllers(VideoPlayer activeplayer)
    {
        videoPlayer = activeplayer;
    }
    public void PlayVideo()
    {
        videoPlayer.Play();
    }
    public void PauseVideo()
    {
        videoPlayer.Pause();
    }
    public void StopVideo()
    {
        videoPlayer.Stop();
    }
    public void FastForwardVideo()
    {
        if(videoPlayer.canStep)
            videoPlayer.StepForward();
    }
   
}
