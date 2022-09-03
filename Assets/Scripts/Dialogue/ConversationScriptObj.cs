using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[Serializable]
public struct Line
{
    public DialogueCharacterScriptObj characterLeft;
    public DialogueCharacterScriptObj characterRight;

    [Range(0, 60)]
    public float LineTimer;

    [TextArea(2, 5)]
    public string textLeft;
    [TextArea(2, 5)]
    public string textRight;
}

[CreateAssetMenu(fileName = "New Conversation", menuName = "Dialogue/Conversation")]
public class ConversationScriptObj : ScriptableObject
{
    public Line[] lines;
}
