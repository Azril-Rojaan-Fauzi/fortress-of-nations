using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Singleton CoinManager sesuai arsitektur Menard Ch. 2.
/// Bertanggung jawab menyimpan data ekonomi koin dan memperbarui UI Text secara reaktif.
/// </summary>
public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    [Header("Economy Data")]
    [SerializeField] private int currentCoins = 0;
    public int CurrentCoins => currentCoins;

    [Header("UI References (Menard Ch. 2)")]
    [Tooltip("Komponen TextMeshProUGUI untuk menampilkan koin")]
    public TMP_Text coinTextTMP;

    [Tooltip("Komponen Legacy UI Text sebagai fallback")]
    public Text coinTextLegacy;

    [Header("UI Formatting")]
    [SerializeField] private string prefix = "Coins: ";
    [SerializeField] private string suffix = " G";

    public event Action<int> OnCoinsChanged;

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        UpdateUI();
    }

    /// <summary>
    /// Menambah koin saat musuh dikalahkan
    /// </summary>
    public void AddCoins(int amount)
    {
        if (amount <= 0) return;
        currentCoins += amount;
        UpdateUI();
        OnCoinsChanged?.Invoke(currentCoins);
        Debug.Log($"[CoinManager] +{amount} Gold! Total sekarang: {currentCoins} G");
    }

    /// <summary>
    /// Mengurangi koin untuk pembelian upgrade / riset
    /// </summary>
    public bool SpendCoins(int amount)
    {
        if (amount <= 0) return false;

        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            UpdateUI();
            OnCoinsChanged?.Invoke(currentCoins);
            Debug.Log($"[CoinManager] -{amount} Gold dibelanjakan. Sisa koin: {currentCoins} G");
            return true;
        }

        Debug.LogWarning($"[CoinManager] Koin tidak mencukupi! Butuh {amount} G, saldo saat ini: {currentCoins} G");
        return false;
    }

    public void ResetCoins(int initial = 0)
    {
        currentCoins = Mathf.Max(0, initial);
        UpdateUI();
        OnCoinsChanged?.Invoke(currentCoins);
        Debug.Log($"[CoinManager] Koin direset ke: {currentCoins} G");
    }

    /// <summary>
    /// Sinkronisasi teks UI dengan nilai koin terkini
    /// </summary>
    public void UpdateUI()
    {
        string text = $"{prefix}{currentCoins}{suffix}";

        if (coinTextTMP != null)
        {
            coinTextTMP.text = text;
        }

        if (coinTextLegacy != null)
        {
            coinTextLegacy.text = text;
        }
    }
}

