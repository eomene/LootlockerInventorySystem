using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cradaptive.MultipleTextureDownloadSystem;
using UnityEngine.UI;
using System;

namespace Cradaptive.MultipleTextureDownloadSystem.Example
{
    public class SimpleData : ICradaptiveTextureOwner
    {
        /// <summary>
        /// Url for image or local name in the Downloaded textures dictionary (if you have the image locally assigned)
        /// </summary>
        public string url => "";
        public Image preview { get; set; }
        public int downloadAttempts { get; set; }
        public Action<Sprite> OnTextureAvailable { get; set; }

        public Sprite texture2D;

    }

    public class CradaptiveTextureSaverExample : MonoBehaviour
    {

    }
}


