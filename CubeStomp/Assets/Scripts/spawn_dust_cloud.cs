﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawn_dust_cloud : MonoBehaviour {

    [SerializeField]
    GameObject dustCloud;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Ground" || collision.tag == "Player")
        {
            Instantiate(dustCloud, transform.position, dustCloud.transform.rotation);
        }
    }
}
