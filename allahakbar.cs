﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class allahakbar : MonoBehaviour
{

    public GameObject bullet;
    public Transform shoot;
    public float timeShoot = 4f;

    // Start is called before the first frame update
    void Start() //скрипт выстрела какашками
    {
        shoot.transform.position = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        StartCoroutine(Shooting());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Shooting()
    {
        yield return new WaitForSeconds(timeShoot);
        Instantiate(bullet, shoot.transform.position, transform.rotation);


        StartCoroutine(Shooting());
    }
}
