using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextScroller : MonoBehaviour
{
    public Text text;
    public float textSpeed = 10f;

    private RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    IEnumerator ScrollText()
    {
        float startY = rectTransform.anchoredPosition.y;
        float endY = rectTransform.rect.height;

        while (rectTransform.anchoredPosition.y < endY)
        {
            rectTransform.anchoredPosition += textSpeed * Vector2.down * Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.y, startY);
    }
    private void Update()
    {
        ScrollText();
    }
}
