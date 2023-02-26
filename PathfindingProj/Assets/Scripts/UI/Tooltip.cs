using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance { get; private set; }

    [SerializeField] private RectTransform canvasRectTransform;

    private RectTransform backgroundRect;
    private TextMeshProUGUI textMeshPro;
    private RectTransform rectTransform;

    private void Awake()
    {
        Instance = this;
          
        backgroundRect = transform.Find("Background").GetComponent<RectTransform>();
        textMeshPro = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        rectTransform = transform.GetComponent<RectTransform>();


        HideTooltip();
    }

    private void SetText(string tooltipText)
    {
        textMeshPro.SetText(tooltipText);
        textMeshPro.ForceMeshUpdate();


        Vector2 textSize = textMeshPro.GetRenderedValues(false);
        Vector2 paddingSize = new Vector2(10, 10);

        backgroundRect.sizeDelta = textSize + paddingSize;
    }

    private void Update()
    {
        Vector2 anchoredPos = Input.mousePosition / canvasRectTransform.localScale.x;

        if (anchoredPos.x + backgroundRect.rect.width > canvasRectTransform.rect.width)
        {
            //  stop tooltip from leaving the right side of the screen
            anchoredPos.x = canvasRectTransform.rect.width - backgroundRect.rect.width;
        }

        if (anchoredPos.y + backgroundRect.rect.height > canvasRectTransform.rect.height)
        {
            //  stop tooltip from leaving the top of the screen
            anchoredPos.y = canvasRectTransform.rect.height - backgroundRect.rect.height;
        }

        rectTransform.anchoredPosition = anchoredPos;
    }

    private void ShowTooltip(string tooltipText)
    {
        gameObject.SetActive(true);
        SetText(tooltipText);
    }

    private void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    public static void ShowTooltip_Static(string tooltipText)
    {
        Instance.ShowTooltip(tooltipText);
    }

    public static void HideTooltip_Static()
    {
        Instance.HideTooltip();
    }
}
