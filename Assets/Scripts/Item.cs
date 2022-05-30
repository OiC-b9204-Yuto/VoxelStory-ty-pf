using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private int score = 1000;
    [SerializeField] private int health = 1;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<Player>().healthRecovery(health);
            GameManager.instance.AddScore(score);
            Destroy(this.gameObject);
        }
    }
}
