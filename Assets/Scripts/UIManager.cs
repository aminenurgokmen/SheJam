using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [Header("Dialog UI")]
    public GameObject dialogPanel;
    public TextMeshProUGUI dialogText;

    private int currentDialogIndex = 0;
    public float typeSpeed = 0.03f;
    private string[] dialogLines =
    {
        "The hands.. How could I have done the healing touches if it weren't for my hands. ",
        "My arms are a burden and my legs are failing me. I got no life inside me anymore. My body... rotting there in silence",
        "Without my legs, I cannot walk… not in this world, nor in dreams.",
        "My head… my memories were buried with it beneath the earth. Bring it back, Oshai I want to remember who I am.",
        "Without my eyes, all I saw was darkness.Now I want to see, Oshai... the world, you… and the truth.",
        "Thank you darling!"
    };

    private Coroutine typingCoroutine;
    private Coroutine warningCoroutine;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ShowDialog(0);
    }

    public void ShowDialog(int index)
    {
        if (dialogPanel == null || dialogText == null) return;

        dialogPanel.SetActive(true);

        if (index >= 0 && index < dialogLines.Length)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TypeText(dialogLines[index]));
        }
    }

    public void NextDialog()
    {
        currentDialogIndex++;
        if (currentDialogIndex >= dialogLines.Length)
        {
            dialogPanel.SetActive(false); // Diyalog bittiğinde gizle
            return;
        }
        ShowDialog(currentDialogIndex);
    }

    public void ShowWrongPartMessage()
    {
        if (warningCoroutine != null)
            StopCoroutine(warningCoroutine);

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        warningCoroutine = StartCoroutine(WrongPartRoutine());
    }


    private IEnumerator WrongPartRoutine()
    {
        dialogText.text = "Find what I want first. Only then this piece of body will accept you.";
        yield return new WaitForSeconds(3f);
        ShowDialog(currentDialogIndex); // Görev diyaloguna geri dön
    }
    private IEnumerator TypeText(string fullText)
    {
        dialogText.text = "";
        foreach (char c in fullText)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
    }
}
