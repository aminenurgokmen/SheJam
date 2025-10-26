using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Inventory UI")]
    public Image slotImage;
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
            //restart scene
            SceneManager.LoadScene(0);
        }
    }

    public void AddToSlot(GameObject item)
    {
        // Eğer önceki item standa bırakılmışsa veya slot temizlenmişse tekrar kullanılabilir
        if (item == null) return;

        // Eğer önceki item zaten yoksa, slot boş sayılır
        if (slotFull && heldItem == null)
            slotFull = false;

        if (slotFull)
        {
            Debug.Log("Slot dolu, yeni item eklenemiyor.");
            return;
        }

        SpriteRenderer sr = item.GetComponent<SpriteRenderer>();
        if (sr != null && slotImage != null)
        {
            slotImage.sprite = sr.sprite;
           slotImage.color = Color.white;
           
        }

        heldItem = item;
        slotFull = true;

        Debug.Log("Item slot'a eklendi: " + item.name);
    }

    public bool IsSlotEmpty() => !slotFull;

    public GameObject GetHeldItem() => heldItem;

    public void ClearSlot()
    {
        // UI temizle
       
            slotImage.sprite = nullSlot;
        //    slotImage.color = new Color(1, 1, 1, 0);
        

        // item referansını da sıfırla
        heldItem = null;
        slotFull = false;

        Debug.Log("Slot temizlendi.");
    }
}
