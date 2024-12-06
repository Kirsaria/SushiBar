using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuIngridients : MonoBehaviour
{
    public RectTransform panel;
    public float duration = 0.5f;
    private Vector2 hiddenPosition;
    private Vector2 visiblePosition;
    private bool isVisible = false;

    private static List<MenuIngridients> openPanels = new List<MenuIngridients>();

    void Start()
    {
        hiddenPosition = new Vector2(panel.anchoredPosition.x, -panel.rect.height);
        visiblePosition = panel.anchoredPosition;
        panel.anchoredPosition = hiddenPosition;
    }

    public void TogglePanel()
    {
        if (isVisible)
        {
            StartCoroutine(SlideToPosition(hiddenPosition));
            openPanels.Remove(this);
        }
        else
        {
            CloseAllOpenPanels();
            StartCoroutine(SlideToPosition(visiblePosition));
            openPanels.Add(this);
        }
        isVisible = !isVisible;
    }

    private void CloseAllOpenPanels()
    {
        for (int i = openPanels.Count - 1; i >= 0; i--)
        {
            var openPanel = openPanels[i];
            if (openPanel != null && openPanel != this)
            {
                openPanel.StartCoroutine(openPanel.SlideToPosition(openPanel.hiddenPosition));
                openPanel.isVisible = false;
            }
            else
            {
                openPanels.RemoveAt(i);
            }
        }
    }

    private IEnumerator SlideToPosition(Vector2 targetPosition)
    {
        float elapsedTime = 0;
        Vector2 startingPosition = panel.anchoredPosition;

        while (elapsedTime < duration)
        {
            panel.anchoredPosition = Vector2.Lerp(startingPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        panel.anchoredPosition = targetPosition;
    }

    private void OnDestroy()
    {
        openPanels.Remove(this);
    }
}