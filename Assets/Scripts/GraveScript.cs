using UnityEngine;

public class GraveScript : MonoBehaviour
{
    public GameObject hiddenObject; 
    private float interactDistance = 3f;

    private Transform player;
    private PlayerMovement playerMovement;

    void Start()
    {
        // Player’ı sahnede bul
        playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null)
            player = playerMovement.transform;

        // Obje başlangıçta kapalı olsun
        if (hiddenObject != null)
            hiddenObject.SetActive(false);
    }

    void Update()
    {
        if (player == null || playerMovement == null) return;

        float distance = Vector2.Distance(player.position, transform.position);

        // Işık kapalıysa hiçbir şey yapma
        if (!playerMovement.torch.activeSelf)
        {
            if (hiddenObject.activeSelf)
                hiddenObject.SetActive(false);
            return;
        }

        // Işık açıksa ve yakınsa aç, uzaksa kapat
        if (distance <= interactDistance)
        {
            if (!hiddenObject.activeSelf)
                hiddenObject.SetActive(true);
        }
        else
        {
            if (hiddenObject.activeSelf)
                hiddenObject.SetActive(false);
        }
    }

    // Sahne içinde mesafeyi görsel olarak görmek istersen
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}
