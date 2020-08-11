using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBabies : MonoBehaviour
{
    private Rigidbody2D rb;
    private float groundSize;
    private Animator anim;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        groundSize = GameObject.Find("Ground Quad").GetComponent<Transform>().localScale.x;
        anim = gameObject.GetComponent<Animator>();
        Avatar.endgame = true;

        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        float t = 0;
        float startPos = transform.position.x;
        float endPos = -startPos;

        while (t < 1)
        {
            if(transform.position.x > 0)
            {

                t += Time.deltaTime / Avatar.baseSpeed;
                transform.position = Vector3.Lerp(new Vector3(startPos, transform.position.y, transform.position.z),
                    new Vector3(endPos, transform.position.y, transform.position.z), t);
            }

            yield return null;
        }

    }

    void Update()
    {
        //if (Avatar.start)
        //{
        //    rb.transform.Translate(Avatar.speed * groundSize, 0, 0);
        //}
        
        //if (gameObject.GetComponentInChildren<Transform>().Find("OnScreen").GetComponent<SpriteRenderer>().isVisible)
        if(transform.position.x <= 0)
        {
            Avatar.start = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
            anim.enabled = true;
    }
}