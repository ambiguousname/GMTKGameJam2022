using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class DialogueCreator : DialogueViewBase
{
    public float textSpeed = 2.0f;

    private Text _characterName;
    private Text _conversation;
    private Text _select;
    private IEnumerator _activeTextPrint;
    private string _textToPrint;
    private Action _dialogueFinished;
    private Action<int> _choiceFinished;

    private void Awake()
    {
        _characterName = this.gameObject.FindChildWithName("Character Name").GetComponent<Text>();
        _conversation = this.gameObject.FindChildWithName("Conversation").GetComponent<Text>();
        _select = this.gameObject.FindChildWithName("Select").GetComponent<Text>();
    }

    public void FireClick() {
        StopCoroutine(_activeTextPrint);
        if (_dialogueFinished != null)
        {
            if (_select.gameObject.activeInHierarchy)
            {
                _dialogueFinished();
            }
            else
            {
                _conversation.text = _textToPrint;
                _select.gameObject.SetActive(true);
            }
        }
        else {
            _select.text = _textToPrint;
        }
    }

    public void ChooseOptions(int choice) {
        _choiceFinished(choice);
    }

    public override void DialogueStarted()
    {
        this.transform.GetChild(0).gameObject.SetActive(true);
    }

    public override void RunOptions(DialogueOption[] dialogueOptions, Action<int> onOptionSelected)
    {
        _dialogueFinished = null;
        _select.gameObject.SetActive(true);
        _select.text = "";
        _textToPrint = "";
        foreach (var option in dialogueOptions) {
            _textToPrint += option.Line.TextWithoutCharacterName.Text + "\n";
        }
        _activeTextPrint = AddText(_textToPrint, _select, textSpeed * 1.5f);
        _choiceFinished = onOptionSelected;
        StartCoroutine(_activeTextPrint);
    }

    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        _select.gameObject.SetActive(false);
        _select.text = "Fire to continue";
        _conversation.text = "";
        _characterName.text = dialogueLine.CharacterName;
        _textToPrint = dialogueLine.TextWithoutCharacterName.Text;
        _activeTextPrint = AddText(_textToPrint, _conversation, textSpeed);
        _dialogueFinished = onDialogueLineFinished;
        StartCoroutine(_activeTextPrint);
    }

    IEnumerator AddText(string t, Text ui, float speed)
    {
        for (int i = 0; i < t.Length; i++)
        {
            ui.text += t[i];

            if (t[i] == '.')
            {
                yield return new WaitForSeconds(0.2f * speed);
            }
            else if (t[i] == ',')
            {
                yield return new WaitForSeconds(0.1f * speed);
            }
            else
            {

                yield return new WaitForSeconds(0.05f * speed);
            }
        }

        yield return new WaitForSeconds(0.1f * speed);
        _select.gameObject.SetActive(true);
    }

    public override void DialogueComplete()
    {
        this.transform.GetChild(0).gameObject.SetActive(false);
    }
}
