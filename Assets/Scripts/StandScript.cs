using UnityEngine;

public class StandScript : MonoBehaviour
{
    private StandSlot[] slots;
    private Transform player;
    private float interactDistance = 5f;
    public ParticleSystem puff;
    public Transform dropPoint;
    public ParticleSystem completeEffect;

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
        if (distance <= interactDistance) PlayerMovement.instance.torch.SetActive(false);
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
                UIManager.instance.ShowWrongPartMessage();
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
                    GetComponent<AudioSource>().Play();

                    targetSlot.PlaceItem(heldItem);
                    GameManager.instance.ClearSlot();
                    UIManager.instance.NextDialog();
                    Debug.Log("QTE başarıyla tamamlandı! Item yerleştirildi ✅");
                    if (AreAllPartsPlaced())
                    {
                        Debug.Log("🎉 Tüm parçalar yerleştirildi!");
                        if (completeEffect != null)
                            completeEffect.Play();
                        GetComponentInChildren<AudioSource>().Play();
                    }
                }
                else
                {
                    Debug.Log("QTE başarısız ❌ Item yukarı çıkıp smooth olarak düşüyor!");

                    // 🔹 Physics kaldır
                    if (heldItem.TryGetComponent<Rigidbody2D>(out var rb)) Destroy(rb);
                    if (heldItem.TryGetComponent<Collider2D>(out var col)) Destroy(col);

                    heldItem.transform.SetParent(null);

                    // 🔹 Sprite görünür hale getir
                    SpriteRenderer sr = heldItem.GetComponent<SpriteRenderer>();
                    if (sr != null)
                        sr.enabled = true;

                    // 🔹 DropPoint belirlenmemişse fallback olarak stand pozisyonunu kullan
                    Vector3 targetPos = dropPoint != null ? dropPoint.position : transform.position;

                    // 🔹 Smooth yay şeklinde düşüş başlat
                    PlayerMovement.instance.StartCoroutine(SmoothArcDrop(heldItem.transform, targetPos, 1.2f, 1.5f));

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
    private bool AreAllPartsPlaced()
    {
        foreach (var slot in slots)
        {
            if (!slot.isOccupied)
                return false;
        }
        return true;
    }
    private System.Collections.IEnumerator SmoothArcDrop(Transform item, Vector3 targetPos, float duration, float arcHeight)
    {
        Vector3 startPos = item.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Y ekseninde parabolik hareket (yukarı çıkıp sonra aşağı)
            float height = 4 * arcHeight * t * (1 - t);
            Vector3 midPos = Vector3.Lerp(startPos, targetPos, t);
            midPos.y += height; // ekstra yükseklik ekle

            item.position = midPos;
            yield return null;
        }

        item.position = targetPos;

        // 🔹 Düşüş tamamlanınca tekrar toplanabilir olsun
        BodyPart bp = item.GetComponent<BodyPart>();
        if (bp != null)
            bp.MakePickable();

        Debug.Log($"{item.name} yay çizerek drop noktasına ulaştı 🎯");
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
