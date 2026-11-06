using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyStats))]
public class EnemyStatsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EnemyStats stats = (EnemyStats)target;

        // Header Banner
        EditorGUILayout.Space(6);
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 14,
            alignment = TextAnchor.MiddleCenter
        };
        EditorGUILayout.LabelField($"📊 EnemyStats: {stats.enemyName}", headerStyle);
        EditorGUILayout.Space(4);

        // Draw Default Fields with styled layout
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Identitas Musuh", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("enemyName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("description"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("themeColor"));
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(4);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Atribut Dasar (Live Sheet Input)", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxHP"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("damage"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("speed"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cooldown"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("armor"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cost"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rangeMultiplier"));
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();

        // Live Calculated Balancing Box (Menampilkan hasil formula PDF Bagian A)
        EditorGUILayout.Space(8);
        EditorGUILayout.BeginVertical("box");
        GUIStyle boxTitleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            normal = { textColor = new Color(0.2f, 0.7f, 1f) }
        };
        EditorGUILayout.LabelField("⚡ Live Balancing Formulas (PDF Bagian A)", boxTitleStyle);
        EditorGUILayout.LabelField("Formula otomatis terhitung langsung saat nilai atribut diubah:");
        EditorGUILayout.Space(3);

        DrawStatRow("DPS (Dmg / CD):", $"{stats.DPS:F2}");
        DrawStatRow("Effective HP (HP / (1 - Armor)):", $"{stats.EffectiveHP:F1}");
        DrawStatRow("PowerScore (DPS x EffHP):", $"{stats.PowerScore:F1}");
        DrawStatRow("Threat Score:", $"{stats.ThreatScore:F1}");

        // Threat per Cost with status indication
        float tpc = stats.ThreatPerCost;
        string statusText = (tpc >= 25f && tpc <= 35f) ? "✓ Seimbang (Toleransi 15%)" : "Perlu Penyesuaian";
        DrawStatRow("Threat per Cost:", $"{tpc:F1} ({statusText})");

        DrawStatRow("Kill Gold Reward (Cost x 0.4):", $"+{stats.RewardGold} Gold");
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(6);
    }

    private void DrawStatRow(string label, string value)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, GUILayout.Width(220));
        EditorGUILayout.SelectableLabel(value, EditorStyles.boldLabel, GUILayout.Height(18));
        EditorGUILayout.EndHorizontal();
    }
}

