using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchLineBox : MonoBehaviour
{
    Image render;
    public float showTime;
    public float originAlpha;
    float timer = 0.01f;

    void Start()
    {
        render = GetComponent<Image>();
        render.color = new Color(1, 1, 1, originAlpha);
    }

    // Update is called once per frame
    void Update()
    {
        render.color = new Color(1, 1, 1, (showTime - timer) / showTime * originAlpha);
        timer += Time.deltaTime;
        if(timer > showTime) { Destroy(gameObject); }
    }
}
