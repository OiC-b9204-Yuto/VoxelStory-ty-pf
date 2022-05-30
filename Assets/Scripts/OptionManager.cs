using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

public class OptionManager : MonoBehaviour
{
    //インスタンス
    public static OptionManager instance;
    //オプションの保存先
    string filePath;

    [SerializeField] private AudioMixer audioMixer;

    Option option = new Option();

    void Awake()
    {
        //一つしか存在しないようにする
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        filePath = Application.dataPath + @"\options.data";
        option = Load().Result;
        audioMixer.SetFloat("BGM", option.bgmVolume);
        audioMixer.SetFloat("SE", option.seVolume);
        DontDestroyOnLoad(this);
    }

    public async Task Save(int bgm,int se)
    {
        option.bgmVolume = bgm;
        option.seVolume = se;

        string jsonStr = JsonUtility.ToJson(option);

        using (var sw = new StreamWriter(filePath))
        {
            await sw.WriteAsync(jsonStr).ConfigureAwait(false);
        }
    }

    public async Task<Option> Load()
    {
        using (var sr = new StreamReader(filePath))
        {
            string jsonStr = await sr.ReadToEndAsync().ConfigureAwait(false);
            option = JsonUtility.FromJson<Option>(jsonStr);
        }

        return option;
    }

}

[Serializable]
public class Option
{
    public int bgmVolume;
    public int seVolume;
}
