using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public struct VideoClipData
{
    public string Name;
    public string URL;
}
[CreateAssetMenu(fileName = "PlayLists", menuName = "PlayList", order = 1)]
public class MyScriptableObjectClass : ScriptableObject
{
    public string PlaylistTitle = "New MyScriptableObject";
    public List<VideoClipData> VideoClipList;
}