using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pollusion : MonoBehaviour
{
    public Vector3 target;
    public bool shouldMove = false;
    public float moveSpeed = 5f;

    // Update is called once per frame
    void Update()
   {
        if(shouldMove == true && target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        }
        if(this.transform.position == target)
        {
            shouldMove = false;
        }
    }

    public void SetTarget(Vector3 target)
    {
       
        this.target = transform.position + (target - transform.position).normalized * 1000.0f;
        this.shouldMove = true;
    }
  }

