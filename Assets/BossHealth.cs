using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{

    [SerializeField] private Enemy[] allArms;
    [SerializeField] private Enemy mainBody;
    private bool mainBodyActive = false;

    [SerializeField] private GameObject shield;

    [SerializeField] private Image bossHealthBar;
    private float totalHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        totalHealth = GetTotalHealth();
    }

    // Update is called once per frame
    void Update()
    {
        if(!mainBodyActive && AreAllArmsDestroyed())
        {
            mainBody.damageable = true;
            mainBodyActive = true;
            shield.SetActive(false);
        }

        UpdateHealthUI();
    }

    public bool AreAllArmsDestroyed()
    {
        foreach (Enemy arm in allArms)
        {
            if (!arm.isDead)
            {
                return false;
            }
        }
        return true;
    }

    private void UpdateHealthUI()
    {
        bossHealthBar.fillAmount = GetTotalHealth() / totalHealth;

        if (bossHealthBar.fillAmount <= 0f)
        {
            bossHealthBar.gameObject.transform.parent.gameObject.SetActive(false);
        }
    }

    private float GetTotalHealth()
    {
        float totalHealth = mainBody.health;
        foreach (Enemy arm in allArms)
        {
            totalHealth += arm.health;
        }
        return totalHealth;
    }
}
