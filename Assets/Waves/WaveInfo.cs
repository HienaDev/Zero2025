using NaughtyAttributes;
using UnityEngine;

public enum Entity
{
    WalkingEnemy,
    FlyingEnemy,
    DashingEnemy,
    DroppedEnemy,
    FlyingBox,
    GroundBoxBig,
    GroundBoxSmall,
    ConveyorBelt,
}

// --- WaveEvent Struct with NaughtyAttributes ---
[System.Serializable] // Structs must be serializable to show up in the inspector
public class WaveEvent //
{
    public float delay;

    public Entity entityType;

    // --- Shared Fields ---
    [ShowIf(nameof(IsEnemy))] // A general field for all enemies (Walk, Fly, Dash, Dropped)
    [AllowNesting]
    public float speedBoost = 1f;
    [ShowIf(nameof(IsEnemy))] // A general field for all enemies (Walk, Fly, Dash, Dropped)
    [AllowNesting]
    public float healthBoost = 1f;
    [ShowIf(nameof(IsEnemy))] // A general field for all enemies (Walk, Fly, Dash, Dropped)
    [AllowNesting]
    public int spawnPosition = 0;

    // ConveyorBelt
    [ShowIf(nameof(IsConveyorBelt))]
    [AllowNesting]
    public float conveyorSpeed;

    // --- ShowIf Condition Methods (Must be Non-Public) ---

    private bool IsWalkingEnemy() => entityType == Entity.WalkingEnemy;
    private bool IsFlyingEnemy() => entityType == Entity.FlyingEnemy;
    private bool IsDashingEnemy() => entityType == Entity.DashingEnemy;
    private bool IsDroppedEnemy() => entityType == Entity.DroppedEnemy;
    private bool IsFlyingBox() => entityType == Entity.FlyingBox;
    private bool IsGroundBoxBig() => entityType == Entity.GroundBoxBig;
    private bool IsGroundBoxSmall() => entityType == Entity.GroundBoxSmall;
    private bool IsConveyorBelt() => entityType == Entity.ConveyorBelt;

    // Grouping methods for cleaner code
    private bool IsEnemy() =>
        entityType == Entity.WalkingEnemy ||
        entityType == Entity.FlyingEnemy ||
        entityType == Entity.DashingEnemy ||
        entityType == Entity.DroppedEnemy;

    private bool IsBox() =>
        entityType == Entity.FlyingBox ||
        entityType == Entity.GroundBoxBig ||
        entityType == Entity.GroundBoxSmall;
}

[CreateAssetMenu(fileName = "WaveInfo", menuName = "Wave")]
public class WaveInfo : ScriptableObject
{
    [Header("Prefabs")]
    public Enemy WalkingEnemyPrefab;
    public Enemy FlyingEnemyPrefab;
    public Enemy DashingEnemyPrefab;
    public Enemy DroppedEnemyPrefab;
    public GameObject FlyingBoxPrefab;
    public GameObject GroundBoxBigPrefab;
    public GameObject GroundBoxSmallPrefab;

    [Header("General Settings")]
    public float generalEnemySpeedBoost = 1f;
    public float generalEnemyHealthBoost = 1f;




    [ReorderableList] // A nice touch for editing arrays
    public WaveEvent[] waveEvents;
}
