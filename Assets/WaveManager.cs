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

    [Serializable] private struct Enemies { public GameObject enemyType; public int enemyCount; }
    [Serializable] private struct Wave { public Enemies[] enemies; }

    [SerializeField] private Wave[] enemyWaves;

    [Header("Wave Settings")]
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private float timeBetweenSpawns = 3f;
    [SerializeField] private TMP_FakeSmokeFade[] waveAnnouncementText;
    [SerializeField] private TMP_FakeSmokeFade[] waveCompleteText;

    [Header("Walking Enemies")]
    [SerializeField] private Transform[] spawnPointsWalk;
    [SerializeField] private Enemy walkingEnemyPrefab;
    [SerializeField] private int walkingEnemiesPerWave = 6;

    [Header("Flying Enemies")]
    [SerializeField] private Transform[] spawnPointsFly;
    [SerializeField] private Enemy flyingEnemyPrefab;
    [SerializeField] private int flyingEnemiesShowUpOnWave = 3;
    [SerializeField] private int flyingEnemiesPerWave = 8;

    [Header("Dashing Enemies")]
    [SerializeField] private Transform[] spawnPointsDash;
    [SerializeField] private Enemy dashingEnemyPrefab;
    [SerializeField] private int dashingEnemiesShowUpOnWave = 6;
    [SerializeField] private int dashingEnemiesPerWave = 6;

    [Header("Debug Info")]
    [SerializeField] private int currentWave = 1;
    [SerializeField, ReadOnly] private bool waveStarted = false;
    [SerializeField, ReadOnly] private bool upgradeChosen = false;
    [SerializeField, ReadOnly] private bool finishedSpawning = true;

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
        playerStats = FindAnyObjectByType<PlayerStats>();
        upgradeChosen = false;
        SpawnUpgrades();

        StartCoroutine(WaveLoop());
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            SpawnUpgrades();
        }
        if(Input.GetKeyDown(KeyCode.M))
        {
            SpawnPowerups();
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
                    if(currentWave < 10)
                        text.GetComponent<TextMeshProUGUI>().text = $"Wave 0{currentWave}";
                    else
                        text.GetComponent<TextMeshProUGUI>().text = $"Wave {currentWave}";
                }

                if(UnityEngine.Random.value < beltChancePerWave)
                {
                    float beltSpeed = UnityEngine.Random.Range(beltSpeedRange.x, beltSpeedRange.y);
                    beltSpeed *= UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1; // random direction
                    FindAnyObjectByType<ConveyorBelt>().conveyorBeltSpeed = (beltSpeed);
                }
                else
                {
                    FindAnyObjectByType<ConveyorBelt>().conveyorBeltSpeed = 0f;
                }

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
                waveStarted = false;

                upgradeChosen = false;
                SpawnUpgrades();
            }
        }
    }

    private void SpawnUpgrades()
    {
        lvlUpScreen.gameObject.SetActive(true);    
        // If odd wave → relics; even → powerups
        if (currentWave % 2 == 1)
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
                Vector3 spawnPos = new Vector3((spawned - numberOfUpgradesToSpawn / 2) * upgradeOffset, 80f, 0f);

                // 50% chance to show a level-up if any are available
                bool canLvlUp = effectsToLevelUp.Count > 0 && UnityEngine.Random.value < 0.5f;

                if(possibleRelics.Count == 0 && effectsToLevelUp.Count > 0)
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
                upgradeButton.onClick.AddListener(() => { FinishUpgrading();});

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
                80f,
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
        lvlUpScreen.gameObject.SetActive(false);
        upgradeChosen = true;
    }


    private IEnumerator StartWave(int waveNumber)
    {
        waveStarted = true;
        finishedSpawning = false;

        Debug.Log($"--- Starting Wave {waveNumber} ---");

        // Determine how many enemies to spawn for each type
        int walkRemaining = walkingEnemiesPerWave * waveNumber;
        int flyRemaining = (waveNumber >= flyingEnemiesShowUpOnWave) ? flyingEnemiesPerWave * (waveNumber - flyingEnemiesShowUpOnWave + 1) : 0;
        int dashRemaining = (waveNumber >= dashingEnemiesShowUpOnWave) ? dashingEnemiesPerWave * (waveNumber - dashingEnemiesPerWave + 1) : 0;

        // Keep spawning until all counts hit zero
        while (walkRemaining > 0 || flyRemaining > 0 || dashRemaining > 0)
        {
            // Spawn 1 of each available type per cycle
            if (walkRemaining > 0)
            {
                SpawnEnemy(walkingEnemyPrefab, spawnPointsWalk);
                walkRemaining--;
            }

            if (flyRemaining > 0)
            {
                SpawnEnemy(flyingEnemyPrefab, spawnPointsFly);
                flyRemaining--;
            }

            if (dashRemaining > 0)
            {
                SpawnEnemy(dashingEnemyPrefab, spawnPointsDash);
                dashRemaining--;
            }

            // Wait before next spawn cycle, scaled by wave number
            yield return new WaitForSeconds(Mathf.Max(0.2f, timeBetweenSpawns - 0.1f * waveNumber));
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
