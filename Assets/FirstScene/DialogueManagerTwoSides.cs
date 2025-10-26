using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    [Header("İçerik")]
    public List<DialogueEntry> lines = new List<DialogueEntry>();

    int index = -1;
    bool isTyping = false;
    string currentFullText = "";
    Coroutine typingCo;
    // aktif hedef UI referansları
    TMP_Text activeName, activeBody;
    GameObject activeBox, activeContinue;

    void Start()
    {
        // ikonları kapat
        if (leftContinueIcon) leftContinueIcon.SetActive(false);
        if (rightContinueIcon) rightContinueIcon.SetActive(false);

        // İstersen otomatik başlat
        if (lines != null && lines.Count > 0)
            StartDialogue();
    }

    public void StartDialogue()
    {
        index = -1;
        NextLine();
    }

    void Update()
    {
        // aktif kutu yoksa dinleme
        if (activeBox == null || !activeBox.activeInHierarchy) return;

        if (Input.GetKeyDown(advanceKey))
        {
            if (isTyping) FinishTypingInstant();
            else NextLine();
        }
    }

    void NextLine()
    {
        // ikonları kapat
        if (leftContinueIcon) leftContinueIcon.SetActive(false);
        if (rightContinueIcon) rightContinueIcon.SetActive(false);

        index++;
        if (index >= lines.Count)
        {
            EndDialogue();
            return;
        }

        var entry = lines[index];

        // Hangi taraf konuşuyor?
        bool leftSpeaking = entry.side == DialogueSide.Left;

        // Göster/Gizle
        leftBox.SetActive(leftSpeaking);
        rightBox.SetActive(!leftSpeaking);

        // Aktif UI referanslarını ata
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

        // Metinleri yükle
        activeName.text = entry.speaker;
        currentFullText = entry.text;
        activeBody.text = "";

        // Diğer kutunun body metnini de temizleyelim (eski yazı kalmasın)
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

    void EndDialogue()
    {
        leftBox.SetActive(false);
        rightBox.SetActive(false);
    }
}
