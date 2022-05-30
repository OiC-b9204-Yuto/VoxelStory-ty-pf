using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SceneLoad : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;

    [SerializeField] private Dropdown resolution;
    [SerializeField] private Toggle fullScreen;
    [SerializeField] private Toggle window;
    [SerializeField] private Toggle fullScreenWindow;

    [SerializeField] private AudioMixer audioMixer;

    List<string> res = new List<string>();

    int index;

    private void Start()
    {
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            res.Add(Screen.resolutions[i].ToString());

            if (Screen.resolutions[i].width == Screen.currentResolution.width &&
                Screen.resolutions[i].height == Screen.currentResolution.height &&
                Screen.resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
            {
                index = i;
            }
        }
        
        resolution.AddOptions(res);
        resolution.value = index;
        Debug.Log(Screen.fullScreenMode);
        switch (Screen.fullScreenMode)
        {
            case FullScreenMode.ExclusiveFullScreen:
                fullScreen.isOn = true;
                break;
            case FullScreenMode.FullScreenWindow:
                
                break;
            case FullScreenMode.MaximizedWindow:
                fullScreenWindow.isOn = true;
                break;
            case FullScreenMode.Windowed:
                window.isOn = true;
                break;
            default:
                break;
        }
    }

    public void Load(string name)
    {
        Fade.instance.SceneLoad(name);
    }

    public void GameExit()
    {
        Application.Quit();
    }

    public void OptionSave()
    {
        OptionManager.instance.Save((int)bgmSlider.value, (int)seSlider.value);
        audioMixer.SetFloat("BGM", (int)bgmSlider.value);
        audioMixer.SetFloat("SE", (int)seSlider.value);
        FullScreenMode fullScreenMode = FullScreenMode.Windowed;
        if (fullScreen.isOn)
        {
            fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        else if (fullScreenWindow.isOn)
        {
            fullScreenMode = FullScreenMode.MaximizedWindow;
        }
        Screen.SetResolution(int.Parse(res[resolution.value].Split(' ')[0]), int.Parse(res[resolution.value].Split(' ')[2]), fullScreenMode);
    }

    public void OptionLoad()
    {
        Option option = OptionManager.instance.Load().Result;
        bgmSlider.value = option.bgmVolume;
        seSlider.value = option.seVolume;
        var res = Screen.resolutions;

        switch (Screen.fullScreenMode)
        {
            case FullScreenMode.ExclusiveFullScreen:
                fullScreen.isOn = true;
                break;
            case FullScreenMode.FullScreenWindow:
                break;
            case FullScreenMode.MaximizedWindow:
                fullScreenWindow.isOn = true;
                break;
            case FullScreenMode.Windowed:
                window.isOn = true;
                break;
            default:
                if (Screen.fullScreen)
                {
                    fullScreen.isOn = true;
                }
                else
                {
                    window.isOn = true;
                }
                break;
        }
    }

}
