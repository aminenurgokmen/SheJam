using UnityEngine;

public class GraveScript : MonoBehaviour
{
    public GameObject hiddenObject;
    public GameObject hiddenGrave;
    private float interactDistance = 4f;

    private Transform player;
    private PlayerMovement playerMovement;
    public GameObject tutorialHint;


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
        if (hiddenGrave != null)
        {
            hiddenGrave.SetActive(false);
        }
    }

    void Update()
    {
        if (hiddenObject == null || playerMovement == null || player == null)
            return;

        float distance = Vector2.Distance(player.position, transform.position);

        if (!playerMovement.torch.activeSelf)

        {
            if (tutorialHint != null)
                tutorialHint.SetActive(false);
            if (hiddenObject.activeSelf)
                hiddenObject.SetActive(false);

            if (hiddenGrave != null && hiddenGrave.activeSelf)
            {
                hiddenGrave.SetActive(false);
            }

            return;
        }

        if (distance <= interactDistance)
        {
            if (!hiddenObject.activeSelf)
                hiddenObject.SetActive(true);
            if (hiddenGrave != null && !hiddenGrave.activeSelf)
            {
                hiddenGrave.SetActive(true);
            }


            if (Input.GetKeyDown(KeyCode.E) && hiddenObject.GetComponent<BodyPart>().id != 99)
            {
                if (hiddenObject.activeSelf && GameManager.instance.IsSlotEmpty())
                {
                    PickUpItem();
                }
            }
            if (tutorialHint != null)
                tutorialHint.SetActive(true);

        }
        else
        {
            if (hiddenObject.activeSelf)
                hiddenObject.SetActive(false);
            if (hiddenGrave != null && hiddenGrave.activeSelf)
            {
                hiddenGrave.SetActive(false);
            }
            if (tutorialHint != null)
                tutorialHint.SetActive(false);

        }
    }

    private void PickUpItem()
    {
        hiddenObject.transform.SetParent(player);
        hiddenObject.transform.localPosition = new Vector3(0f, -.6f, 0);
        hiddenObject.GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<AudioSource>().Play();
        GameManager.instance.AddToSlot(hiddenObject);
        hiddenObject = null; // mezardaki referans sıfırlanır
    }
}
