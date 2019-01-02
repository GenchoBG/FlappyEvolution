using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour {

    private Rigidbody rb;

    public float JumpingForce = 300f;

    public bool Alive { get; set; }

    void Start()
    {
        this.rb = this.GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        //Debug.Log(string.Format("velocity: [{0}, {1}, {2}] {3}", this.rb.velocity.x, this.rb.velocity.y, this.rb.velocity.z, this.rb.velocity.magnitude));
        //Debug.Log(string.Format("position: [{0}, {1}, {2}] {3}", this.rb.position.x, this.rb.position.y, this.rb.position.z, this.rb.position.magnitude));
        if (Input.GetKeyDown("space"))
        {
            this.Jump();
        }
    }

    void Jump()
    {
        this.rb.velocity = Vector3.one;
        this.rb.AddForce(Vector3.up * this.JumpingForce);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pillar"))
        {
            this.Alive = false;
            Destroy(this.gameObject);
        }
    } 
}
