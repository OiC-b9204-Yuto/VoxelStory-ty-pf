using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroup : MonoBehaviour
{
    [SerializeField] private GameObject actionObject;

    [SerializeField] private Enemy[] enemies;

    [SerializeField] private bool posCorrection = false;


    private bool flag = false;

    void Start()
    {

    }

    void Update()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            flag = true;
            if (enemies[i])
            {
                flag = false;
            }
        }

        if (flag)
        {
            actionObject.SetActive(!actionObject.activeSelf);
            Destroy(this.gameObject);
        }
    }
}
