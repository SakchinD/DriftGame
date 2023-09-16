using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    public event Action<Color> OnColorChange;

    [SerializeField] Button colorSpaceButton;
    [SerializeField] Image colorSpace;
    [SerializeField] Slider valueSlider;

    private Color currentColor;
    public Color CurrentColor => currentColor;

    public void Init()
    {
        currentColor = Color.white;

        colorSpace.rectTransform.anchoredPosition = Vector2.one * 0.5f;
        colorSpaceButton.onClick.AddListener(OnColorSpaceClicked);
        valueSlider.value = valueSlider.maxValue;
        valueSlider.onValueChanged.AddListener(OnValueSliderChanged);
    }

    private void OnDestroy()
    {
        colorSpaceButton.onClick.RemoveAllListeners();
        valueSlider.onValueChanged.RemoveAllListeners();
    }

    private void OnColorSpaceClicked()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(colorSpace.rectTransform, Input.mousePosition, null, out localPoint);
        float hue = localPoint.x / colorSpace.rectTransform.rect.width + 0.5f;
        float saturation = localPoint.y / colorSpace.rectTransform.rect.height + 0.5f;

        currentColor = Color.HSVToRGB(hue, saturation, valueSlider.value);

        colorSpace.color = currentColor;
        OnColorChange?.Invoke(currentColor);
    }

    private void OnValueSliderChanged(float value)
    {
        Color.RGBToHSV(currentColor, out float h, out float s, out _);
        currentColor = Color.HSVToRGB(h, s, value);

        colorSpace.color = currentColor;
        OnColorChange?.Invoke(currentColor);
    }
}
