using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public float ScalingFactor;

    private RectTransform rectTransform;

    private RectTransform bar;
    private TextMeshProUGUI text;
    private List<RectTransform> bars;

    private PlayerController player;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        bar = transform.Find("Range").GetComponent<RectTransform>();
        bars = new List<RectTransform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.name.StartsWith("Overheal ")) bars.Add(child.GetComponent<RectTransform>());
        }
        bars.Sort((a, b) => int.Parse(a.name.Substring(9)).CompareTo(int.Parse(b.name.Substring(9))));
        bars.Insert(0, bar);

        player = FindObjectOfType<PlayerController>();
        text = bar.Find("Display").GetComponent<TextMeshProUGUI>();

        Tick(true);
    }

    private float prevHealth;
    private void Update() => Tick(false);
    private void Tick(bool instant)
    {
        float size = Mathf.Log10(player.BaseStats.MaxHealth) * ScalingFactor;
        float desiredHealth = player.CurrentHealth;
        float ratio = desiredHealth / player.BaseStats.MaxHealth;

        if (!instant)
        {
            desiredHealth = Mathf.Lerp(prevHealth, desiredHealth, Time.deltaTime * 5);
            ratio = desiredHealth / player.BaseStats.MaxHealth;
        }

        for (int i = 0; i < bars.Count; i++)
        {
            float thisPart = Mathf.Clamp01(ratio);
            RectTransform bar = bars[i];
            rectTransform.sizeDelta = new Vector2(size, rectTransform.sizeDelta.y);
            bar.sizeDelta = new Vector2((thisPart - 1) * size, 0);
            bar.localPosition = new Vector2(0.5f * size * thisPart, bar.localPosition.y);
            ratio--;
        }
        text.text = Mathf.RoundToInt(desiredHealth).ToString();

        prevHealth = desiredHealth;
    }
}
