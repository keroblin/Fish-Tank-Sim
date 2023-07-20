using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    public Slider volumeSlider;
    public AudioMixer mixer;

    float vol;

    private void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("volume");
        vol = PlayerPrefs.GetFloat("volume");
        mixer.SetFloat("MasterVolume", Mathf.Log10(vol) * 20);
    }
    public void SaveVolume()
    {
        vol = volumeSlider.value;
        PlayerPrefs.SetFloat("volume", vol);
        mixer.SetFloat("MasterVolume", Mathf.Log10(vol) * 20);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
