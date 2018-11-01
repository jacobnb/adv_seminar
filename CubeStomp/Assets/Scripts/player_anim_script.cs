using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_anim_script : MonoBehaviour {
    ParticleSystem rightCloud, leftCloud, bottomCloud;
	// Use this for initialization
	void Start () {
        rightCloud = gameObject.transform.Find("RightDustCloud").GetComponent<ParticleSystem>();
        leftCloud = gameObject.transform.Find("LeftDustCloud").GetComponent<ParticleSystem>();
        bottomCloud = gameObject.transform.Find("BottomDustCloud").GetComponent<ParticleSystem>();

    }

    // Update is called once per frame
    void Update () {
		
	}

    public void setAllDusts(bool left, bool right, bool bottom)
    {
        leftDust(left);
        rightDust(right);
        bottomDust(bottom);
    }
    public void leftDust(bool enabled)
    {
        if (enabled)
        {
            leftCloud.Play();
        }
        else
        {
            leftCloud.Stop();
        }
    }
    public void rightDust(bool enabled)
    {
        if (enabled)
        {
            rightCloud.Play();
        }   
        else
        {   
            rightCloud.Stop();
        }
    }
    public void bottomDust(bool enabled)
    {
        if (enabled)
        {
            bottomCloud.Play();
        }
        else
        {
            bottomCloud.Stop();
        }
    }

}
