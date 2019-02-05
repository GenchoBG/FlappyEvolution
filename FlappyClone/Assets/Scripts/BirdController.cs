using System;
using System.Collections.Generic;
using System.Diagnostics;
using SimpleNeuralNetwork;
using SimpleNeuralNetwork.Interfaces;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class BirdController : MonoBehaviour
{
    private Rigidbody rb;

    public float JumpingForce = 300f;

    public bool Alive { get; set; }

    public Stopwatch AliveTime { get; set; }
    
    public ITrainableNetwork Brain { get; set; }

    void Start()
    {
        //inputs are: vertical distance to top pipe, vertical distance to bottom pipe, horisontal distance to pipes, vertical velocity
        //output is one: should it jump
        this.rb = this.GetComponent<Rigidbody>();
        this.Alive = true;
        this.AliveTime = new Stopwatch();
        this.AliveTime.Start();
    }

    void Update()
    {
        var closestPillarPair = GameController.Instance.GetClosestPillar(this.gameObject);
        var birdpos = this.transform.position;
        var pillarpos = closestPillarPair.transform.position;

        var horizontalDistance = Math.Abs(birdpos.x - pillarpos.x);
        var verticalVelocity = this.rb.velocity.y;
        var verticalDistanceToBottomPipe = (pillarpos.y - 24) - birdpos.y; //hardcoded values. i got them by fiddling around in the Unity Editor
        var verticalDistanceToTopPipe = 0.6 - verticalDistanceToBottomPipe; //some day i will probably learn how to remove this horrible magic numbers from my code


        if (!this.IsOnScreen())
        {
            this.Die();
        }

        var prediction = this.Brain.FeedForward(new List<double> { verticalDistanceToTopPipe, verticalDistanceToBottomPipe, horizontalDistance, verticalVelocity })[0];

        if (prediction > 0.5)
        {
            this.Jump();
        }
//
//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            this.Jump();
//        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pillar"))
        {
            this.Die();
        }
    }

    private void Jump()
    {
        this.rb.velocity = Vector3.one;
        this.rb.AddForce(Vector3.up * this.JumpingForce);
    }

    private void Die()
    {
        this.Alive = false;
        this.AliveTime.Stop();
        GameController.Instance.DeadBirds.Add(this.Brain, this.AliveTime.Elapsed);
        GameController.Instance.UnregisterBird(this.gameObject);
        Destroy(this.gameObject);
    }

    private bool IsOnScreen()
    {
        var screenPoint = Camera.main.WorldToViewportPoint(this.transform.position);

        var onScreen = screenPoint.y > 0 && screenPoint.y < 1;

        return onScreen;
    }
}
