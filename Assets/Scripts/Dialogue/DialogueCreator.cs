using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class DialogueCreator : DialogueViewBase
{
    private struct DialogueChoice {
        public int lower;
        public int upper;
        public string attribute;
        public DialogueChoice(int l, int u, string a) {
            lower = l;
            upper = u;
            attribute = a;
        }
    }

    public float textSpeed = 2.0f;

    private Text _characterName;
    private Text _conversation;
    private Text _select;
    private IEnumerator _activeTextPrint;
    private string _textToPrint;
    private Action _dialogueFinished;
    private Action<int> _choiceFinished;
    private List<DialogueChoice> _choices;
    private PlayerController _player;

    private void Awake()
    {
        _characterName = this.gameObject.FindChildWithName("Character Name").GetComponent<Text>();
        _conversation = this.gameObject.FindChildWithName("Conversation").GetComponent<Text>();
        _select = this.gameObject.FindChildWithName("Select").GetComponent<Text>();
        _choices = new List<DialogueChoice>();
    }

    private void OnEnable()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _player.onFire.AddListener(FireClick);
    }

    private void OnDisable()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().onFire.RemoveListener(FireClick);
    }

    public void SetOption(int start, int end, string attribute="") {
        _choices.Add(new DialogueChoice(start, end, attribute));
    }

    public void FireClick() {
        if (_activeTextPrint != null)
        {
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
            else
            {
                _select.text = _textToPrint;
                ShowRolling();
            }
        }
    }

    public void ChooseOptions(int roll, string attribute) {
        for (int i = 0; i < _choices.Count; i++) {
            var choice = _choices[i];
            if (attribute == choice.attribute && roll >= choice.lower && roll <= choice.upper) {
                _choices.Clear();
                _choiceFinished(i);
                return;
            }
        }
        _choices.Clear();
        Debug.LogError("No valid choices found for a roll of " + roll + " with Attribute: " + attribute + ".");
    }

    public override void DialogueStarted()
    {
        this.transform.GetChild(0).gameObject.SetActive(true);
        _player.moveEnabled = false;
        FindObjectOfType<CombatUIManager>().Show(false);
    }

    string EvaluateAttributes(List<Yarn.Markup.MarkupAttribute> attributes, string toAdd) {
        int offset = 0;
        foreach (var attribute in attributes)
        {
            if (attribute.Name == "style")
            {
                var colorTag = "<color=#" + attribute.Properties["color"].StringValue + ">";
                toAdd = toAdd.Insert(attribute.Position + offset, colorTag);
                offset += colorTag.Length;
                toAdd = toAdd.Insert(attribute.Position + attribute.Length + offset, "</color>");
                offset += "</color>".Length;
            }
            if (attribute.Name == "range")
            {
                var range = attribute.Properties["range"].StringValue.Split(',');
                if (attribute.Properties.ContainsKey("attribute"))
                {
                    SetOption(Int16.Parse(range[0]), Int16.Parse(range[1]), attribute.Properties["attribute"].StringValue);
                }
                else
                {
                    SetOption(Int16.Parse(range[0]), Int16.Parse(range[1]));
                }
            }
        }
        return toAdd;
    }

    public override void RunOptions(DialogueOption[] dialogueOptions, Action<int> onOptionSelected)
    {
        _dialogueFinished = null;
        _select.gameObject.SetActive(true);
        _select.text = "";
        _textToPrint = "";
        for (int i = 0; i < dialogueOptions.Length; i++) {
            var option = dialogueOptions[i];
            var toAdd = option.Line.TextWithoutCharacterName.Text + ((i < dialogueOptions.Length - 1) ? "\n" : "");
            _textToPrint += EvaluateAttributes(option.Line.TextWithoutCharacterName.Attributes, toAdd);
        }
        _activeTextPrint = AddText(_textToPrint, _select, textSpeed * 0.5f);
        _choiceFinished = onOptionSelected;
        StartCoroutine(_activeTextPrint);
    }

    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        _select.gameObject.SetActive(false);
        _select.text = "Fire to continue";
        _conversation.text = "";
        _characterName.text = dialogueLine.CharacterName;
        _textToPrint = EvaluateAttributes(dialogueLine.TextWithoutCharacterName.Attributes, dialogueLine.TextWithoutCharacterName.Text);
        _activeTextPrint = AddText(_textToPrint, _conversation, textSpeed);
        _dialogueFinished = onDialogueLineFinished;
        StartCoroutine(_activeTextPrint);
    }

    void ShowRolling() {
        var roller = FindObjectOfType<RollerManager>();
        if (!roller.transform.GetChild(0).gameObject.activeInHierarchy) {
            roller.EnableRolling(ChooseOptions);
        }
    }

    IEnumerator AddText(string t, Text ui, float speed)
    {
        // The limitations of this is that you can only have one tag around something at a time. Shouldn't be a problem for now.
        bool isTag = false;
        int tagStart = -1;
        int tagEnd = -1;
        int tagEndLength = -1;
        for (int i = 0; i < t.Length; i++)
        {
            if (i > tagStart && i < tagEnd)
            {
                ui.text = ui.text.Insert(ui.text.Length - (tagEndLength), "" + t[i]);
            } else {
                ui.text += t[i];
            }

            if (isTag && t[i] == '>')
            {
                tagStart = i;
                int j;
                for (j = i; t[j] != '<'; j++) ;
                tagEnd = j;
                tagEndLength = 0;
                int k;
                for (k = j; t[k] != '>'; k++)
                {
                    ui.text += t[k];
                    tagEndLength++;
                }
                tagEndLength++;
                ui.text += t[k];
                isTag = false;
                continue;
            }
            else if (t[i] == '<')
            {
                isTag = true;
                continue;
            }
            else if (isTag)
            {
                continue;
            }

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

            if (tagEnd != -1 && i >= tagEnd - 1)
            {
                i += (tagEndLength);
                isTag = false;
                tagStart = -1;
                tagEnd = -1;
                tagEndLength = -1;
            }
        }

        yield return new WaitForSeconds(0.1f * speed);
        _select.gameObject.SetActive(true);

        if (_dialogueFinished == null) {
            ShowRolling();
        }
    }

    public override void DialogueComplete()
    {
        this.transform.GetChild(0).gameObject.SetActive(false);
        _player.moveEnabled = true;
        FindObjectOfType<CombatUIManager>().Show(true);
    }
}
