﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class SettingsMenuScript : MonoBehaviour {

    public AudioMixer audioMixer;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown graphicsDropdown;
    
    private Resolution[] resolutions;

    private void Start(){

        /*SETTING UP RESOLUTIONS*/
        resolutions = Screen.resolutions;
        List<string> options = new List<string>();
        int resolutionSelected = 0;
        for(int i = 0; i < resolutions.Length; i++){
            options.Add(resolutions[i].width+"x"+resolutions[i].height);
            resolutionDropdown.onValueChanged.AddListener(
                delegate { 
                    Screen.SetResolution(resolutions[resolutionDropdown.value].width, resolutions[resolutionDropdown.value].height, true);
                        });
            if(Screen.currentResolution.height == resolutions[i].height && Screen.currentResolution.width == resolutions[i].width)
                resolutionSelected = i;
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = resolutionSelected;
        /*SETTING UP GRAPHICS*/
        options.Clear();
        foreach(string quality in QualitySettings.names){
            options.Add(quality);
        }
        graphicsDropdown.AddOptions(options);
        graphicsDropdown.value = QualitySettings.GetQualityLevel();
    }

    public void SetVolume(float volume)
    {
        if(audioMixer != null)
            audioMixer.SetFloat("volume", Mathf.Lerp(-30, 20, volume));
        if(volume == 0)
        {
            AudioListener.pause = true;
        }
        else
        {
            AudioListener.pause = false;
        }
    }

    private void SetSFX(float volume)
    {
        // Does nothing currently
    }

    public void SetQuality(int qualityIndex)
    {
        Debug.Log(qualityIndex);
        QualitySettings.SetQualityLevel(qualityIndex);
    }
}
