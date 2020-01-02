using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public struct VideoClipData
{
    public string Name;
    public string URL;
}
[CreateAssetMenu(fileName = "NewPlayList", menuName = "PlayList", order = 1)]
[Serializable]
public class PlaylistAsset : ScriptableObject
{
    public string PlaylistTitle = "Playlist title";
    public List<VideoClipData> VideoClipList;
   
}