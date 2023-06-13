using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchLineBox : MonoBehaviour
{
    Image render;
    public float showTime = 0.2f;
    public float originAlpha = 0.5f;
    float timer = 0.01f;

    void Start()
    {
        render = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > showTime) { Destroy(gameObject); }
        render.color = new Color(1, 1, 1, (showTime - timer) / showTime);
    }
}
