using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private float startPos;
    private float endPos;
    //private float groundSize;
    private Transform trampoline;
    private Transform cue;
    float inputWindow;

    float correction = 0.09f;
    
    void Start()
    {
        trampoline = GetComponentInChildren<Transform>().Find("_TriggerZone").transform;
        cue = GetComponentInChildren<Transform>().Find("Cue").transform;
        inputWindow = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().inputWindowSeconds;

        startPos = transform.position.x;
        endPos = -startPos;
        Resize();

        StartCoroutine(Move());

        //groundSize = GameObject.Find("Ground Quad").GetComponent<Transform>().localScale.x;
    }

    void Update()
    {

    }

    IEnumerator Move()
    {
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime / Avatar.speed;
            transform.position = Vector3.Lerp(new Vector3(startPos, transform.position.y, transform.position.z), new Vector3(endPos, transform.position.y, transform.position.z), t);

            //Debug.Log(transform.position.x);

            if (transform.position.x < -24)
            {
                Destroy(gameObject);
            }
            yield return null;
        }

    }

    void Resize()
    {
        float oldWidth = trampoline.transform.localScale.x;
        float newWidth = ((startPos - endPos) / Avatar.baseSpeed * inputWindow) / 10
            + correction * (inputWindow - 1);

        trampoline.transform.localScale = new Vector3(newWidth, trampoline.transform.localScale.y);

        float move = trampoline.transform.localPosition.x + ((oldWidth - newWidth) * inputWindow);
        trampoline.transform.localPosition = new Vector3(move, trampoline.transform.localPosition.y);
        cue.transform.Translate(new Vector3(move / 2 + inputWindow, 0));
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Physics2D.IgnoreCollision(other, GetComponent<Collider2D>()); //no collision for player
        }
    }
}
