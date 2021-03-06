﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//herit from the lognotifier script for the notify function (logging to csv)
public class Avatar : Lognotifier
{
    private Rigidbody2D rb;
    private Animator anim;

    private AudioSource music;
    private AudioSource windSound;
    private AudioSource birdCall;
    private AudioSource kiwiSquawk;
    private AudioSource feedback;

    private bool inTask; //checks if player is within task phase

    private float jumpForce;

    public SpriteRenderer signal;
    public SpriteRenderer prep;
    public SpriteRenderer exclamation;

    public bool blinkeye = false;
    public float Distance_;

    private bool end;

    //public static bool doneDidDoIt;
    //public static bool jumping;
    public static string zone = " ";

    public GameObject babies;
    public GameObject heart;
    public GameObject kiwi;
    private Transform Trampoline;

    private bool playSquawk = true;

    bool sham;
    private bool canStart;
    public static bool start = false;
    public static bool endgame;
    public static float speed; //current speed of environment moving
    public static float baseSpeed = 13f; //base speed (amt of seconds for obstacles to move from start to end point)

    [Range(0, 2)]
    public int condition;

    //private int[] cond0 = new int[20] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }; //0
    //private int[] cond1 = new int[20] { 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }; //1
    //private int[] cond2 = new int[20] { 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }; //2
    //private int[] demo = new int[10] { 0, 0, 2, 2, 2, 1, 1, 1, 1, 1 };
    //private int[] input;

    float timer;
    bool timing;

    public GameManager gm;
    
    void Start()
    {
        zone = "prestart";
        speed = baseSpeed; //set speed to base speed at first

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        jumpForce = 400f;

        AudioSource[] soundEffects = GetComponents<AudioSource>();
        windSound = soundEffects[0];
        birdCall = soundEffects[1];
        music = soundEffects[2];
        kiwiSquawk = soundEffects[3];
        feedback = soundEffects[4];

        Invoke("OhNo", 2.0f);
        Invoke("StartGame", 4.0f);
    }

    void Update()
    {
        //if ((Input.GetKeyDown(KeyCode.Space) || !BlinkDetector.blinked) && !blinkeye  && !start && canStart)
        if (Input.GetKeyDown(KeyCode.Space) && !start && canStart)
        {
            StartCoroutine(blinkdelay());

            gm.RunGame();

            zone = "ground";

            start = true;
            exclamation.enabled = false;

            Show(babies, false, false);

            anim.Play("kiwiRun");
            music.Play();

            //signal.enabled = false;

            canStart = false;
        }

        if (timing)
            timer += Time.deltaTime;

        if (!end && endgame)
        {
            if (playSquawk)
            {
                kiwiSquawk.Play();
                playSquawk = false;
            }
            gm.EndGame();
            exclamation.enabled = true;
            StartCoroutine(GameEnd());
            //rb.transform.Translate((-speed / 1.5f) * GameObject.Find("Ground Quad").GetComponent<Transform>().localScale.x, 0, 0);
        }
    }

    IEnumerator GameEnd()
    {
        float t = 0;
        float startPos = transform.position.x;

        while (t < 1)
        {
            if (!start)
            {
                speed = baseSpeed / 4;
            }

            t += Time.deltaTime / speed;

            if (!end)
            {
                transform.position = Vector3.Lerp(new Vector3(startPos, transform.position.y, transform.position.z),
                    new Vector3(0, transform.position.y, transform.position.z), t);
            }

            yield return null;
        }
    }

    IEnumerator blinkdelay()
    {
        blinkeye = true;
        yield return new WaitForSeconds(0.3f);
        blinkeye = false;
    }

    void StartGame()
    {
        //signal.enabled = true;
        canStart = true;
    }

    void OhNo()
    {
        exclamation.enabled = true;
        Show(babies, true, true);
        kiwiSquawk.Play();
    }
    
    //this function add the current trial and the trampolineoffset each time the notify function is called 
    protected override void notify(Dictionary<string, List<string>> dico)
    {
        dico.Add("KiwiTrampolineOffset", new List<string>());
        dico.Add("Current Trial", new List<string>());
        GameObject temp = GameObject.Find("_TriggerZone");
        
        if (temp)
        {
            Trampoline = temp.GetComponent<Transform>();
            Distance_ = Vector2.Distance(kiwi.transform.position, Trampoline.position);
            string strdistance = Distance_.ToString();
            dico["KiwiTrampolineOffset"].Add(strdistance);
            dico["Current Trial"].Add(SpawnObstacles.trials.ToString());
            base.notify(dico);
        }
    }

    public void onGameDecision(GameDecisionData decisionData)
    {
        if (decisionData.decision == InputTypes.AcceptAllInput)
        {
            Jump();
            Debug.Log("Showing Feedback from Real Input.");
        }
        else if (decisionData.decision == InputTypes.FabInput)
        {
            Jump();
            Debug.Log("Showing Feedback from Fabricated Input.");
        }
        else
        {
            Debug.Log("No Feedback.");
        }
    }

    private int[] Shuffle(int[] myArray)
    {
        System.Random r = new System.Random();
        myArray = myArray.OrderBy(x => r.Next()).ToArray();

        string arr = "";
        foreach (int i in myArray)
            arr += i;
        
        return myArray;
    }

    private void Jump() //when jumping, play feedback
    {
        Dictionary<string, List<string>> jumplogCollection = new Dictionary<string, List<string>>();
        jumplogCollection.Add("Event", new List<string>());
        jumplogCollection["Event"].Add("KiwiJumped");
        notify(jumplogCollection);

        //jumping = true;

        rb.AddForce(Vector2.up * jumpForce);
        feedback.Play();
        anim.Play("kiwiJump");

        sham = false;
    }

    private void Show(GameObject toShow, bool renderer, bool animator)
    {
        toShow.GetComponent<SpriteRenderer>().enabled = renderer;
        toShow.GetComponent<Animator>().enabled = animator;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Dictionary<string, List<string>> signallogCollection = new Dictionary<string, List<string>>();
        signallogCollection.Add("Event", new List<string>());
        switch (other.tag)
        {
            case "Cue":
                //prep.enabled = true;
                zone = "cue";
                notify(new Dictionary<string, List<string>>() { { "Event", new List<string> { "TrialStart" } } });
                signallogCollection["Event"].Add("DontBlinkSignal");
                notify(signallogCollection);
                break;

            case "TriggerZone":
                timer = 0;
                timing = true;
                //prep.enabled = false;
                //signal.enabled = true;
                signallogCollection["Event"].Add("BlinkSignal");
                notify(signallogCollection);
                zone = "task";

                gm.OpenInputWindow();
                Debug.Log("hello");

                inTask = true;
                if (SpawnObstacles.trials % 5 != 0)
                {
                    SpawnObstacles.timeToSpawn = true;
                }

                break;

            case "Slow":
                speed *= 2f;
                anim.speed *= 0.5f;
                Show(babies, true, true);
                zone = "slow";
                signallogCollection["Event"].Add("KiwiObstacle");
                notify(signallogCollection);
                break;

            case "Sham":
                if (sham)
                {
                    zone = "sham";
                    Jump();
                }

                sham = false;
                CancelInvoke();
                break;

            case "Flying":
                zone = "flying";

                rb.velocity = Vector2.zero;
                rb.gravityScale = 0;

                speed *= 0.5f;

                anim.Play("kiwiFall");
                windSound.Play();
                break;

            case "Babies":
                anim.Play("kiwiIdle");
                music.Stop();
                birdCall.Play();

                end = true;
                exclamation.enabled = false;

                zone = "babies";

                Show(heart, true, true);
                break;

            default:
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Dictionary<string, List<string>> vareventlogCollection = new Dictionary<string, List<string>>();
        vareventlogCollection.Add("Event", new List<string>());

        switch (other.tag)
        {
            case "TriggerZone":
                timing = false;
                Debug.Log("Input window: " + timer + "sec");
                inTask = false;
                //signal.enabled = false;
                break;

            case "Slow":
                speed = baseSpeed;
                if (SpawnObstacles.trials % 5 == 0)
                {
                    SpawnObstacles.timeToSpawn = true;
                    notify(new Dictionary<string, List<string>>() { { "Event", new List<string> { "KiwiObstacle" } } });
                }

                Show(babies, false, false);
                zone = "ground";
                anim.speed *= 2f;
                vareventlogCollection["Event"].Add("TrialEnd");
                notify(vareventlogCollection);
                SpawnObstacles.trials++;
                break;

            case "Flying":
                speed = baseSpeed;
                if (SpawnObstacles.trials % 5 == 0)
                {
                    SpawnObstacles.timeToSpawn = true;
                }

                rb.gravityScale = 1;
                anim.Play("kiwiJump");
                vareventlogCollection["Event"].Add("TrialEnd");
                notify(vareventlogCollection);
                SpawnObstacles.trials++;
                zone = "ground";
                windSound.Stop();
                break;

            default:
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            if (start)
                anim.Play("kiwiRun");
        }
    }



    //public void Task(int input)
    //{
    //    Dictionary<string, List<string>> blinklogCollection = new Dictionary<string, List<string>>();
    //    blinklogCollection.Add("Event", new List<string>());
    //    if (input == 1) //if has completed task, jump and reset success
    //    {

    //        Jump();
    //        List<string> row = new List<string>();
    //        blinklogCollection["Event"].Add("BlinkAccepted");
    //        Debug.Log("input = 1");
    //        notify(blinklogCollection);
    //        i++;



    //    }
    //    else if (input == 2)
    //    {
    //        blinklogCollection["Event"].Add("ShamFeedback");
    //        Debug.Log("input = 2");
    //        notify(blinklogCollection);
    //        i++;
    //        sham = true;
    //        float r = UnityEngine.Random.Range(0.5f, 4.0f);
    //        Invoke("Jump", r);


    //    }

    //    else if (input == 0)
    //    {
    //        blinklogCollection["Event"].Add("BlinkDiscarded");
    //        Debug.Log("input = 0");
    //        notify(blinklogCollection);
    //        i++;

    //        //Debug.Log(input);
    //    }

    //}


    // from Update()
    //if (inTask == false && start && !blinkeye && !canStart && (!BlinkDetector.blinked || Input.GetKeyDown(KeyCode.UpArrow)))
    //{
    //    StartCoroutine(blinkdelay());
    //    Debug.Log("invalid");
    //    Dictionary<string, List<string>> badblinkCollection = new Dictionary<string, List<string>>();
    //    badblinkCollection.Add("Event", new List<string>());
    //    badblinkCollection["Event"].Add("InvalidBlink");
    //    notify(badblinkCollection);
    //}

    //if (inTask && !blinkeye && (!BlinkDetector.blinked || Input.GetKeyDown(KeyCode.UpArrow)))
    //{
    //    StartCoroutine(blinkdelay());
    //    doneDidDoIt = true;
    //    Invoke("StopDoinIt", 0.1f);
    //    Task(input[i]);
    //    inTask = false;
    //}
}
