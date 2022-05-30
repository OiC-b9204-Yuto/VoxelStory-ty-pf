using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeFade : MonoBehaviour
{
    [SerializeField] private float alpha;

    private int change = 1;

    Renderer renderer;

    void Start()
    {
        renderer = this.GetComponent<Renderer>();
    }

    void Update()
    {
        Color c = renderer.material.color;
        alpha += 1.0f / 1 * Time.deltaTime * change;
        c.a = alpha;

        if(alpha >= 1)
        {
            c.a = 1;
            change = -1;
        }
        else if(alpha <= 0)
        {
            c.a = 0;
            change = 1;
        }

        renderer.material.color = c;
    }
}
