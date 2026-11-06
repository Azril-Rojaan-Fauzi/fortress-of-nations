using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI Controller untuk demonstrasi interaktif Balancing Fortress of Nations.
/// Menghubungkan tombol aksi UI dengan EnemyController dan CoinManager.
/// </summary>
public class GameManagerUI : MonoBehaviour
{
    [Header("Referensi Musuh di Scene")]
    public EnemyController scoutEnemy;
    public EnemyController carrierEnemy;
    public EnemyController droneEnemy;

    [Header("Referensi Tombol UI")]
    public Button btnScout;
    public Button btnCarrier;
    public Button btnDrone;
    public Button btnSpend;
    public Button btnReset;

    [Header("HUD Info")]
    public TMP_Text statusLogText;

    private void Awake()
    {
        // Hubungkan event listener tombol secara runtime
        if (btnScout != null) btnScout.onClick.AddListener(KillScout);
        if (btnCarrier != null) btnCarrier.onClick.AddListener(KillCarrier);
        if (btnDrone != null) btnDrone.onClick.AddListener(KillDrone);
        if (btnSpend != null) btnSpend.onClick.AddListener(() => SpendCoinsTest(50));
        if (btnReset != null) btnReset.onClick.AddListener(ResetAll);
    }

    private void Start()
    {
        LogStatus("Demo siap. Klik tombol untuk mengalahkan musuh atau uji pengurangan koin.");
    }

    public void KillScout()
    {
        if (scoutEnemy != null)
        {
            scoutEnemy.TakeDamage(9999f);
            LogStatus($"[Aksi] Infantry Scout dikalahkan! (+{scoutEnemy.stats.RewardGold} Gold)");
        }
    }

    public void KillCarrier()
    {
        if (carrierEnemy != null)
        {
            carrierEnemy.TakeDamage(9999f);
            LogStatus($"[Aksi] Armored Carrier dikalahkan! (+{carrierEnemy.stats.RewardGold} Gold)");
        }
    }

    public void KillDrone()
    {
        if (droneEnemy != null)
        {
            droneEnemy.TakeDamage(9999f);
            LogStatus($"[Aksi] Attack Drone dikalahkan! (+{droneEnemy.stats.RewardGold} Gold)");
        }
    }

    public void DamageAllEnemies(float damageAmount)
    {
        if (scoutEnemy != null) scoutEnemy.TakeDamage(damageAmount);
        if (carrierEnemy != null) carrierEnemy.TakeDamage(damageAmount);
        if (droneEnemy != null) droneEnemy.TakeDamage(damageAmount);
        LogStatus($"[Aksi] Semua musuh menerima {damageAmount} raw damage.");
    }

    public void SpendCoinsTest(int amount)
    {
        if (CoinManager.Instance != null)
        {
            bool success = CoinManager.Instance.SpendCoins(amount);
            if (success)
            {
                LogStatus($"[Ekonomi] Berhasil membeli upgrade seharga {amount} G!");
            }
            else
            {
                LogStatus($"[Ekonomi] Gagal membeli! Gold tidak mencukupi ({CoinManager.Instance.CurrentCoins}/{amount} G).");
            }
        }
    }

    public void ResetAll()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.ResetCoins(0);
        }

        if (scoutEnemy != null) scoutEnemy.InitializeFromStats();
        if (carrierEnemy != null) carrierEnemy.InitializeFromStats();
        if (droneEnemy != null) droneEnemy.InitializeFromStats();

        LogStatus("[Reset] Seluruh koin dan musuh berhasil direset.");
    }

    private void LogStatus(string message)
    {
        if (statusLogText != null)
        {
            statusLogText.text = message;
        }
        Debug.Log(message);
    }
}
