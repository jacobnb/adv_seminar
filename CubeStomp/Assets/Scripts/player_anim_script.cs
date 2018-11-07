using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_anim_script : MonoBehaviour {
    [SerializeField]
    GameObject stunAnim;
    GameObject stunAnimPlaying;
    ParticleSystem rightCloud, leftCloud, bottomCloud;
	void Start () {
        rightCloud = gameObject.transform.Find("RightDustCloud").GetComponent<ParticleSystem>();
        leftCloud = gameObject.transform.Find("LeftDustCloud").GetComponent<ParticleSystem>();
        bottomCloud = gameObject.transform.Find("BottomDustCloud").GetComponent<ParticleSystem>();

    }
    public void disableStunAnim()
    {
        if (stunAnimPlaying)
        {
            Destroy(stunAnimPlaying);
        }
    }
    public void enableStunAnim()
    {
        if (!stunAnimPlaying)
        {
            stunAnimPlaying = Instantiate(stunAnim);
            stunAnimPlaying.GetComponent<stunAnimScript>().init(transform);
        }
    }
    public void setAllDusts(bool left, bool right, bool top)
    {
        leftDust(left);
        rightDust(right);
        topDust(top);
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
    public void topDust(bool enabled)
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
