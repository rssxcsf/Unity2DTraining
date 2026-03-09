using UnityEngine;
public enum CharacterType
{
    Npc,
    Player
}
[System.Serializable]
public class DialogNode
{
    public CharacterType type;
    public string name;
    [TextArea]
    public string dialog;
}
[CreateAssetMenu(fileName = "Dialogue")]
public class Dialogue :ScriptableObject
{
    public DialogNode[] dialogNodes;
}
