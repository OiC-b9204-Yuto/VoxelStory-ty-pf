using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class damageText : MonoBehaviour
{
   
    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        image.rectTransform.position = new Vector3(image.rectTransform.position.x + 3 / 3 * Time.deltaTime, image.rectTransform.position.y + 5 / 3 * Time.deltaTime, image.rectTransform.position.z);

        Color c = image.color;
        c.a -= 1.0f / 3 * Time.deltaTime;
        if (c.a == 0)
        {
            Destroy(this.gameObject);
        }
        image.color = c;
    }
}
