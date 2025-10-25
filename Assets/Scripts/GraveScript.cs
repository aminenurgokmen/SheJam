using UnityEngine;

public class GraveScript : MonoBehaviour
{
    public GameObject hiddenObject;
    private float interactDistance = 3f;

    private Transform player;
    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null)
            player = playerMovement.transform;

        if (hiddenObject != null)
        {
            hiddenObject.SetActive(false);

            // ItemData varsa orijinal konumunu hatırlasın
            BodyPart data = hiddenObject.GetComponent<BodyPart>();
            if (data != null)
                data.RememberOrigin(transform);
        }
    }

    void Update()
    {
        if (hiddenObject == null || playerMovement == null || player == null)
            return;

        float distance = Vector2.Distance(player.position, transform.position);

        if (!playerMovement.torch.activeSelf)
        {
            if (hiddenObject.activeSelf)
                hiddenObject.SetActive(false);
            return;
        }

        if (distance <= interactDistance)
        {
            if (!hiddenObject.activeSelf)
                hiddenObject.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (hiddenObject.activeSelf && GameManager.instance.IsSlotEmpty())
                {
                    PickUpItem();
                }
            }
        }
        else
        {
            if (hiddenObject.activeSelf)
                hiddenObject.SetActive(false);
        }
    }

    private void PickUpItem()
    {
        hiddenObject.transform.SetParent(player);
        hiddenObject.transform.localPosition = new Vector3(0.5f, 0, 0);

        GameManager.instance.AddToSlot(hiddenObject);
        hiddenObject = null; // mezardaki referans sıfırlanır
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}
