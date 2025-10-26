using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;
    public float moveSpeed = 3f;
    private Rigidbody2D rb;
    private Vector2 movement;
    public Animator animator;
    public GameObject torch;

    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody2D>();

    }

    private void Update()
    {
        // 🔦 F tuşuyla feneri aç/kapat
        if (Input.GetKeyDown(KeyCode.F))
        {
            torch.SetActive(!torch.activeSelf);
        }

        // 🔹 Hareket girdisi
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // 🔹 Sağ / Sol yönüne göre karakteri çevir
        if (movement.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (movement.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        // 🔹 Animasyon kontrolü
        if (movement.x != 0)
        {
            animator.Play("Side"); // A veya D basılıysa
        }
        else if (movement.y != 0)
        {
            animator.Play("Walk"); // W veya S basılıysa
        }
        else
        {
            animator.Play("Idle"); // Hiçbir tuş basılı değilse
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
