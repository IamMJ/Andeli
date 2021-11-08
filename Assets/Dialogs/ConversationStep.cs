using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[CreateAssetMenu(fileName = "Conversation Step")]
public class ConversationStep : ScriptableObject
{
    [SerializeField] public string NPCText = "";
    [SerializeField] public Sprite NPCSprite = null;
    [SerializeField] public Color SpriteColor = Color.white;
    [SerializeField] public string PlayerText_0 = "Choice 0";
    [SerializeField] public ReplyOption resultFromOption0 = ReplyOption.AdvanceOneStep;
    [SerializeField] public string PlayerText_1 = "Choice 1";
    [SerializeField] public ReplyOption resultFromOption1 = ReplyOption.PermQuitConvo;
    [SerializeField] public Vector2 Destination = Vector2.zero;

    [SerializeField] public AdvancementOption ResultFromAdvancement = AdvancementOption.None;

    public enum ReplyOption { AdvanceOneStep, AdvanceTwoSteps, PermQuitConvo, TempQuitConvo, TempMoveNPCandQuitConvo, PermMoveNPCandQuitConvo, TeleportPlayerAndQuitConvo};
    public enum AdvancementOption { None, GrantPowerInDemo}
}
