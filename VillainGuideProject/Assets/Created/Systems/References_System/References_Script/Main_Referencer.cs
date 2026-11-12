using UnityEngine;

public class Main_Referencer : MonoBehaviour
{
    public static Main_Referencer Instance { get; private set; }
    public VisualReferencer visual;
    public ValuesReferencer value;
    public MonsterReferencer monster;
}
