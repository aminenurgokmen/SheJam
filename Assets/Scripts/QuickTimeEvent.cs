using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class QuickTimeEvent : MonoBehaviour
{
    public static QuickTimeEvent instance;

    [Header("UI Elements")]
    public GameObject panel;            // QTE paneli (ana panel)
    public RectTransform arrow;         // İbre (hareket eden parça)
    public RectTransform successZone;   // Başarı bölgesi (ortadaki alan)
    public float speed = 500f;          // İbrenin hızı

    private bool movingRight = true;
    private bool active = false;
    private bool success = false;
    private System.Action<bool> onResult; // Callback

    private void Awake()
    {
        instance = this;
        panel.SetActive(false);
    }

    private void Update()
    {
        if (!active) return;

        MoveArrow();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckSuccess();
        }
    }

    void MoveArrow()
    {
        float step = speed * Time.deltaTime;
        arrow.anchoredPosition += new Vector2(movingRight ? step : -step, 0);

        float halfWidth = panel.GetComponent<RectTransform>().rect.width / 2 - 10;

        if (arrow.anchoredPosition.x > halfWidth)
            movingRight = false;
        else if (arrow.anchoredPosition.x < -halfWidth)
            movingRight = true;
    }

    void CheckSuccess()
    {
        float arrowX = arrow.anchoredPosition.x;
        float successMin = successZone.anchoredPosition.x - successZone.rect.width / 2;
        float successMax = successZone.anchoredPosition.x + successZone.rect.width / 2;

        success = arrowX >= successMin && arrowX <= successMax;

        panel.SetActive(false);
        active = false;

        onResult?.Invoke(success);
    }

    public void StartQTE(System.Action<bool> callback)
    {
        panel.SetActive(true);
        arrow.anchoredPosition = new Vector2(-panel.GetComponent<RectTransform>().rect.width / 2 + 10, 0);
        movingRight = true;
        active = true;
        onResult = callback;

        // 🎯 Başarı bölgesini rastgele konumlandır (-200 ile 230 arasında)
        float randomX = Random.Range(-200f, 230f);
        successZone.anchoredPosition = new Vector2(randomX, successZone.anchoredPosition.y);

        Debug.Log($"Yeni success zone konumu: {randomX}");
    }
}
