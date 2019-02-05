using System;
using System.Collections.Generic;
using System.Linq;
using SimpleNeuralNetwork;
using SimpleNeuralNetwork.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    private List<GameObject> pillars;
    private HashSet<GameObject> birds;

    public GameObject BirdPrefab;
    public GameObject PillarPairPrefab;

    public static GameController Instance { get; private set; }

    public Dictionary<ITrainableNetwork, TimeSpan> DeadBirds { get; set; }

    private const int FitnessRaisePower = 10;

    public int BirdsCount = 100;
    public float MutationRate = 1F;
    public int Generation { get; private set; }


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
        this.birds = new HashSet<GameObject>();
        this.DeadBirds = new Dictionary<ITrainableNetwork, TimeSpan>();
        this.Generation = 0;
    }

    void Update()
    {
        if (!this.birds.Any())
        {
            this.NewGeneration();
        }

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

    public void UnregisterBird(GameObject bird)
    {
        this.birds.Remove(bird);
    }

    private void NewGeneration()
    {
        this.Generation++;
        this.RemovePillars();
        this.CreateGeneration();
        this.DeadBirds.Clear();
    }

    private void RemovePillars()
    {
        foreach (var pillar in this.pillars)
        {
            Destroy(pillar);
        }
        this.pillars.Clear();
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
        var spawnPos = this.PillarPairPrefab.transform.position;

        if (!Physics.CheckSphere(spawnPos, 18))
        {
            var pillar = Instantiate(this.PillarPairPrefab);

            pillar.transform.position = new Vector3(pillar.transform.position.x, pillar.transform.position.y + Random.Range(-8, 12), pillar.transform.position.z);

            this.pillars.Add(pillar);
        }
    }

    private bool IsPillarOnScreen(GameObject pillar)
    {
        var screenPoint = Camera.main.WorldToViewportPoint(pillar.transform.position);

        var onScreen = screenPoint.x > 0 && screenPoint.x < 1;

        return onScreen;
    }

    private void CreateGeneration()
    {

        var brains = new List<ITrainableNetwork>();
        
        //Generate birds brains
        if (this.DeadBirds.Count == 0)
        {
            for (int i = 0; i < this.BirdsCount; i++)
            {
                brains.Add(new TrainableNetwork(4, new List<int>() { 5 }, 1));
            }
        }
        else
        {
            var probabilities = this.GenerateProbabilities(this.DeadBirds);

            for (int i = 0; i < this.BirdsCount; i++)
            {
                var firstParent = this.GetRandomParent(this.DeadBirds.Keys.ToList(), probabilities);
                var secondParent = this.GetRandomParent(this.DeadBirds.Keys.ToList(), probabilities);

                var brain = firstParent.Crossover(secondParent);
                brain.Mutate(this.MutationRate);

                brains.Add(brain);
            }
        }

        //spawn the birds
        for (int i = 0; i < this.BirdsCount; i++)
        {
            var bird = Instantiate(this.BirdPrefab);
            bird.GetComponent<BirdController>().Brain = brains[i];
            

            foreach (var otherBird in this.birds)
            {
                Physics.IgnoreCollision(bird.GetComponent<Collider>(), otherBird.GetComponent<Collider>());
            }

            this.birds.Add(bird);
        }
    }

    private IDictionary<ITrainableNetwork, float> GenerateProbabilities(IDictionary<ITrainableNetwork, TimeSpan> previous)
    {
        var fitnessSum = 0F;

        foreach (var bird in previous)
        {
            var fitness = (float)bird.Value.TotalMilliseconds / 1000F;

            fitnessSum += Mathf.Pow(fitness, FitnessRaisePower);
        }

        var probabilities = new Dictionary<ITrainableNetwork, float>();
        foreach (var bird in previous)
        {
            var fitness = (float)bird.Value.TotalMilliseconds / 1000F;

            probabilities[bird.Key] = Mathf.Pow(fitness, FitnessRaisePower) / fitnessSum;
        }

        return probabilities;
    }

    private ITrainableNetwork GetRandomParent(IList<ITrainableNetwork> previous, IDictionary<ITrainableNetwork, float> probabilities)
    {
        for (int i = 0; i < this.BirdsCount; i++)
        {
            var random = Random.Range(0f, 1f);
            for (int j = 0; j < this.BirdsCount; j++)
            {
                random -= probabilities[previous[i]];
                if (random <= 0)
                {
                    return previous[i];
                }
            }
        }

        throw new Exception("Can't generate random parent");
    }
}
