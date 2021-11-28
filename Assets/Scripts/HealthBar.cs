using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Text unitName;
    public Color Low;
    public Color High;
    public Vector3 OffsetHB;
    public Vector3 OffsetName;

    public void setHealth(float health, float maxHealth)
    {
        slider.gameObject.SetActive(health < maxHealth);
        slider.maxValue = maxHealth;
        slider.value = health;
        //Debug.Log(slider.value);

        slider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(Low, High, slider.normalizedValue);
    }

    private void Update()
    {
        slider.transform.position = Camera.main.WorldToScreenPoint(transform.parent.position + OffsetHB);
        if (unitName)
        {
            unitName.transform.position = Camera.main.WorldToScreenPoint(transform.parent.position + OffsetName);
        }

    }
}
