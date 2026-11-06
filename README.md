# Fortress of Nations — Game Balancing Demo (Bagian B)

Tugas 4 Bagian B — Unity Editor Dasar: ScriptableObject = Live Sheet & Singleton CoinManager.  
Mata Kuliah: **Desain dan Teknologi Game**  
Program Studi Teknik Informatika, Fakultas Sains dan Teknologi, Universitas Muhammadiyah Bandung.

---

## 👥 Tim Penyusun
- **Adam Bani Khairil Anam** — 230102006
- **Azril Rojaan Fauzi** — 230102029
- **Haidir Ahmad Gumelar** — 230102054
- **Naufal Arizal Fiqri** — 230102101

---

## 🔗 Referensi & Sumber Data
- **Live Google Sheet Balancing:**  
  [https://docs.google.com/spreadsheets/d/1oObj3NGXCmbAo_t6m97vbX-hcW89RPfqdkncT2mRHB8/edit?usp=sharing](https://docs.google.com/spreadsheets/d/1oObj3NGXCmbAo_t6m97vbX-hcW89RPfqdkncT2mRHB8/edit?usp=sharing)
- **Arsitektur Singleton:** Menard Chapter 2 (Game Architecture & Economy Management).

---

## 🎯 Fitur & Implementasi

### 1. ScriptableObject = Live Sheet (`EnemyStats`)
Menerapkan formula balancing otomatis langsung di Inspector Unity:
- $\text{DPS} = \frac{\text{Damage}}{\text{Cooldown}}$
- $\text{Effective HP} = \frac{\text{HP}}{1 - \text{Armor}}$
- $\text{PowerScore} = \text{DPS} \times \text{Effective HP}$
- $\text{Threat Score} = \text{PowerScore} \times \text{Range Mult} \times (1 + 0.10 \times (\text{Speed} - 2.0))$
- $\text{Threat per Cost} = \frac{\text{Threat Score}}{\text{Cost}}$
- $\text{Reward Gold} = \text{Cost} \times 0.4$

Tiga ScriptableObject tersimpan di `Assets/ScriptableObjects/`:
1. **Infantry Scout**: HP 60, Dmg 6, Spd 3, CD 1, Armor 5%, Cost 14 $\rightarrow$ Threat 417, Thr/Cost 29.8, Reward +6 G.
2. **Armored Carrier**: HP 240, Dmg 18, Spd 1, CD 2, Armor 40%, Cost 115 $\rightarrow$ Threat 3,564, Thr/Cost 31.0, Reward +46 G.
3. **Attack Drone**: HP 80, Dmg 10, Spd 2.5, CD 1.5, Armor 10%, Cost 28 $\rightarrow$ Threat 809, Thr/Cost 28.9, Reward +11 G.

### 2. Singleton CoinManager (`CoinManager.cs`)
- Mengelola saldo koin terpusat melalui `CoinManager.Instance`.
- Memperbarui tampilan UI Text (`Coins: X G`) secara reaktif saat koin bertambah atau dibelanjakan.
- Menyediakan method `AddCoins()`, `SpendCoins()`, dan `ResetCoins()`.

### 3. Tiga Prefab Enemy (`Assets/Prefabs/`)
- Masing-masing prefab mengikat data dari SO bersangkutan dan memiliki floating billboard UI.
- Saat musuh menerima damage, dihitung terhadap armor: $\text{Actual Damage} = \text{Raw Damage} \times (1 - \text{Armor})$.
- Saat musuh mati, memanggil `CoinManager.Instance.AddCoins(stats.RewardGold)`.

### 4. Interactive Test Scene (`Assets/Scenes/SampleScene.unity`)
- Menampilkan visual 3D ketiga tipe musuh di atas arena.
- Dilengkapi tombol interaktif untuk mengalahkan musuh, menguji reward koin, simulasi pembelian upgrade, serta reset demo.

---

## 📦 Deliverables
- **UnityPackage:** [`FortressOfNations_BagianB.unitypackage`](FortressOfNations_BagianB.unitypackage)
- **Screenshots:** Terletak di folder [`Assets/Screenshots/`](Assets/Screenshots/)

