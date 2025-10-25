using UnityEngine;

public class StandScript : MonoBehaviour
{
    private StandSlot[] slots;
    private Transform player;
    private float interactDistance = 5f;
    public ParticleSystem puff;

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

        // 🔹 1. Önceki slotlar dolu mu?
        for (int i = 0; i < id; i++)
        {
            StandSlot previousSlot = FindSlotById(i);
            if (previousSlot != null && !previousSlot.isOccupied)
            {
                Debug.Log($"Slot {id} yerleşemez! Önce slot {i} doldurulmalı.");
                data.ReturnToOrigin();
                GameManager.instance.ClearSlot();
                return;
            }
        }

        // 🔹 2. Doğru slotu bul
        StandSlot targetSlot = FindSlotById(id);
        if (targetSlot != null && targetSlot.CanPlaceItem(id))
        {
            // 🔸 QTE başlat
            QuickTimeEvent.instance.StartQTE(success =>
            {
                if (success)
                {
                    puff.Play();
                    targetSlot.PlaceItem(heldItem);
                    GameManager.instance.ClearSlot();
                    Debug.Log("QTE başarıyla tamamlandı! Item yerleştirildi ✅");
                }
                else
                {
                    Debug.Log("QTE başarısız ❌ Item fırlatılıyor!");

                    Rigidbody2D rb = heldItem.GetComponent<Rigidbody2D>();
                    if (rb == null) rb = heldItem.AddComponent<Rigidbody2D>();
                    rb.gravityScale = 1.5f;
                    rb.constraints = RigidbodyConstraints2D.None;

                    if (heldItem.GetComponent<Collider2D>() == null)
                    {
                        var col = heldItem.AddComponent<CircleCollider2D>();
                        col.radius = 0.2f;
                    }

                    heldItem.transform.SetParent(null);

                    Vector2 randomOffset = Random.insideUnitCircle.normalized * Random.Range(1.5f, 3f);
                    Vector2 dropPosition = (Vector2)transform.position + randomOffset;

                    Vector2 forceDir = (dropPosition - (Vector2)transform.position).normalized;
                    rb.AddForce(forceDir * Random.Range(3f, 6f), ForceMode2D.Impulse);
                    rb.AddTorque(Random.Range(-5f, 5f), ForceMode2D.Impulse);

                    PlayerMovement.instance.StartCoroutine(StopFallingAfterDelay(rb));

                    GameManager.instance.ClearSlot();
                }

            });
            return;
        }

        // 🔹 3. Eğer uygun değilse, geri gönder
        Debug.Log($"Item {id} için uygun slot yok veya dolu.");
        data.ReturnToOrigin();
        GameManager.instance.ClearSlot();
    }

    private System.Collections.IEnumerator StopFallingAfterDelay(Rigidbody2D rb)
    {
        yield return new WaitForSeconds(.8f);

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;

            // 🔹 Yere düştü, artık toplanabilir
            BodyPart bp = rb.GetComponent<BodyPart>();
            if (bp != null)
                bp.MakePickable();
        }
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
