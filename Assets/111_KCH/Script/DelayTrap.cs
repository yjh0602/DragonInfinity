using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayTrap : MonoBehaviour
{
    Animator animator;
    public float delayTime;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(DelayCo());
    }


    IEnumerator DelayCo()
    {   yield return new WaitForSeconds(delayTime);
        animator.enabled = true;
    }
     

    // Update is called once per frame
    void Update()
    {
        
    }
}
