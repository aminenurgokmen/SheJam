using UnityEngine;

public class StandScript : MonoBehaviour
{
    private StandSlot[] slots;
    private Transform player;
    private float interactDistance = 2.5f;

    void Start()
    {
        slots = GetComponentsInChildren<StandSlot>();
        if (PlayerMovement.instance != null)
            player = PlayerMovement.instance.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(player.position, transform.position);
        if (distance <= interactDistance && Input.GetKeyDown(KeyCode.E))
        {
            TryPlaceItem();
        }
    }

    private void TryPlaceItem()
    {
        GameObject heldItem = GameManager.instance.GetHeldItem();
        if (heldItem == null)
        {
            Debug.Log("Elde item yok.");
            return;
        }

        BodyPart data = heldItem.GetComponent<BodyPart>();
        if (data == null)
        {
            Debug.Log("ItemData yok.");
            return;
        }

        int id = data.id;

        // 🔹 1. Sıralama kontrolü: önceki slotlar dolu mu?
        for (int i = 0; i < id; i++)
        {
            StandSlot previousSlot = FindSlotById(i);
            if (previousSlot != null && !previousSlot.isOccupied)
            {
                Debug.Log($"Slot {id} yerleşemez! Önce slot {i} doldurulmalı.");
                // item mezarına geri dönsün
                data.ReturnToOrigin();
                GameManager.instance.ClearSlot();
                return;
            }
        }

        // 🔹 2. Doğru slotu bul ve yerleştir
        StandSlot targetSlot = FindSlotById(id);
        if (targetSlot != null && targetSlot.CanPlaceItem(id))
        {
            targetSlot.PlaceItem(heldItem);
            GameManager.instance.ClearSlot();
            return;
        }

        // 🔹 3. Eğer uygun değilse, geri gönder
        Debug.Log($"Item {id} için uygun slot yok veya dolu.");
        data.ReturnToOrigin();
        GameManager.instance.ClearSlot();
    }

    private StandSlot FindSlotById(int id)
    {
        foreach (StandSlot slot in slots)
        {
            if (slot.slotId == id)
                return slot;
        }
        return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}
