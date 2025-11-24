using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private BossAttacks bossPrefab;
    [Serializable] private struct Enemies { public GameObject enemyType; public int enemyCount; }
    [Serializable] private struct Wave { public Enemies[] enemies; }

    [SerializeField] private WaveInfo[] waves;

    [SerializeField] private GameObject UItoDisappear;

    [Header("SpawnPositions")]
    [SerializeField] private Transform[] spawnPositions;

    [Header("Wave Settings")]
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private TMP_FakeSmokeFade[] waveAnnouncementText;
    [SerializeField] private TMP_FakeSmokeFade[] waveCompleteText;

    [Header("Debug Info")]
    [SerializeField] public int currentWave = 1;
    [SerializeField, ReadOnly] private bool waveStarted = false;
    [SerializeField, ReadOnly] private bool upgradeChosen = false;
    [SerializeField, ReadOnly] private bool finishedSpawning = true;
    [SerializeField, ReadOnly] private bool bossSpawned = false;
    [SerializeField, ReadOnly] private bool gameOver = false;

    [Header("Upgrades")]
    [SerializeField] private Transform lvlUpScreen;
    [SerializeField] private LevelUpRelic lvlUpPrefab;
    [SerializeField] private GameObject[] relicPrefabs;
    [SerializeField] private GameObject[] powerupPrefabs;
    [SerializeField] private int numberOfUpgradesToSpawn = 3;
    [SerializeField] private float upgradeOffset = 200f;

    private List<Enemy> activeEnemies = new List<Enemy>();

    private PlayerStats playerStats;

    [SerializeField] private float beltChancePerWave = 0.25f;
    [SerializeField] private Vector2 beltSpeedRange = new Vector2(5f, 15f);
    private ConveyorBelt conveyorBelt;

    [SerializeField] public Image[] relicDisplay;
    private int relicIndex = 0;

    public void AddRelicDisplay(Sprite relicSprite)
    {
        if (relicIndex < relicDisplay.Length)
        {
            relicDisplay[relicIndex].sprite = relicSprite;
            relicDisplay[relicIndex].color = Color.white;
            relicIndex++;
        }
    }

    private void Start()
    {
        conveyorBelt = FindAnyObjectByType<ConveyorBelt>();
        playerStats = FindAnyObjectByType<PlayerStats>();
        upgradeChosen = true;
        

        StartCoroutine(WaveLoop());
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.N))
        {
            SpawnUpgrades();
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.M))
        {
            SpawnPowerups();
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.B))
        {
            currentWave = 15;
            UItoDisappear.SetActive(false);
        }
    }

    private IEnumerator WaveLoop()
    {
        // Infinite wave loop (stop when game ends, etc.)
        while (true)
        {
            if (!waveStarted && upgradeChosen)
            {
                foreach (var text in waveAnnouncementText)
                {
                    text.gameObject.SetActive(true);
                    if (currentWave < 10)
                        text.GetComponent<TextMeshProUGUI>().text = $"Wave 0{currentWave}";
                    else
                        text.GetComponent<TextMeshProUGUI>().text = $"Wave {currentWave}";
                }

                //FindAnyObjectByType<ConveyorBelt>().conveyorBeltSpeed = waves[currentWave - 1].conve

                yield return new WaitForSeconds(timeBetweenWaves - 3f);

                foreach (var text in waveAnnouncementText)
                {
                    text.FadeOut();
                }

                yield return new WaitForSeconds(3f);

                StartCoroutine(StartWave(currentWave));
            }

            yield return null;

            foreach (Enemy enemy in activeEnemies.ToArray())
            {
                if (enemy == null)
                {
                    activeEnemies.Remove(enemy);
                }
            }

            // Check if wave is finished
            if (finishedSpawning && activeEnemies.Count == 0 && waveStarted)
            {

                if (bossSpawned)
                    gameOver = true;

                FindAnyObjectByType<ConveyorBelt>().conveyorBeltSpeed = 0f;
                Debug.Log($"--- Wave {currentWave} Completed ---");
                foreach (var text in waveCompleteText)
                {
                    text.gameObject.SetActive(true);
                }
                yield return new WaitForSeconds(timeBetweenWaves - 3f);

                foreach (var text in waveCompleteText)
                {
                    text.FadeOut();
                }
                yield return new WaitForSeconds(3f);

                if (gameOver)
                    yield break;

                waveStarted = false;

                upgradeChosen = false;
                SpawnUpgrades();
            }
        }
    }

    private void SpawnUpgrades()
    {
        lvlUpScreen.parent.gameObject.SetActive(true);

        if (currentWave % 2 == 0)
        {
            relicPrefabs.Shuffle();

            List<GameObject> possibleRelics = new List<GameObject>();
            List<ProjectileEffect> effectsToLevelUp = new List<ProjectileEffect>();

            foreach (var relicPrefab in relicPrefabs)
            {
                AddProjectileEffect relicEffect = relicPrefab.GetComponent<AddProjectileEffect>();
                Debug.Log(relicEffect);
                ProjectileEffect effect = relicEffect.EffectToAdd;

                // Check if player already has this effect
                var existing = playerStats.ProjectileEffects.Find(e => e.GetType().Name == effect.GetType().Name);

                ProjectileEffect[] effects = relicEffect.dependsOnThisEffect;


                bool hasDependecies = true;

                Debug.Log("current effect: " + effect.GetType().Name + " and dependecies length: " + effects.Length);

                if (effects != null && effects.Length > 0)
                {
                    foreach (var dep in effects)
                    {
                        var hasEffect = playerStats.ProjectileEffects.Find(e => e.GetType().Name == dep.GetType().Name);
                        if (hasEffect == null)
                        {
                            hasDependecies = false;
                            break;
                        }
                    }
                }

                if (!hasDependecies)
                    continue;


                if (existing == null)
                {
                    // new effect, can be offered normally
                    possibleRelics.Add(relicPrefab);
                }
                else if (!existing.IsAtMaxLevel)
                {
                    // same effect, but can level up
                    effectsToLevelUp.Add(existing);
                }
            }

            // No available relics at all? → fallback to powerups
            if (possibleRelics.Count == 0 && effectsToLevelUp.Count == 0)
            {
                Debug.Log("No relics available — spawning powerups instead.");
                SpawnPowerups();
                return;
            }

            int spawned = 0;

            // Spawn up to N upgrades
            while (spawned < numberOfUpgradesToSpawn)
            {
                Vector3 spawnPos = new Vector3((spawned - numberOfUpgradesToSpawn / 2) * upgradeOffset, 40f, 0f);

                // 50% chance to show a level-up if any are available
                bool canLvlUp = effectsToLevelUp.Count > 0 && UnityEngine.Random.value < 0.5f;

                if (possibleRelics.Count == 0 && effectsToLevelUp.Count > 0)
                {
                    canLvlUp = true; // force level-up if no new relics are available
                }
                canLvlUp = false;
                GameObject upgradeInstance;

                if (canLvlUp)
                {
                    ProjectileEffect effectToLvlUp = effectsToLevelUp[UnityEngine.Random.Range(0, effectsToLevelUp.Count)];
                    upgradeInstance = Instantiate(lvlUpPrefab.gameObject, lvlUpScreen);//, Quaternion.identity);
                    upgradeInstance.transform.localPosition = spawnPos;
                    LevelUpRelic relic = upgradeInstance.GetComponent<LevelUpRelic>();
                    relic.SetEffectToLvlUp(effectToLvlUp);
                    effectsToLevelUp.Remove(effectToLvlUp); // prevent duplicates
                }
                else if (possibleRelics.Count > 0)
                {
                    GameObject relicToSpawn = possibleRelics[UnityEngine.Random.Range(0, possibleRelics.Count)];
                    upgradeInstance = Instantiate(relicToSpawn, lvlUpScreen);//, Quaternion.identity);
                    upgradeInstance.transform.localPosition = spawnPos;
                    possibleRelics.Remove(relicToSpawn);
                }
                else
                {
                    int remainingSlots = numberOfUpgradesToSpawn - spawned;
                    if (remainingSlots > 0)
                        SpawnPowerups(remainingSlots, spawned);
                    return;

                }

                // Attach button logic
                Button upgradeButton = upgradeInstance.GetComponent<Button>();
                upgradeButton.onClick.AddListener(() => { FinishUpgrading(); });

                spawned++;
            }
        }
        else
        {
            // Even waves → powerups
            SpawnPowerups();
        }

    }

    private void SpawnPowerups(int amountToSpawn = -1, int filledSlots = 0)
    {
        if (amountToSpawn < 0)
            amountToSpawn = numberOfUpgradesToSpawn;

        powerupPrefabs.Shuffle();

        for (int i = 0; i < amountToSpawn && i < powerupPrefabs.Length; i++)
        {
            // Start after already filled slots
            int slotIndex = i + filledSlots;
            Vector3 spawnPos = new Vector3(
                (slotIndex - numberOfUpgradesToSpawn / 2) * upgradeOffset,
                40f,
                0f
            );

            GameObject upgrade = Instantiate(powerupPrefabs[i], lvlUpScreen);
            upgrade.transform.localPosition = spawnPos;

            Button button = upgrade.GetComponent<Button>();
            button.onClick.AddListener(() => { FinishUpgrading(); });
        }
    }



    public void FinishUpgrading()
    {
        foreach (Transform child in lvlUpScreen)
        {
            Destroy(child.gameObject);
        }
        lvlUpScreen.parent.gameObject.SetActive(false);
        upgradeChosen = true;
    }


    private IEnumerator StartWave(int waveNumber)
    {
        waveStarted = true;
        finishedSpawning = false;

        Debug.Log($"--- Starting Wave {waveNumber} ---");

        

        if (waveNumber - 1 >= waves.Length)
        {
            // Spawn boss
            BossAttacks boss = Instantiate(bossPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            //boss.Initialize(playerStats);

            conveyorBelt.KillAllBoxes();

            foreach (Enemy enemy in boss.GetComponentsInChildren<Enemy>())
            {
                activeEnemies.Add(enemy);
            }

            finishedSpawning = true;
            yield break;
        }


        WaveInfo waveInfo = waves[waveNumber - 1];

        WaveEvent[] events = waveInfo.waveEvents;

        foreach (var waveEvent in events)
        {
            float timeBetweenEvents = waveEvent.delay;

            yield return new WaitForSeconds(timeBetweenEvents);

            Enemy enemyTemp;
            switch (waveEvent.entityType)
            {
                // Fill in for every entity type
                case Entity.WalkingEnemy:
                    enemyTemp = Instantiate(waveInfo.WalkingEnemyPrefab, Vector3.zero, Quaternion.identity);
                    enemyTemp.health *= (waveEvent.healthBoost + waveInfo.generalEnemyHealthBoost - 1);
                    enemyTemp.speed *= (waveEvent.speedBoost + waveInfo.generalEnemySpeedBoost - 1);
                    enemyTemp.transform.position = spawnPositions[waveEvent.spawnPosition].position;
                    activeEnemies.Add(enemyTemp);
                    break;
                case Entity.FlyingEnemy:
                    enemyTemp = Instantiate(waveInfo.FlyingEnemyPrefab, Vector3.zero, Quaternion.identity);
                    enemyTemp.health *= (waveEvent.healthBoost + waveInfo.generalEnemyHealthBoost - 1);
                    enemyTemp.speed *= (waveEvent.speedBoost + waveInfo.generalEnemySpeedBoost - 1);
                    enemyTemp.transform.position = spawnPositions[waveEvent.spawnPosition].position;
                    activeEnemies.Add(enemyTemp);
                    break;
                case Entity.DashingEnemy:
                    enemyTemp = Instantiate(waveInfo.DashingEnemyPrefab, Vector3.zero, Quaternion.identity);
                    enemyTemp.health *= (waveEvent.healthBoost + waveInfo.generalEnemyHealthBoost - 1);
                    enemyTemp.speed *= (waveEvent.speedBoost + waveInfo.generalEnemySpeedBoost - 1);
                    enemyTemp.transform.position = spawnPositions[waveEvent.spawnPosition].position;
                    activeEnemies.Add(enemyTemp);
                    break;
                case Entity.DroppedEnemy:
                    enemyTemp = Instantiate(waveInfo.DroppedEnemyPrefab, Vector3.zero, Quaternion.identity);
                    enemyTemp.health *= (waveEvent.healthBoost + waveInfo.generalEnemyHealthBoost - 1);
                    enemyTemp.speed *= (waveEvent.speedBoost + waveInfo.generalEnemySpeedBoost - 1);
                    enemyTemp.transform.position = spawnPositions[waveEvent.spawnPosition].position;
                    activeEnemies.Add(enemyTemp);
                    break;
                case Entity.FlyingBox:
                    conveyorBelt.SpawnFlyingBox(waveInfo.FlyingBoxPrefab);
                    break;
                case Entity.GroundBoxBig:
                    conveyorBelt.SpawnGroundBox(waveInfo.GroundBoxBigPrefab);
                    break;
                case Entity.GroundBoxSmall:
                    conveyorBelt.SpawnGroundBox(waveInfo.GroundBoxSmallPrefab);
                    break;
                case Entity.ConveyorBelt:
                    conveyorBelt.conveyorBeltSpeed = waveEvent.conveyorSpeed;
                    break;

            }
            
        }

        // Done spawning all enemies for this wave
        finishedSpawning = true;

        currentWave++;

        Debug.Log($"--- Finished Spawning Wave {waveNumber} ---");
    }

    private void SpawnEnemy(Enemy prefab, Transform[] spawnPoints)
    {
        if (prefab == null || spawnPoints == null || spawnPoints.Length == 0)
            return;

        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        Enemy newEnemy = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        newEnemy.health *= 1 + (0.15f * currentWave);
        newEnemy.speed *= 1 + (0.075f * currentWave);
        newEnemy.originalSpeed *= 1 + (0.075f * currentWave);
        activeEnemies.Add(newEnemy);
    }
}
