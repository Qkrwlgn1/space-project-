    using UnityEngine;

[CreateAssetMenu(fileName = "Defense", menuName = "Scriptable Object/DefenseData")]
public class DefenseStatus : ScriptableObject
{
    public enum DefenseType { HP, Shield, Barrier }

    [Header("# Status Type")]
    public DefenseType statusType;
    public int statusId;
    public string statusDesc;
    public string statusName;
    public Sprite statusIcon;

    [Header("# Status Data")]
    public int HP;
    public float Shield;
    public float Barrier;
    public int[] HP_Steps;
    public float[] Shield_Steps;
    public float[] Barrier_Steps;

    public int[] Count;

    [Header("# Status")]
    public GameObject statusTile;
}
