using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Kontroler Musuh yang membaca data langsung dari ScriptableObject EnemyStats (Live Sheet).
/// Memberikan reward koin ke CoinManager saat dikalahkan.
/// </summary>
public class EnemyController : MonoBehaviour
{
    [Header("ScriptableObject Data (Live Sheet)")]
    public EnemyStats stats;

    [Header("Live State")]
    [SerializeField] private float currentHP;
    [SerializeField] private bool isDead = false;

    [Header("UI & Visual Elements")]
    public TMP_Text billboardText;
    public MeshRenderer bodyRenderer;

    private Color originalColor;
    private Coroutine flashCoroutine;

    public float CurrentHP => currentHP;
    public bool IsDead => isDead;

    private void Awake()
    {
        if (bodyRenderer == null)
            bodyRenderer = GetComponentInChildren<MeshRenderer>();

        if (bodyRenderer != null && bodyRenderer.material != null)
            originalColor = bodyRenderer.material.color;
    }

    private void Start()
    {
        InitializeFromStats();
    }

    public void InitializeFromStats()
    {
        if (stats == null)
        {
            Debug.LogWarning($"[EnemyController] {gameObject.name} belum memiliki referensi EnemyStats!");
            return;
        }

        currentHP = stats.maxHP;
        isDead = false;

        if (bodyRenderer != null && bodyRenderer.material != null)
        {
            bodyRenderer.material.color = stats.themeColor;
            originalColor = stats.themeColor;
        }

        UpdateBillboard();
    }

    /// <summary>
    /// Menerima damage dengan memperhitungkan reduksi armor musuh
    /// Formula: actualDamage = incomingDamage * (1 - armor)
    /// </summary>
    public void TakeDamage(float incomingDamage)
    {
        if (isDead) return;

        float effectiveReduction = stats != null ? stats.armor : 0f;
        float actualDamage = incomingDamage * (1f - effectiveReduction);
        currentHP = Mathf.Max(0f, currentHP - actualDamage);

        Debug.Log($"[{gameObject.name}] Terkena raw damage {incomingDamage:F1}, " +
                  $"direduksi armor {effectiveReduction * 100:F0}% -> Net damage: {actualDamage:F1}. " +
                  $"Sisa HP: {currentHP:F1}/{stats.maxHP}");

        FlashColor(Color.white);
        UpdateBillboard();

        if (currentHP <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// Logika kematian musuh: mentransfer reward koin ke Singleton CoinManager
    /// </summary>
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        int reward = stats != null ? stats.RewardGold : 5;
        Debug.Log($"[{gameObject.name}] Dikalahkan! Memberikan reward {reward} koin ke CoinManager.");

        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.AddCoins(reward);
        }

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        FlashColor(Color.red);
        if (billboardText != null)
        {
            billboardText.text = $"<color=red>DEFEATED!</color>\n+{stats.RewardGold} Gold";
        }

        yield return new WaitForSeconds(1.5f);

        InitializeFromStats();
    }

    private void FlashColor(Color flashCol)
    {
        if (bodyRenderer == null) return;
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashRoutine(flashCol));
    }

    private IEnumerator FlashRoutine(Color flashCol)
    {
        bodyRenderer.material.color = flashCol;
        yield return new WaitForSeconds(0.12f);
        if (!isDead && bodyRenderer != null)
        {
            bodyRenderer.material.color = originalColor;
        }
    }

    public void UpdateBillboard()
    {
        if (billboardText == null || stats == null) return;

        float hpPercent = (currentHP / stats.maxHP) * 100f;
        billboardText.text = $"<b>{stats.enemyName}</b>\n" +
                             $"HP: <color=#2ECC71>{currentHP:F0}</color>/{stats.maxHP:F0} ({hpPercent:F0}%)\n" +
                             $"Armor: <b>{stats.armor * 100:F0}%</b> | Spd: <b>{stats.speed}</b>\n" +
                             $"Cost: <b>{stats.cost}</b> | Reward: <color=#F1C40F>+{stats.RewardGold}G</color>";
    }

    private void Update()
    {
        // Pastikan billboard selalu menghadap kamera utama
        if (billboardText != null && Camera.main != null)
        {
            billboardText.transform.rotation = Quaternion.LookRotation(billboardText.transform.position - Camera.main.transform.position);
        }
    }
}

