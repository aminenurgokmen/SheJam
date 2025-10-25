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
            hiddenObject.SetActive(false);
    }

    void Update()
    {
        // Eğer zaten hiddenObject yoksa (önceden alınmışsa), hiçbir şey yapma
        if (hiddenObject == null || playerMovement == null || player == null)
            return;

        float distance = Vector2.Distance(player.position, transform.position);

        // Fener kapalıysa objeyi gizle
        if (!playerMovement.torch.activeSelf)
        {
            if (hiddenObject.activeSelf)
                hiddenObject.SetActive(false);
            return;
        }

        // Yakınsa göster, uzaksa gizle
        if (distance <= interactDistance)
        {
            if (!hiddenObject.activeSelf)
                hiddenObject.SetActive(true);

            // E'ye basılırsa, slot boşsa ve obje aktifse pickle
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (hiddenObject.activeSelf && GameManager.instance.IsSlotEmpty())
                {
                    GameManager.instance.AddToSlot(hiddenObject);
                    hiddenObject = null; // bir daha alınamaz
                }
                else if (!GameManager.instance.IsSlotEmpty())
                {
                    Debug.Log("Slot dolu! Başka eşya alınamaz.");
                }
            }
        }
        else
        {
            if (hiddenObject.activeSelf)
                hiddenObject.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}
