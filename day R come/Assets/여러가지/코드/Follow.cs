using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
   [SerializeField]
 public Transform target;
  [SerializeField]
 public Vector3 offset;

    // Update is called once per frame
    void Update()
    {
      if(!target)
        return;
      
      transform.position=target.position+offset; 
      transform.parent = target;
    }
}
