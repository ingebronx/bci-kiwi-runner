﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObstacles : MonoBehaviour
{

    public GameObject[] obstacles;
    public GameObject babies;

    private float spawnPosX;
    private float spawnPosY;
    private Vector3 spawnPoint; //obstacle spawnpoint

    
    public static bool timeToSpawn; //checks if it is time to spawn

    public static int trials = 1;
    public int maxTrials = 20;
    
    void Start() //set spawntime at beginning
    {
        //spawnTime = 0;

        Transform ground = gameObject.transform;
        spawnPosY = ground.position.y + ground.localScale.y / 2;
        spawnPosX = 25;
        spawnPoint = new Vector3(spawnPosX, spawnPosY);

        timeToSpawn = true;
    }
    
    void Update() 
    {
        if (Avatar.start && trials <= maxTrials)
        {
            if (timeToSpawn) //when spawntime hits 0, and we're in the ITT, spawn obstacle
            {
                int rnd = Random.Range(0, obstacles.Length);
                Spawn(obstacles[rnd], spawnPoint);
            }
        }
        else if(trials > maxTrials && timeToSpawn)
        {
            Vector3 spawnPoint2 = new Vector3(spawnPosX, spawnPosY, 0);
            Debug.Log("babies spawn at: " + spawnPoint2);
            Spawn(babies, spawnPoint2);
        }
    }

    void Spawn(GameObject obs, Vector3 spawnPoint)
    {
        Instantiate(obs, spawnPoint, Quaternion.identity); //spawn new random obstacle
        timeToSpawn = false;
    }
}
