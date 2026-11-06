using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

[InitializeOnLoad]
public static class FortressSetupEditor
{
    private const string SO_FOLDER = "Assets/ScriptableObjects";
    private const string PREFAB_FOLDER = "Assets/Prefabs";
    private const string MATERIAL_FOLDER = "Assets/Materials";
    private const string SCREENSHOT_FOLDER = "Assets/Screenshots";
    private const string SCENE_PATH = "Assets/Scenes/SampleScene.unity";

    static FortressSetupEditor()
    {
        EditorApplication.delayCall += CheckAndRunFirstTime;
    }

    private static void CheckAndRunFirstTime()
    {
        string marker = "Library/FortressSetupDone_v3.marker";
        if (!File.Exists(marker))
        {
            File.WriteAllText(marker, "Setup initiated v3");
            BuildEverything();
        }
    }

    [MenuItem("Fortress of Nations/Build Complete Project (Scene + Prefabs + SO)", false, 1)]
    public static void BuildEverything()
    {
        Debug.Log("<color=cyan><b>[FortressSetup] Memulai build otomatis proyek Fortress of Nations...</b></color>");

        try
        {
            TMPro.TMP_PackageResourceImporter.ImportResources(true, false, false);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("[FortressSetup] TMP Import warning: " + ex.Message);
        }

        EnsureDirectories();
        EnemyStats scoutSO = CreateScoutSO();
        EnemyStats carrierSO = CreateCarrierSO();
        EnemyStats droneSO = CreateDroneSO();
        AssetDatabase.SaveAssets();

        Material matGround = GetOrCreateMaterial("Mat_Ground", new Color(0.18f, 0.22f, 0.26f));
        Material matScout = GetOrCreateMaterial("Mat_InfantryScout", scoutSO.themeColor);
        Material matCarrier = GetOrCreateMaterial("Mat_ArmoredCarrier", carrierSO.themeColor);
        Material matDrone = GetOrCreateMaterial("Mat_AttackDrone", droneSO.themeColor);

        GameObject scoutPrefab = CreateEnemyPrefab("Enemy_InfantryScout", scoutSO, matScout, PrimitiveType.Capsule, new Vector3(0.9f, 1.3f, 0.9f), 1.55f);
        GameObject carrierPrefab = CreateEnemyPrefab("Enemy_ArmoredCarrier", carrierSO, matCarrier, PrimitiveType.Cube, new Vector3(2.2f, 1.2f, 2.2f), 1.55f);
        GameObject dronePrefab = CreateEnemyPrefab("Enemy_AttackDrone", droneSO, matDrone, PrimitiveType.Sphere, new Vector3(1.4f, 0.45f, 1.4f), 1.15f);

        SetupScene(scoutPrefab, carrierPrefab, dronePrefab, matGround);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        CaptureAllScreenshots();
        ExportUnityPackage();

        Debug.Log("<color=green><b>[FortressSetup] SEMUA ASSET, PREFAB, SO, SCENE, SCREENSHOT, DAN UNITYPACKAGE TELAH SELESAI DIBUAT!</b></color>");
    }

    private static void EnsureDirectories()
    {
        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects"))
            AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
            AssetDatabase.CreateFolder("Assets", "Materials");
        if (!AssetDatabase.IsValidFolder("Assets/Screenshots"))
            AssetDatabase.CreateFolder("Assets", "Screenshots");
    }

    private static Material GetOrCreateMaterial(string matName, Color col)
    {
        string path = $"{MATERIAL_FOLDER}/{matName}.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            mat = new Material(shader);
            mat.color = col;
            AssetDatabase.CreateAsset(mat, path);
        }
        else
        {
            mat.color = col;
            EditorUtility.SetDirty(mat);
        }
        return mat;
    }

    private static EnemyStats CreateScoutSO()
    {
        string path = $"{SO_FOLDER}/SO_Enemy_InfantryScout.asset";
        EnemyStats so = AssetDatabase.LoadAssetAtPath<EnemyStats>(path);
        if (so == null)
        {
            so = ScriptableObject.CreateInstance<EnemyStats>();
            AssetDatabase.CreateAsset(so, path);
        }

        so.enemyName = "Infantry Scout";
        so.description = "Unit darat cepat dan murah. Ancaman utama bersumber dari jumlah kawanan (swarm).";
        so.themeColor = new Color(0.18f, 0.8f, 0.44f);
        so.maxHP = 60f;
        so.damage = 6f;
        so.speed = 3.0f;
        so.cooldown = 1.0f;
        so.armor = 0.05f;
        so.cost = 14;
        so.rangeMultiplier = 1.0f;

        EditorUtility.SetDirty(so);
        return so;
    }

    private static EnemyStats CreateCarrierSO()
    {
        string path = $"{SO_FOLDER}/SO_Enemy_ArmoredCarrier.asset";
        EnemyStats so = AssetDatabase.LoadAssetAtPath<EnemyStats>(path);
        if (so == null)
        {
            so = ScriptableObject.CreateInstance<EnemyStats>();
            AssetDatabase.CreateAsset(so, path);
        }

        so.enemyName = "Armored Carrier";
        so.description = "Unit pembawa lapis baja tebal berdaya tahan masif. Dihargai cost tinggi karena armor dan HP besar.";
        so.themeColor = new Color(0.9f, 0.45f, 0.15f);
        so.maxHP = 240f;
        so.damage = 18f;
        so.speed = 1.0f;
        so.cooldown = 2.0f;
        so.armor = 0.40f;
        so.cost = 115;
        so.rangeMultiplier = 1.1f;

        EditorUtility.SetDirty(so);
        return so;
    }

    private static EnemyStats CreateDroneSO()
    {
        string path = $"{SO_FOLDER}/SO_Enemy_AttackDrone.asset";
        EnemyStats so = AssetDatabase.LoadAssetAtPath<EnemyStats>(path);
        if (so == null)
        {
            so = ScriptableObject.CreateInstance<EnemyStats>();
            AssetDatabase.CreateAsset(so, path);
        }

        so.enemyName = "Attack Drone";
        so.description = "Unit penyerang udara berkecepatan tinggi dengan jangkauan serang superior.";
        so.themeColor = new Color(0.2f, 0.65f, 1.0f);
        so.maxHP = 80f;
        so.damage = 10f;
        so.speed = 2.5f;
        so.cooldown = 1.5f;
        so.armor = 0.10f;
        so.cost = 28;
        so.rangeMultiplier = 1.3f;

        EditorUtility.SetDirty(so);
        return so;
    }

    private static GameObject CreateEnemyPrefab(string prefabName, EnemyStats stats, Material mat, PrimitiveType primType, Vector3 scale, float billboardOffsetY)
    {
        string path = $"{PREFAB_FOLDER}/{prefabName}.prefab";

        GameObject root = new GameObject(prefabName);

        GameObject model = GameObject.CreatePrimitive(primType);
        model.name = "Model";
        model.transform.SetParent(root.transform);
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;
        model.transform.localScale = scale;
        model.GetComponent<MeshRenderer>().sharedMaterial = mat;

        EnemyController controller = root.AddComponent<EnemyController>();
        controller.stats = stats;
        controller.bodyRenderer = model.GetComponent<MeshRenderer>();

        GameObject billboardGO = new GameObject("BillboardUI");
        billboardGO.transform.SetParent(root.transform);
        billboardGO.transform.localPosition = new Vector3(0, billboardOffsetY, 0);

        TextMeshPro tmpro = billboardGO.AddComponent<TextMeshPro>();
        tmpro.alignment = TextAlignmentOptions.Center;
        tmpro.fontSize = 2.3f;
        tmpro.color = Color.white;
        tmpro.textWrappingMode = TextWrappingModes.NoWrap;
        tmpro.lineSpacing = 12f;

        string hpColorHex = ColorUtility.ToHtmlStringRGB(new Color(0.3f, 0.9f, 0.45f));
        tmpro.text = $"<b><size=125%>{stats.enemyName}</size></b>\n" +
                     $"HP: <color=#{hpColorHex}>{stats.maxHP:F0}/{stats.maxHP:F0}</color> | Armor: <b>{stats.armor * 100:F0}%</b>\n" +
                     $"Spd: <b>{stats.speed}</b> | Cost: <b>{stats.cost}</b> | Reward: <color=#F1C40F>+{stats.RewardGold}G</color>";

        controller.billboardText = tmpro;

        GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        GameObject.DestroyImmediate(root);

        return savedPrefab;
    }

    private static void SetupScene(GameObject scoutPrefab, GameObject carrierPrefab, GameObject dronePrefab, Material matGround)
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.transform.position = new Vector3(0.3f, 3.2f, -8.6f);
            cam.transform.rotation = Quaternion.Euler(14f, 0f, 0f);
            cam.fieldOfView = 48f;
            cam.backgroundColor = new Color(0.10f, 0.12f, 0.16f);
            cam.clearFlags = CameraClearFlags.SolidColor;
        }

        Light light = Object.FindFirstObjectByType<Light>();
        if (light != null)
        {
            light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            light.color = new Color(1f, 0.98f, 0.92f);
            light.intensity = 1.15f;
        }

        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(2.5f, 1f, 1.5f);
        ground.GetComponent<MeshRenderer>().sharedMaterial = matGround;

        GameObject enemiesParent = new GameObject("--- ENEMIES ---");

        GameObject scoutObj = (GameObject)PrefabUtility.InstantiatePrefab(scoutPrefab, enemiesParent.transform);
        scoutObj.name = "Enemy_InfantryScout";
        scoutObj.transform.position = new Vector3(-3.0f, 0.65f, 0f);

        GameObject carrierObj = (GameObject)PrefabUtility.InstantiatePrefab(carrierPrefab, enemiesParent.transform);
        carrierObj.name = "Enemy_ArmoredCarrier";
        carrierObj.transform.position = new Vector3(0.4f, 0.6f, 0f);

        GameObject droneObj = (GameObject)PrefabUtility.InstantiatePrefab(dronePrefab, enemiesParent.transform);
        droneObj.name = "Enemy_AttackDrone";
        droneObj.transform.position = new Vector3(3.7f, 2.0f, 0f);

        GameObject managersParent = new GameObject("--- MANAGERS ---");

        GameObject coinManagerGO = new GameObject("CoinManager");
        coinManagerGO.transform.SetParent(managersParent.transform);
        CoinManager coinManager = coinManagerGO.AddComponent<CoinManager>();

        GameObject gmUIGO = new GameObject("GameManagerUI");
        gmUIGO.transform.SetParent(managersParent.transform);
        GameManagerUI gmUI = gmUIGO.AddComponent<GameManagerUI>();
        gmUI.scoutEnemy = scoutObj.GetComponent<EnemyController>();
        gmUI.carrierEnemy = carrierObj.GetComponent<EnemyController>();
        gmUI.droneEnemy = droneObj.GetComponent<EnemyController>();

        CreateCanvasUI(coinManager, gmUI, cam);

        EditorSceneManager.SaveScene(scene, SCENE_PATH);

        EditorBuildSettingsScene[] buildScenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene(SCENE_PATH, true)
        };
        EditorBuildSettings.scenes = buildScenes;
    }

    private static void CreateCanvasUI(CoinManager coinManager, GameManagerUI gmUI, Camera cam)
    {
        GameObject canvasGO = new GameObject("UI_Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = cam;
        canvas.planeDistance = 5f;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject esGO = new GameObject("EventSystem");
            esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGO.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        // Top Header Banner
        GameObject headerGO = CreateUIPanel(canvasGO.transform, "HeaderPanel", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -45), new Vector2(1840, 70), new Color(0.08f, 0.1f, 0.14f, 0.92f));
        CreateUIText(headerGO.transform, "TitleText", "FORTRESS OF NATIONS — GAME BALANCING DEMO (MENARD CH. 2)", 24, TextAlignmentOptions.Left, TextStyles.Bold, new Vector2(30, 0), new Vector2(1200, 50), Color.white);

        // Coin Display Box (Top Right)
        GameObject coinBox = CreateUIPanel(headerGO.transform, "CoinDisplayBox", new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-150, 0), new Vector2(260, 52), new Color(0.18f, 0.14f, 0.04f, 0.95f));
        TMP_Text coinTMP = CreateUIText(coinBox.transform, "CoinText", "Coins: 0 G", 26, TextAlignmentOptions.Center, TextStyles.Bold, Vector2.zero, new Vector2(250, 48), new Color(1f, 0.85f, 0.2f));
        coinManager.coinTextTMP = coinTMP;

        // Subtitle Info Bar
        GameObject subHeader = CreateUIPanel(canvasGO.transform, "SubHeaderPanel", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -96), new Vector2(1840, 30), new Color(0.12f, 0.15f, 0.2f, 0.75f));
        CreateUIText(subHeader.transform, "SubTitle", "Implementasi ScriptableObject = Live Sheet • Singleton CoinManager + UI Text • 3 Tipe Enemy Sesuai Dokumen Balancing", 14, TextAlignmentOptions.Left, TextStyles.Normal, new Vector2(30, 0), new Vector2(1600, 26), new Color(0.75f, 0.85f, 0.95f));

        // Bottom Action Controls Panel
        GameObject controlsPanel = CreateUIPanel(canvasGO.transform, "ControlsPanel", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, 95), new Vector2(1840, 140), new Color(0.08f, 0.1f, 0.14f, 0.94f));

        CreateUIText(controlsPanel.transform, "ControlsLabel", "TEST CONTROLS (Simulasi Kemenangan & Interaksi Ekonomi):", 15, TextAlignmentOptions.Left, TextStyles.Bold, new Vector2(30, 42), new Vector2(800, 26), new Color(0.2f, 0.8f, 1f));

        // Buttons
        gmUI.btnScout = CreateButton(controlsPanel.transform, "Btn_Scout", "Kalahkan Scout (+6 G)", new Vector2(-460, -10), new Vector2(240, 48), new Color(0.12f, 0.5f, 0.28f));
        gmUI.btnCarrier = CreateButton(controlsPanel.transform, "Btn_Carrier", "Kalahkan Carrier (+46 G)", new Vector2(-200, -10), new Vector2(250, 48), new Color(0.75f, 0.35f, 0.1f));
        gmUI.btnDrone = CreateButton(controlsPanel.transform, "Btn_Drone", "Kalahkan Drone (+11 G)", new Vector2(60, -10), new Vector2(240, 48), new Color(0.15f, 0.45f, 0.8f));
        gmUI.btnSpend = CreateButton(controlsPanel.transform, "Btn_Spend", "Beli Upgrade (50 G)", new Vector2(300, -10), new Vector2(210, 48), new Color(0.55f, 0.2f, 0.65f));
        gmUI.btnReset = CreateButton(controlsPanel.transform, "Btn_Reset", "Reset Demo", new Vector2(510, -10), new Vector2(170, 48), new Color(0.35f, 0.38f, 0.42f));

        // Status Feedback Log Bar
        GameObject logBox = CreateUIPanel(canvasGO.transform, "LogPanel", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, 14), new Vector2(1840, 28), new Color(0.05f, 0.06f, 0.08f, 0.95f));
        TMP_Text logTMP = CreateUIText(logBox.transform, "LogText", "Status: Sistem siap. Klik tombol aksi di atas untuk menguji flow reward koin.", 14, TextAlignmentOptions.Left, TextStyles.Normal, new Vector2(20, 0), new Vector2(1800, 24), new Color(0.4f, 1f, 0.6f));
        gmUI.statusLogText = logTMP;

        // Left Live Sheet Balancing Overlay Card
        GameObject infoCard = CreateUIPanel(canvasGO.transform, "BalancingTablePanel", new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(170, 20), new Vector2(290, 310), new Color(0.06f, 0.08f, 0.12f, 0.88f));
        CreateUIText(infoCard.transform, "CardTitle", "DATA BALANCING (SHEET)", 15, TextAlignmentOptions.Left, TextStyles.Bold, new Vector2(12, 130), new Vector2(265, 26), new Color(1f, 0.8f, 0.2f));

        string sheetSummary =
            "1. Infantry Scout\n" +
            "   HP: 60 | Dmg: 6 | Spd: 3\n" +
            "   Cost: 14 | Reward: +6 G\n" +
            "   Threat: 417 (Thr/Cost: 29.8)\n\n" +
            "2. Armored Carrier\n" +
            "   HP: 240 | Dmg: 18 | Arm: 40%\n" +
            "   Cost: 115 | Reward: +46 G\n" +
            "   Threat: 3,564 (Thr/Cost: 31.0)\n\n" +
            "3. Attack Drone\n" +
            "   HP: 80 | Dmg: 10 | Spd: 2.5\n" +
            "   Cost: 28 | Reward: +11 G\n" +
            "   Threat: 809 (Thr/Cost: 28.9)\n\n" +
            "<color=#2ECC71>Deviasi max 3.7% (< 15% toleransi)</color>";

        CreateUIText(infoCard.transform, "CardBody", sheetSummary, 11, TextAlignmentOptions.TopLeft, TextStyles.Normal, new Vector2(12, -18), new Vector2(265, 235), Color.white);
    }

    private static GameObject CreateUIPanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 size, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);

        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;

        Image img = panel.AddComponent<Image>();
        img.color = color;
        return panel;
    }

    private static TMP_Text CreateUIText(Transform parent, string name, string content, float fontSize, TextAlignmentOptions alignment, TextStyles style, Vector2 anchoredPos, Vector2 size, Color color)
    {
        GameObject textGO = new GameObject(name);
        textGO.transform.SetParent(parent, false);

        RectTransform rt = textGO.AddComponent<RectTransform>();
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;

        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = content;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        if (style == TextStyles.Bold) tmp.fontStyle = FontStyles.Bold;
        tmp.color = color;
        return tmp;
    }

    private static Button CreateButton(Transform parent, string name, string label, Vector2 anchoredPos, Vector2 size, Color bgColor)
    {
        GameObject btnGO = new GameObject(name);
        btnGO.transform.SetParent(parent, false);

        RectTransform rt = btnGO.AddComponent<RectTransform>();
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;

        Image img = btnGO.AddComponent<Image>();
        img.color = bgColor;

        Button btn = btnGO.AddComponent<Button>();
        btn.targetGraphic = img;

        ColorBlock cb = btn.colors;
        cb.highlightedColor = bgColor * 1.25f;
        cb.pressedColor = bgColor * 0.8f;
        btn.colors = cb;

        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(btnGO.transform, false);

        RectTransform textRT = textGO.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.sizeDelta = Vector2.zero;

        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 14;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        return btn;
    }

    [MenuItem("Fortress of Nations/Capture Screenshots", false, 3)]
    public static void CaptureAllScreenshots()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        CoinManager cm = Object.FindFirstObjectByType<CoinManager>();
        GameManagerUI gm = Object.FindFirstObjectByType<GameManagerUI>();

        // 1. Screenshot Initial State
        if (cm != null) cm.ResetCoins(0);
        if (gm != null && gm.statusLogText != null)
            gm.statusLogText.text = "Status: Demo siap. Klik tombol aksi di bawah untuk menguji flow reward koin.";

        CaptureCameraToFile(cam, $"{SCREENSHOT_FOLDER}/01_GameView_Initial.png");

        // 2. Screenshot Defeating Carrier (+46 G) & Scout (+6 G) -> 52 G
        if (cm != null) cm.SetCoins(52);
        if (gm != null && gm.statusLogText != null)
            gm.statusLogText.text = "[Aksi] Armored Carrier (+46 G) & Infantry Scout (+6 G) dikalahkan! Total Gold: 52 G";

        CaptureCameraToFile(cam, $"{SCREENSHOT_FOLDER}/02_GameView_Action_52Coins.png");

        // Reset back to 0
        if (cm != null) cm.ResetCoins(0);
        if (gm != null && gm.statusLogText != null)
            gm.statusLogText.text = "Status: Sistem siap. Tekan tombol aksi di atas untuk menguji flow reward koin.";

        Debug.Log("<color=green><b>[FortressSetup] Screenshots Game View berhasil diambil!</b></color>");
    }

    private static void CaptureCameraToFile(Camera cam, string filePath)
    {
        int width = 1920;
        int height = 1080;
        RenderTexture rt = new RenderTexture(width, height, 24);
        RenderTexture prevRT = cam.targetTexture;

        cam.targetTexture = rt;
        cam.Render();

        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenShot.Apply();

        cam.targetTexture = prevRT;
        RenderTexture.active = null;
        Object.DestroyImmediate(rt);

        byte[] bytes = screenShot.EncodeToPNG();
        Object.DestroyImmediate(screenShot);

        File.WriteAllBytes(filePath, bytes);
    }

    [MenuItem("Fortress of Nations/Export UnityPackage", false, 2)]
    public static void ExportUnityPackage()
    {
        string[] assetPaths = new string[]
        {
            "Assets/Scripts",
            "Assets/Editor",
            "Assets/ScriptableObjects",
            "Assets/Prefabs",
            "Assets/Materials",
            "Assets/Screenshots",
            "Assets/TextMesh Pro",
            "Assets/Scenes/SampleScene.unity"
        };

        string exportFile = "FortressOfNations_BagianB.unitypackage";
        AssetDatabase.ExportPackage(assetPaths, exportFile, ExportPackageOptions.Recurse);
        Debug.Log($"<color=green><b>[FortressSetup] UnityPackage berhasil diexport ke: {Path.GetFullPath(exportFile)}</b></color>");
    }

    private enum TextStyles { Normal, Bold }
}
