using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OptionsData 
{
    public float volumePreference;
    public int qualiltyIndex;
    //public bool isFullScreen;

    public OptionsData(OptionsMenu options)
    {
        this.volumePreference = options.GetAudioMixerVolume();
        this.qualiltyIndex = options.GetQualityIndex();
        //this.isFullScreen = options.GetIsFullScreen();
    }
}
