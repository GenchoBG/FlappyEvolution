using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarMovementScript : MonoBehaviour
{
    public float PillarSpeed = 5;

    
    void Update()
    {
        this.transform.position = new Vector3(this.transform.position.x - this.PillarSpeed * Time.deltaTime, this.transform.position.y, this.transform.position.z);
    }
}
