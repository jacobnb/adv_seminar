using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawn_dust_cloud : MonoBehaviour {

    [SerializeField]
    GameObject dustCloud;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Instantiate(dustCloud, transform.position, dustCloud.transform.rotation);
    }
}
