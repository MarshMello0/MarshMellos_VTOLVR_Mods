using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class MouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public UnityEvent OnClick;
    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    private Color normalColour = Color.white;
    [SerializeField]
    private Color hoveredColour = Color.yellow;
    [SerializeField]
    private Color clickedColour = Color.cyan;
    private Coroutine timer;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        if (text == null)
            Debug.LogError("[MouseOver] Text variable was null on " + gameObject.name);
    }
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (!text)
        {
            Debug.LogError("[MouseOver] Text variable was null on " + gameObject.name);
            return;
        }
        text.color = hoveredColour;
    }
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (timer == null)
            timer = StartCoroutine(ClickColour());
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        text.color = normalColour;
    }

    private IEnumerator ClickColour()
    {
        if (OnClick != null)
            OnClick.Invoke();
        text.color = clickedColour;
        yield return new WaitForSeconds(0.5f);
        text.color = normalColour;
        timer = null;
    }
}
