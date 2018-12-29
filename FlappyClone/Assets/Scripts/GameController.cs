using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private List<GameObject> pillars;
    private List<GameObject> birds;

    public GameObject BirdPrefab;
    public GameObject PillarPairPrefab;

    public static GameController Instance { get; private set; }

    public int BirdsCount = 1;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        this.pillars = new List<GameObject>();
        this.birds = new List<GameObject>();

        this.SpawnBirds();
    }

    void Update()
    {
        foreach (var pillar in this.pillars.ToArray())
        {
            if (!this.IsObjectOnScreen(pillar))
            {
                Destroy(pillar);
                this.pillars.Remove(pillar);
            }
        }

        this.TrySpawnPillar();
    }

    private float GetCameraDistance()
    {
        return Camera.main.transform.position.y - this.transform.position.y;
    }


    private void TrySpawnPillar()
    {
//        var topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, this.GetCameraDistance()));
        var spawnPos = this.PillarPairPrefab.transform.position;

        if (!Physics.CheckSphere(spawnPos, 10))
        {
            var pillar = Instantiate(this.PillarPairPrefab);

            pillar.transform.position = new Vector3(pillar.transform.position.x, pillar.transform.position.y + Random.Range(-5, 5), pillar.transform.position.z);

            this.pillars.Add(pillar);
        }
    }

    private bool IsObjectOnScreen(GameObject gameObject)
    {
        var screenPoint = Camera.main.WorldToViewportPoint(gameObject.transform.position);

        var onScreen = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

        return onScreen;
    }
    
    private void SpawnBirds()
    {
        for (int i = 0; i < this.BirdsCount; i++)
        {
            var bird = Instantiate(this.BirdPrefab);

            var y = Random.Range(0.2f, 0.8f);

            var viewPortPos = Camera.main.WorldToViewportPoint(bird.transform.position);
            viewPortPos.y = y;

            bird.transform.position = Camera.main.ViewportToWorldPoint(viewPortPos);

            foreach (var otherBird in this.birds)
            {
                Physics.IgnoreCollision(bird.GetComponent<Collider>(), otherBird.GetComponent<Collider>());
            }

            this.birds.Add(bird);
        }
    }

}
