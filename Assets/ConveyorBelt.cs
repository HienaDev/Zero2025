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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnedBoxes = new List<GameObject>();

        sr = GetComponent<SpriteRenderer>();
        playerController = FindAnyObjectByType<PlayerController>();

        currentBoxSpawnTimer = Random.Range(boxSpawnInterval.x, boxSpawnInterval.y);
    }

    // Update is called once per frame
    void Update()
    {
        sr.size += new Vector2(conveyorBeltSpeed * Time.deltaTime, 0);
        if(conveyorBeltSpeed != 0)
            playerController.SetConveyorBeltSpeed(conveyorBeltSpeed / -5f );
        else
            playerController.SetConveyorBeltSpeed(0f);

        foreach (Transform cog in cogs)
        {
            cog.Rotate(0, 0, conveyorBeltSpeed * cogSpeed * Time.deltaTime);
        }

        if(conveyorBeltSpeed != 0)
            BoxLogic(); 

    }

    private void BoxLogic()
    {
        if (Time.time > justSpawnedBox + currentBoxSpawnTimer)
        {
            Debug.Log("Time.time: " + Time.time + " justSpawnedBox: " + justSpawnedBox + " currentBoxSpawnTimer: " + currentBoxSpawnTimer);
            currentBoxSpawnTimer = Random.Range(boxSpawnInterval.x, boxSpawnInterval.y);
            justSpawnedBox = Time.time;

            GameObject boxToSpawn = boxPrefabs[Random.Range(0, boxPrefabs.Length)];
            GameObject spawnedBox = Instantiate(boxToSpawn, Vector3.zero, Quaternion.identity);
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
}
