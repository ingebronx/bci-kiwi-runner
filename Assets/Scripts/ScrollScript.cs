using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollScript : MonoBehaviour
{
    [Range(0.0f,1.0f)]
    public float speedMult;

    //float t = 0;
    float offset = 0;
    bool scrolling;

    private void Start()
    {
        //StartCoroutine(Scroll());
    }

    void Update()
    {
        if (Avatar.start && !scrolling)
        {
            StartCoroutine(Scroll());
            scrolling = true;
        }
    }

    IEnumerator Scroll()
    {
        float t = 0;

        while (Avatar.start)
        {
            t += Time.deltaTime / Avatar.speed;
            offset = t * speedMult * 1.3f;
            GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offset, 0);

            yield return null;
        }
    }
}