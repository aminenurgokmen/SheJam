using UnityEngine;

public class BodyPart : MonoBehaviour
{
 public int id;
    [HideInInspector] public Vector3 originalPosition;
    [HideInInspector] public Transform originalParent;

    public void RememberOrigin(Transform parent)
    {
        originalParent = parent;
        originalPosition = transform.position;
    }

    public void ReturnToOrigin()
    {
        if (originalParent == null)
        {
            Debug.LogWarning($"Item {name} geri dönmek istedi ama orijinal parent bulunamadı.");
            return;
        }

        // Pozisyon ve parent geri atanıyor
        transform.SetParent(originalParent);
        transform.position = originalPosition;
        gameObject.SetActive(false);

        // GraveScript'i bulup hiddenObject olarak yeniden bağla
        GraveScript grave = originalParent.GetComponent<GraveScript>();
        if (grave != null)
        {
            grave.hiddenObject = gameObject;
            Debug.Log($"Item {id} geri döndü ve {grave.name} içine tekrar atandı.");
        }
        else
        {
            Debug.LogWarning($"Item {id} orijinal parentta GraveScript bulunamadı.");
        }
    }
}
