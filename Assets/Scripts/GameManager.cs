using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Inventory UI")]
    public Image slotImage;   // Ana slot image (parent)
    private bool slotFull = false;
    public Sprite nullSlot;

    [Header("Slotta tutulan item referansı")]
    public GameObject heldItem;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadScene(0);
        }
    }

    public void AddToSlot(GameObject item)
    {
        if (item == null) return;

        if (slotFull && heldItem == null)
            slotFull = false;

        if (slotFull)
        {
            Debug.Log("Slot dolu, yeni item eklenemiyor.");
            return;
        }

        // 🔹 Önce tüm çocuk image'leri kapat
        for (int i = 0; i < slotImage.transform.childCount; i++)
        {
            slotImage.transform.GetChild(i).gameObject.SetActive(false);
        }

        // 🔹 Item id'sine göre doğru image'ı aç
        BodyPart bp = item.GetComponent<BodyPart>();
        if (bp != null && bp.id >= 0 && bp.id < slotImage.transform.childCount)
        {
            slotImage.transform.GetChild(bp.id).gameObject.SetActive(true);
            Debug.Log($"UI slotunda {bp.id} id'li item gösterildi.");
        }
        else
        {
            Debug.LogWarning("Item id geçersiz veya UI slotunda o kadar çocuk yok!");
        }

        heldItem = item;
        slotFull = true;
    }

    public bool IsSlotEmpty() => !slotFull;

    public GameObject GetHeldItem() => heldItem;

    public void ClearSlot()
    {
        // 🔹 Tüm alt görselleri kapat
        for (int i = 0; i < slotImage.transform.childCount; i++)
        {
            slotImage.transform.GetChild(i).gameObject.SetActive(false);
        }

        // 🔹 Boş slot sprite’ı geri koy
        if (slotImage != null)
            slotImage.sprite = nullSlot;

        heldItem = null;
        slotFull = false;
        Debug.Log("Slot temizlendi.");
    }
}
