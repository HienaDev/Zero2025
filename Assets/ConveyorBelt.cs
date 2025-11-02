using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{

    private SpriteRenderer sr;
    [SerializeField] private float conveyorBeltSpeed = 5f;
    [SerializeField] private float cogSpeed = 20f;

    private PlayerController playerController;

    [SerializeField] private Transform[] cogs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        playerController = FindAnyObjectByType<PlayerController>();
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
    }
}
