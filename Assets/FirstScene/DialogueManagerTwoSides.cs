using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // 🎬 Sahne geçişi için eklendi

public class DialogueManagerTwoSides : MonoBehaviour
{
    [Header("Left UI")]
    public GameObject leftBox;
    public TMP_Text leftNameText;
    public TMP_Text leftBodyText;
    public GameObject leftContinueIcon;

    [Header("Right UI")]
    public GameObject rightBox;
    public TMP_Text rightNameText;
    public TMP_Text rightBodyText;
    public GameObject rightContinueIcon;

    [Header("Ayarlar")]
    public KeyCode advanceKey = KeyCode.Space;
    public float charsPerSecond = 45f;
    public float startDelay = 3f; // 💬 İlk diyalog gecikmesi
    float endSceneDelay = 1f; // 🎬 Son sahne geçiş gecikmesi

    [Header("İçerik")]
    public List<DialogueEntry> lines = new List<DialogueEntry>();
    public string nextSceneName = "MainScene"; // 💡 Hedef sahne ismi (Inspector'dan ayarlanabilir)
    public GameObject cam;
    int index = -1;
    bool isTyping = false;
    string currentFullText = "";
    Coroutine typingCo;

    TMP_Text activeName, activeBody;
    GameObject activeBox, activeContinue;

    void Start()
    {
        if (leftBox) leftBox.SetActive(false);
        if (rightBox) rightBox.SetActive(false);
        if (leftContinueIcon) leftContinueIcon.SetActive(false);
        if (rightContinueIcon) rightContinueIcon.SetActive(false);

        if (lines != null && lines.Count > 0)
            StartCoroutine(DelayedStartDialogue());
    }

    IEnumerator DelayedStartDialogue()
    {
        yield return new WaitForSeconds(startDelay);
        StartDialogue();
    }

    public void StartDialogue()
    {
        index = -1;
        NextLine();
    }

    void Update()
    {
        if (activeBox == null || !activeBox.activeInHierarchy) return;

        if (Input.GetKeyDown(advanceKey))
        {
            if (isTyping) FinishTypingInstant();
            else NextLine();
        }
    }

    void NextLine()
    {
        if (leftContinueIcon) leftContinueIcon.SetActive(false);
        if (rightContinueIcon) rightContinueIcon.SetActive(false);

        index++;
        if (index >= lines.Count)
        {
            // 🎬 Diyalog bittiğinde sahne geçişini başlat
            StartCoroutine(EndDialogueAndChangeScene());
            return;
        }

        var entry = lines[index];
        bool leftSpeaking = entry.side == DialogueSide.Left;

        leftBox.SetActive(leftSpeaking);
        rightBox.SetActive(!leftSpeaking);

        if (leftSpeaking)
        {
            activeBox = leftBox;
            activeName = leftNameText;
            activeBody = leftBodyText;
            activeContinue = leftContinueIcon;
        }
        else
        {
            activeBox = rightBox;
            activeName = rightNameText;
            activeBody = rightBodyText;
            activeContinue = rightContinueIcon;
        }

        activeName.text = entry.speaker;
        currentFullText = entry.text;
        activeBody.text = "";

        if (leftSpeaking) rightBodyText.text = "";
        else leftBodyText.text = "";

        if (typingCo != null) StopCoroutine(typingCo);
        typingCo = StartCoroutine(TypeRoutine(currentFullText));
    }

    IEnumerator TypeRoutine(string full)
    {
        isTyping = true;
        float t = 0f;
        int len = 0;

        while (len < full.Length)
        {
            t += Time.deltaTime * charsPerSecond;
            int nextLen = Mathf.Clamp(Mathf.FloorToInt(t), 0, full.Length);
            if (nextLen != len)
            {
                len = nextLen;
                activeBody.text = full.Substring(0, len);
            }
            yield return null;
        }

        activeBody.text = full;
        isTyping = false;
        if (activeContinue) activeContinue.SetActive(true);
    }

    void FinishTypingInstant()
    {
        if (typingCo != null) StopCoroutine(typingCo);
        isTyping = false;
        activeBody.text = currentFullText;
        if (activeContinue) activeContinue.SetActive(true);
    }

    IEnumerator EndDialogueAndChangeScene()
    {
        leftBox.SetActive(false);
        rightBox.SetActive(false);
        cam.GetComponent<Animator>().SetTrigger("Viynet");

        Debug.Log("Diyalog bitti, sahne değişimi başlıyor...");
        yield return new WaitForSeconds(endSceneDelay);
        SceneManager.LoadScene(2);

    }
}
