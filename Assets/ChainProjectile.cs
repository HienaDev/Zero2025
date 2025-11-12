using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ChainProjectile : MonoBehaviour
{

    private List<Enemy> enemiesToHit = new List<Enemy>();
    private float damage;

    private Tweener pathTween;

    [SerializeField] private ParticleSystem hitEffect;

    [SerializeField] private AudioClip[] sounds;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(10f);

        Destroy(gameObject);
    }

    public void TravelThroughEnemies(List<Enemy> enemies, float speed, float damage)
    {

        enemiesToHit = enemies;
        this.damage = damage;

        // Convert your Transforms to Vector3 array
        Vector3[] path = new Vector3[enemiesToHit.Count];


        for (int i = 0; i < enemiesToHit.Count; i++)
        {
            path[i] = enemiesToHit[i].transform.position;
        }

        OnReachWaypoint(0); // Apply damage to the first enemy immediately

        pathTween = transform.DOPath(path, speed, PathType.CatmullRom)
            .SetOptions(false)         // 'false' = don't close path
            .SetLookAt(0.01f)          // make it face forward slightly ahead
            .SetEase(Ease.Linear).SetSpeedBased(true)    // consistent speed
            .OnWaypointChange(OnReachWaypoint);
    }

    // This runs every time a waypoint is reached
    void OnReachWaypoint(int waypointIndex)
    {
        if (sounds.Length > 0)
            AudioManager.Instance.Play(sounds[Random.Range(0, sounds.Length)], loop: false, volume: 0.4f, pitch: Random.Range(0.9f, 1.1f));

        enemiesToHit[waypointIndex]?.TakeDamage(damage); // Apply damage to the enemy at this waypoint
        Instantiate(hitEffect, enemiesToHit[waypointIndex].transform.position, Quaternion.identity);
        pathTween.Pause(); 
        DOVirtual.DelayedCall(0.125f, () => pathTween.Play());
    }
}
