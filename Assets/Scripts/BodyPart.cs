using System.Collections;
using UnityEngine;

public class BodyPart : MonoBehaviour
{
    public int id;
    [HideInInspector] public Vector3 originalPosition;
    [HideInInspector] public Quaternion originalRotation;
    [HideInInspector] public Transform originalParent;

    private bool canBePickedUp = false;
    private Transform player;
    private float pickupDistance = 2f;

    private void Start()
    {
        if (PlayerMovement.instance != null)
            player = PlayerMovement.instance.transform;
    }

    private void Update()
    {
        if (!canBePickedUp || player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= pickupDistance && Input.GetKeyDown(KeyCode.E))
        {
            PickUp();
        }
    }

    public void RememberOrigin(Transform parent)
    {
        originalParent = parent;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    public void ReturnToOrigin()
    {
        if (originalParent == null)
        {
            Debug.LogWarning($"Item {name} geri dönmek istedi ama orijinal parent bulunamadı.");
            return;
        }

        transform.SetParent(originalParent);
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        gameObject.SetActive(false);
      GetComponent<SpriteRenderer>().enabled = true;

        GraveScript grave = originalParent.GetComponent<GraveScript>();
        if (grave != null)
        {
            grave.hiddenObject = gameObject;
            Debug.Log($"Item {id} geri döndü ve {grave.name} içine tekrar atandı.");
        }
    }

    public void MakePickable()
    {
        canBePickedUp = true;
        Debug.Log($"{name} artık toplanabilir durumda.");
    }

    private void PickUp()
    {
        // Player itemi eline alıyor
        GameManager.instance.AddToSlot(gameObject);
        transform.SetParent(player);
        transform.localPosition = new Vector3(0f, -.6f, 0);
        transform.rotation = originalRotation;
        canBePickedUp = false;

        // Rigidbody ve Collider kaldır
      //  Rigidbody2D rb = GetComponent<Rigidbody2D>();
      //  if (rb != null) Destroy(rb);

     //   GetComponent<Collider2D>().enabled = false;

        Debug.Log($"{name} toplandı! ✅");
    }
}
