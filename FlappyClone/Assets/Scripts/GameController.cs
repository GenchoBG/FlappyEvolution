using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    private List<GameObject> pillars;
    private List<GameObject> birds;

    public GameObject BirdPrefab;
    public GameObject PillarPairPrefab;

    public static GameController Instance { get; private set; }

    public int BirdsCount = 100;
    
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
            if (!this.IsPillarOnScreen(pillar))
            {
                Destroy(pillar);
                this.pillars.Remove(pillar);
            }
        }

        this.TrySpawnPillar();
    }

    public GameObject GetClosestPillar(GameObject bird)
    {
        return this.pillars.OrderByDescending(p =>
        {
            var distance = this.GetHorizontalDistanceFromPillarToBird(bird, p);
            if (distance < 0)
            {
                distance += 10000;
            }

            return distance;
        }).First();
    }
    
    private float GetHorizontalDistanceFromPillarToBird(GameObject bird, GameObject pillar)
    {
        var birdpos = bird.transform.position;
        var pillarpos = pillar.transform.position;

        var distance = birdpos.x - pillarpos.x;
        
        if (birdpos.x > pillarpos.x)
        {
            distance = -distance;
        }

        return distance;
    }

    private float GetCameraDistance()
    {
        return Camera.main.transform.position.y - this.transform.position.y;
    }
    
    private void TrySpawnPillar()
    {
//        var topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, this.GetCameraDistance()));
        var spawnPos = this.PillarPairPrefab.transform.position;

        if (!Physics.CheckSphere(spawnPos, 18))
        {
            var pillar = Instantiate(this.PillarPairPrefab);

            pillar.transform.position = new Vector3(pillar.transform.position.x, pillar.transform.position.y + Random.Range(-8, 12), pillar.transform.position.z);

            this.pillars.Add(pillar);
        }
    }

    private bool IsPillarOnScreen(GameObject gameObject)
    {
        var screenPoint = Camera.main.WorldToViewportPoint(gameObject.transform.position);

        var onScreen = screenPoint.x > 0 && screenPoint.x < 1;

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
