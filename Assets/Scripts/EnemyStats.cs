using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStats", menuName = "Fortress of Nations/Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    [Header("Identitas Musuh")]
    public string enemyName = "Enemy";
    [TextArea(2, 3)]
    public string description = "";
    public Color themeColor = Color.white;

    [Header("Atribut Dasar (Live Sheet)")]
    [Tooltip("Health Points awal")]
    public float maxHP = 100f;

    [Tooltip("Damage per serangan")]
    public float damage = 10f;

    [Tooltip("Kecepatan gerak musuh (m/s)")]
    public float speed = 2f;

    [Tooltip("Attack Cooldown dalam detik")]
    public float cooldown = 1f;

    [Range(0f, 0.99f)]
    [Tooltip("Persentase armor / reduksi damage (misal 0.05 = 5%, 0.40 = 40%)")]
    public float armor = 0.05f;

    [Tooltip("Cost / resource point musuh")]
    public int cost = 10;

    [Tooltip("Range Multiplier untuk Threat Score")]
    public float rangeMultiplier = 1f;

    // =========================================================================
    // FORMULA BALANCING (Live Calculated dari Sheet PDF Bagian A)
    // =========================================================================

    /// <summary>
    /// DPS = Damage / Cooldown
    /// </summary>
    public float DPS => cooldown > 0.001f ? damage / cooldown : damage;

    /// <summary>
    /// Effective HP = HP / (1 - Armor)
    /// </summary>
    public float EffectiveHP => armor < 0.999f ? maxHP / (1f - armor) : maxHP;

    /// <summary>
    /// PowerScore = (Damage / Cooldown) x (HP / (1 - Armor)) = DPS x Effective HP
    /// </summary>
    public float PowerScore => DPS * EffectiveHP;

    /// <summary>
    /// Threat Score = PowerScore x Range Mult x (1 + 0.10 x (Speed - 2.0))
    /// </summary>
    public float ThreatScore => PowerScore * rangeMultiplier * (1f + 0.10f * (speed - 2.0f));

    /// <summary>
    /// Rasio Threat terhadap Cost
    /// </summary>
    public float ThreatPerCost => cost > 0 ? ThreatScore / cost : 0f;

    /// <summary>
    /// Reward Gold musuh saat dikalahkan = Cost * 0.4 (PDF Bagian A)
    /// </summary>
    public int RewardGold => Mathf.Max(1, Mathf.RoundToInt(cost * 0.4f));

    private void OnValidate()
    {
        if (maxHP < 1f) maxHP = 1f;
        if (damage < 0f) damage = 0f;
        if (speed < 0f) speed = 0f;
        if (cooldown < 0.05f) cooldown = 0.05f;
        if (armor < 0f) armor = 0f;
        if (armor >= 1f) armor = 0.99f;
        if (cost < 1) cost = 1;
        if (rangeMultiplier < 0.1f) rangeMultiplier = 0.1f;
    }
}

