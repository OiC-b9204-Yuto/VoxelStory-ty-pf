using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class RankingManager : MonoBehaviour
{
    //インスタンス
    public static RankingManager instance;
    //オプションの保存先
    string filePath;

    Ranking ranking;

    void Awake()
    {
        //一つしか存在しないようにする
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        filePath = Application.dataPath + @"\rank.data";
    }

    public async Task Save(int bgm, int se)
    {

        string jsonStr = JsonUtility.ToJson(ranking);

        using (var sw = new StreamWriter(filePath))
        {
            await sw.WriteAsync(jsonStr).ConfigureAwait(false);
        }
    }

    public async Task Load()
    {
        using (var sr = new StreamReader(filePath))
        {
            string jsonStr = await sr.ReadToEndAsync().ConfigureAwait(false);
            ranking = JsonUtility.FromJson<Ranking>(jsonStr);
        }
    }

    public Ranking GetRanking()
    {
        return ranking;
    }

}

[Serializable]
public class Ranking
{
    public string name;
    public string time;
    public int score;
}

