using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private RectTransform openPos, closePos;
    [SerializeField]
    private RectTransform rectTransform;
    [SerializeField]
    private AnimationCurve movementCurve;
    [SerializeField]
    private float time;
    private bool uiOpen;
    
    public void ToggleUI()
    {
        StartCoroutine(Toggle());
    }

    private IEnumerator Toggle()
    {
        uiOpen = !uiOpen;
        float t = 0f;
        if (uiOpen)
        {
            while (t < 1f)
            {
                t += Time.deltaTime / time;
                rectTransform.position = new Vector3(Mathf.Lerp(closePos.position.x,
                                                                openPos.position.x,
                                                                movementCurve.Evaluate(t)),
                                                    rectTransform.position.y,
                                                    rectTransform.position.z);
                yield return new WaitForEndOfFrame();
            }

        }
        else
        {
            while (t < 1f)
            {
                t += Time.deltaTime / time;
                rectTransform.position = new Vector3(Mathf.Lerp(openPos.position.x,
                                                                closePos.position.x,
                                                                movementCurve.Evaluate(t)),
                                                    rectTransform.position.y,
                                                    rectTransform.position.z);
                yield return new WaitForEndOfFrame();
            }

        }
    }
}
