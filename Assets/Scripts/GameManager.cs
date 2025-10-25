using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Inventory UI")]
    public Image slotImage; // UI Image referansı
    private bool slotFull = false;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void AddToSlot(GameObject item)
    {
        if (slotFull || item == null) return;

        SpriteRenderer sr = item.GetComponent<SpriteRenderer>();
        if (sr != null && slotImage != null)
        {
            slotImage.sprite = sr.sprite;
           // slotImage.color = Color.white; // görünür yap
            slotFull = true;
            Debug.Log("Item slot'a eklendi: " + item.name);
        }

        Destroy(item);
    }

    public bool IsSlotEmpty()
    {
        return !slotFull;
    }

    public void ClearSlot()
    {
        slotImage.sprite = null;
        slotImage.color = new Color(1, 1, 1, 0); // şeffaf
        slotFull = false;
    }
}
