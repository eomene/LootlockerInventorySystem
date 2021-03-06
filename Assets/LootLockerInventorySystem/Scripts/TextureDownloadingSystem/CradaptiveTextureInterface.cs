using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cradaptive.MultipleTextureDownloadSystem
{
    public interface ICradaptiveTextureOwner
    {
        /// <summary>
        /// This is the path to the texture, It can be local if it is already assigned in the gameobject that owns the textue downloader
        /// </summary>
        string url { get; }
        /// <summary>
        /// Fired as soon as we get a texture from the url
        /// </summary>
        Action<Sprite> OnTextureAvailable { set; get; }
    }
}