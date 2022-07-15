using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DialogueCreator : DialogueViewBase
{
    public override void RunOptions(DialogueOption[] dialogueOptions, Action<int> onOptionSelected)
    {
        base.RunOptions(dialogueOptions, onOptionSelected);
    }

    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        base.RunLine(dialogueLine, onDialogueLineFinished);
    }

    public override void DialogueComplete()
    {
        base.DialogueComplete();
    }
}
