using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileEffects", menuName = "Scriptable Objects/ProjectileEffects")]
public class ProjectileEffects : ScriptableObject
{
    public ProjectileEffect projectileEffect;

    public float effectChance = 1f;

}
