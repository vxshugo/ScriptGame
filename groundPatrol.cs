﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundPatrol : MonoBehaviour
{


    public float speed = 1f;
    public bool moveleft = true;
    public Transform groundDetect;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()  // Скрипт для наземных врагов (метод чекает есть ли земля перед ним)
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetect.position, Vector2.down, 1f);


        if(groundInfo.collider == false)
        {
            if(moveleft == true)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
                moveleft = false;
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                moveleft = true;
            }
        }
    }
}
