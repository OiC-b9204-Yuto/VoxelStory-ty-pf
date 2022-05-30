using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public static Fade instance;

    [SerializeField] private float fadeTime = 2.0f;
    private float change;

    bool fade;

    [SerializeField]Image fadePanel;

    string sceneName;

    void Awake()
    {
        Cursor.visible = false;
        if (instance)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        //fadePanel = this.GetComponent<Image>();
    }

    void Start()
    {
        SceneManager.sceneLoaded += FadeIn;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.F10))
        {
            Cursor.visible = !Cursor.visible;
        }

        if (!fade)
        {
            return;
        }
        Color c = fadePanel.color;
        c.a += 1 / fadeTime * Time.deltaTime * change;
        if (c.a <= 0 && change < 0 || c.a >= 1 && change > 0)
        {
            c.a = (int)c.a;
            fade = false;
            if (sceneName != "")
            {
                Debug.Log("よみこみ" + c.a + sceneName);
                SceneManager.LoadScene(sceneName);
                sceneName = "";
            }
            else if (GameManager.instance)
            {
                GameManager.instance.GamaStart();
            }

        }
        fadePanel.color = c;
    }

    void FadeStart()
    {
        if (!fade)
        {
            fade = true;
            if (fadePanel.color.a > 0.5f)
            {
                change = -1;
            }
            else
            {
                change = 1;
            }
        }
    }

    void FadeIn(Scene scene,LoadSceneMode loadSceneMode)
    {
        FadeStart();
    }

    public void SceneLoad(string scene)
    {
        sceneName = scene;
        FadeStart();
    }


}
