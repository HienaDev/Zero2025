using NaughtyAttributes;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class ConveyorBelt : MonoBehaviour
{

    private SpriteRenderer sr;
    [SerializeField] public float conveyorBeltSpeed = 5f;
    [SerializeField] private float cogSpeed = 20f;
    [SerializeField] private float boxSpeed = 2f;
    [SerializeField] private float flyingBoxSpeed = 3f;

    private PlayerController playerController;

    [SerializeField] private Transform[] cogs;

    [SerializeField] private GameObject[] boxPrefabs;
    [SerializeField] private Vector2 boxSpawnInterval = new Vector2(3f, 7f);
    private float currentBoxSpawnTimer = 0f;
    private float justSpawnedBox = 0f;
    private List<GameObject> spawnedBoxes;
    [SerializeField] private float distanceToDestroySpawnedBoxes = 15f; // If a box crosses this on the X or -X it is destroyed
    [SerializeField] private Transform spawnerLeft;
    [SerializeField] private Transform spawnerRight;

    [SerializeField] private GameObject[] flyingBoxPrefabs;
    [SerializeField] private Vector2 flyingBoxSpawnInterval = new Vector2(5f, 15f);
    private float currentFlyingBoxSpawnTimer = 0f;
    private float justSpawnedFlyingBox = 0f;
    private List<GameObject> spawnedFlyingBoxes;
    [SerializeField] private float distanceToDestroySpawnedFlyingBoxes = 20f; // If a box crosses this on the X or -X it is destroyed
    [SerializeField] private Transform flyingBoxSpawnerLeft;
    [SerializeField] private Transform flyingBoxSpawnerRight;

    private WaveManager waveManager;

    public void KillAllBoxes()
    {
        foreach(GameObject box in spawnedFlyingBoxes)
        {
            Destroy(box);
        }

        spawnedFlyingBoxes.Clear();

        foreach(GameObject box in spawnedBoxes)
        {
            Destroy(box);
        }

        spawnedBoxes.Clear();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        waveManager = FindAnyObjectByType<WaveManager>();
        spawnedBoxes = new List<GameObject>();
        spawnedFlyingBoxes = new List<GameObject>();

        sr = GetComponent<SpriteRenderer>();
        playerController = FindAnyObjectByType<PlayerController>();

        currentBoxSpawnTimer = Random.Range(boxSpawnInterval.x, boxSpawnInterval.y);
    }

    // Update is called once per frame
    void Update()
    {
        sr.size += new Vector2(conveyorBeltSpeed * Time.deltaTime, 0);
        if (conveyorBeltSpeed != 0)
            playerController.SetConveyorBeltSpeed(conveyorBeltSpeed / -5f);
        else
            playerController.SetConveyorBeltSpeed(0f);

        foreach (Transform cog in cogs)
        {
            cog.Rotate(0, 0, conveyorBeltSpeed * cogSpeed * Time.deltaTime);
        }

        if (conveyorBeltSpeed != 0)
        {
            BoxMovement();
            FlyingBoxMovement();
        }
    }

    public void SpawnGroundBox(GameObject boxPrefab)
    {
        GameObject spawnedBox = Instantiate(boxPrefab, Vector3.zero, Quaternion.identity);
        if (conveyorBeltSpeed < 0)
        {
            spawnedBox.transform.position = new Vector3(spawnerLeft.position.x, spawnerLeft.position.y, spawnedBox.transform.position.z);
        }
        else
        {
            spawnedBox.transform.position = new Vector3(spawnerRight.position.x, spawnerRight.position.y, spawnedBox.transform.position.z);
        }
        spawnedBoxes.Add(spawnedBox);
    }

    public void SpawnFlyingBox(GameObject flyingBoxPrefab)
    {
        GameObject spawnedFlyingBox = Instantiate(flyingBoxPrefab, Vector3.zero, Quaternion.identity);
        if (conveyorBeltSpeed < 0)
        {
            spawnedFlyingBox.transform.position = new Vector3(flyingBoxSpawnerLeft.position.x, flyingBoxSpawnerLeft.position.y, spawnedFlyingBox.transform.position.z);
        }
        else
        {
            spawnedFlyingBox.transform.position = new Vector3(flyingBoxSpawnerRight.position.x, flyingBoxSpawnerRight.position.y, spawnedFlyingBox.transform.position.z);
        }
        spawnedFlyingBoxes.Add(spawnedFlyingBox);
    }

    private void BoxMovement()
    {
        List<GameObject> boxToBeDestroyed = new List<GameObject>();

        foreach (GameObject box in spawnedBoxes)
        {
            box.transform.Translate(Vector3.left * conveyorBeltSpeed * boxSpeed * Time.deltaTime);
            if (Mathf.Abs(box.transform.position.x) > distanceToDestroySpawnedBoxes)
            {
                boxToBeDestroyed.Add(box);
            }
        }

        foreach (GameObject box in boxToBeDestroyed)
        {
            spawnedBoxes.Remove(box);
            Destroy(box);
        }
    }

    private void FlyingBoxMovement()
    {
        List<GameObject> flyingboxToBeDestroyed = new List<GameObject>();

        foreach (GameObject flyingbox in spawnedFlyingBoxes)
        {

            flyingbox.transform.Translate(Vector3.left * conveyorBeltSpeed * flyingBoxSpeed * Time.deltaTime);


            if (Mathf.Abs(flyingbox.transform.position.x) > distanceToDestroySpawnedBoxes)
            {
                flyingboxToBeDestroyed.Add(flyingbox);
            }
        }

        foreach (GameObject flyingbox in flyingboxToBeDestroyed)
        {
            spawnedFlyingBoxes.Remove(flyingbox);
            Destroy(flyingbox);
        }
    }
}
