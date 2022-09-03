using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public GameObject UIDialogObj;
    public bool DialogueShown;
    public float timeSinceLastDialogueStarted;

    //  Left

    public GameObject _dialoguePromptLeft;
    public Text _dialogueLeftText;
    public Button _leftButton;
    public Image _leftDialogueTime;
    public GameObject _speakerLeft;
    public Image _speakerLeftImage;
    public GameObject _speakerLeftImageBackground;
    public Text _speakerLeftName;
    public RectTransform _speakerLeftNameBG;
    public GameObject _speakerLeftNameObject;

    //  Right

    public GameObject _dialoguePromptRight;
    public Text _dialogueRightText;
    public Button _rightButton;
    public Image _rightDialogueTime;
    public Image _speakerRightImage;
    public GameObject _speakerRightImageBackground;
    public GameObject _speakerRight;
    public Text _speakerRightName;
    public RectTransform _speakerRightNameBG;
    public GameObject _speakerRightNameObject;
    public static bool playingConversation;

    //  Misc

    [Range(1,16)]
    public int dialogueSpeed;
    private int FramesBetweenCharacters;
    public float speakerHeight;
    [SerializeField]
    private Queue<Line> sentences = new Queue<Line>();
    private Line currentSentence;
    private float timeSinceLastLine;
    private bool mouseHoveringOverText;
    private bool typing;
    private enum Speaker { Left, Right  }

    //  Commands

    [SerializeField]
    private float timeSinceLastWeaponFired;
    private float timeBetweenFireCommands;
    private int lastFireCommandGiven;

    private void Awake()
    {
        REF.Dialog = this;
        typing = false;
        speakerHeight = _speakerRightImage.rectTransform.sizeDelta.y;
        _leftButton.onClick.AddListener(() => DisplayNextSentence());
        _rightButton.onClick.AddListener(() => DisplayNextSentence());
    }
    private void Start()
    {
        timeSinceLastLine = 0;
        timeSinceLastWeaponFired = timeBetweenFireCommands = 30;
        lastFireCommandGiven = 0;
        FramesBetweenCharacters = 16 / dialogueSpeed; // 0 frames between text is max dialogue speed
        TurnOffDialogue();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            REF.Dialog.DisplayNextSentence();
        }
        
        /*if (Input.GetKeyDown(KeyCode.F))
        {
            REF.Dialog.StartDialogue(Resources.Load("Dialogue/Conversations/TestConvo", typeof(ConversationScriptObj)) as ConversationScriptObj);
        }*/
        timeSinceLastLine += Time.deltaTime;
        timeSinceLastWeaponFired += Time.deltaTime;
        if (mouseHoveringOverText || typing) timeSinceLastLine = 0;
        UpdateDialogueLength();
        AutoCheckNextsentence();
    }
    public void StartDialogue(ConversationScriptObj convo, bool showDialogueTime = true, bool buttonsInteractable = true)
    {
        if (!convo) { Debug.LogError("No Convo Found!"); return; }

        DialogueShown = true;
        TurnOnDialogue();
        if (!playingConversation)
        {
            playingConversation = true;
            sentences.Clear();

            foreach (Line line in convo.lines)
            {
                sentences.Enqueue(line);
            }
            DisplayNextSentence();
        }
        ShowDialogueTimes(showDialogueTime);
        ButtonsInteractable(buttonsInteractable);
    }

    private void AutoCheckNextsentence()
    {
        if (currentSentence.LineTimer == 0) return;
        if (timeSinceLastLine >= currentSentence.LineTimer)
        {
            DisplayNextSentence();
        }
    }
    private void UpdateDialogueLength()
    {
        if (typing || currentSentence.LineTimer == 0)
        {
            _leftDialogueTime.fillAmount = 1;
            _rightDialogueTime.fillAmount = 1;
        }
        else
        {
            _leftDialogueTime.fillAmount = Mathf.Max(0, 1 - Mathf.Min(1, timeSinceLastLine / currentSentence.LineTimer));
            _rightDialogueTime.fillAmount = Mathf.Max(0, 1 - Mathf.Min(1, timeSinceLastLine / currentSentence.LineTimer));
        }
    }
    public void DisplayNextSentence()
    {
        mouseHoveringOverText = false;
        timeSinceLastLine = 0;
        HideDialoguePrompt();

        _speakerLeft.SetActive(false);
        _speakerRight.SetActive(false);
        _speakerLeftNameObject.SetActive(false);
        _speakerRightNameObject.SetActive(false);
        _dialogueLeftText.gameObject.SetActive(false);
        _dialogueRightText.gameObject.SetActive(false);

        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        Line line = sentences.Dequeue();
        currentSentence = line;
        StopAllCoroutines();
        StartCoroutine(ShowDialoguePrompt());

        if (line.characterLeft)
        {
            _speakerLeft.SetActive(true);
            _speakerLeftImage.sprite = line.characterLeft.portrait;
            if(line.characterLeft.width > 0 && line.characterLeft.height > 0)
            {
                _speakerLeftImage.rectTransform.sizeDelta = new Vector2(((float) line.characterLeft.width/line.characterLeft.height) * speakerHeight, speakerHeight);
            }
            if(line.characterLeft.fullName != null)
            {
                _speakerLeftNameObject.SetActive(true);
                _speakerLeftName.text = line.characterLeft.fullName;
                //_speakerLeftNameBG.sizeDelta = new Vector2(Mathf.Max(_speakerLeftImageBackground.GetComponent<RectTransform>().sizeDelta.x, 30 + line.characterLeft.fullName.Length * 28), _speakerLeftNameBG.rect.height);
            }

            if (line.textLeft != "")
            {
                _dialogueLeftText.gameObject.SetActive(true);
                StartCoroutine(TypeSentence(line.textLeft, Speaker.Left));
            }
        }
        if (line.characterRight)
        {
            _speakerRight.SetActive(true);
            _speakerRightImage.sprite = line.characterRight.portrait;
            if (line.characterRight.width > 0 && line.characterRight.height > 0)
            {
                _speakerRightImage.rectTransform.sizeDelta = new Vector2(((float) line.characterRight.width / line.characterRight.height) * speakerHeight, speakerHeight);
            }
            if (line.characterRight.fullName != null)
            {
                _speakerRightNameObject.SetActive(true);
                _speakerRightName.text = line.characterRight.fullName;
                //_speakerRightNameBG.sizeDelta = new Vector2(Mathf.Max( _speakerRightImageBackground.GetComponent<RectTransform>().sizeDelta.x, 30 + line.characterRight.fullName.Length * 28), _speakerRightNameBG.rect.height);
            }

            if (line.textRight != "")
            {
                _dialogueRightText.gameObject.SetActive(true);
                StartCoroutine(TypeSentence(line.textRight, Speaker.Right));
            }
        }
    }
    private IEnumerator TypeSentence(string sentence, Speaker speaker)
    {
        timeSinceLastLine = 0;
        typing = true;
        _dialogueLeftText.text = "";
        _dialogueRightText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            if (speaker == Speaker.Left) _dialogueLeftText.text += letter;
            else _dialogueRightText.text += letter;

            for (int i = 0; i <= FramesBetweenCharacters; i++)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        typing = false;
        timeSinceLastLine = 0;
    }

    public void TurnOnDialogue()
    {
        UIDialogObj.SetActive(true);
    }
    public void TurnOffDialogue()
    {
        UIDialogObj.SetActive(false);
    }

    public void EndDialogue()
    {
        currentSentence = new Line();
        playingConversation = false;
        TurnOffDialogue();
        DialogueShown = false;
        timeSinceLastDialogueStarted = 0f;
        mouseHoveringOverText = false;
    }
    public IEnumerator ShowDialoguePrompt()
    {
        yield return new WaitForSeconds(5f);
        for(int i = 0; i < 50; i++)
        {
            foreach(Image img in _dialoguePromptRight.GetComponentsInChildren<Image>())
            {
                img.color = new Color(1, 1, 1, (i / 50f));
            }
            foreach (Image img in _dialoguePromptLeft.GetComponentsInChildren<Image>())
            {
                img.color = new Color(1, 1, 1, (i / 50f));
            }
            yield return new WaitForFixedUpdate();
        }
    }
    public void HideDialoguePrompt()
    {
        StopCoroutine(ShowDialoguePrompt());
        foreach (Image img in _dialoguePromptLeft.GetComponentsInChildren<Image>())
        {
            img.color = new Color(1, 1, 1, 0);
        }
        foreach (Image img in _dialoguePromptRight.GetComponentsInChildren<Image>())
        {
            img.color = new Color(1, 1, 1, 0);
        }
    }
    private void ShowDialogueTimes(bool showDialogueTime)
    {
        _leftDialogueTime.transform.parent.gameObject.SetActive(showDialogueTime);
        _rightDialogueTime.transform.parent.gameObject.SetActive(showDialogueTime);
    }
    private void ButtonsInteractable(bool interactable)
    {
        _leftButton.interactable = interactable;
        _rightButton.interactable = interactable;
    }
    public void MouseHoveringOverText()
    {
        mouseHoveringOverText = true;
    }
    public void MouseNoLongerHoveringOverText()
    {
        mouseHoveringOverText = false;
    }

    //  Specific Conversations

    public void FireWeapon()
    {
        if (timeSinceLastWeaponFired < timeBetweenFireCommands) return;

        timeSinceLastWeaponFired = 0;
        int convoIndex = UnityEngine.Random.Range(0, 3);
        ConversationScriptObj conversation = Resources.Load("Dialogue/Conversations/FireCommand0", typeof(ConversationScriptObj)) as ConversationScriptObj;
        if (convoIndex == lastFireCommandGiven) convoIndex += 1;
        if (convoIndex > 2) convoIndex = 0;

        if (convoIndex == 0) conversation = Resources.Load("Dialogue/Conversations/FireCommand1", typeof(ConversationScriptObj)) as ConversationScriptObj;
        else if (convoIndex == 1) conversation = Resources.Load("Dialogue/Conversations/FireCommand2", typeof(ConversationScriptObj)) as ConversationScriptObj;
        else if (convoIndex == 2) conversation = Resources.Load("Dialogue/Conversations/FireCommand3", typeof(ConversationScriptObj)) as ConversationScriptObj;

        lastFireCommandGiven = convoIndex;
        StartDialogue(conversation, false, false);
    }
}