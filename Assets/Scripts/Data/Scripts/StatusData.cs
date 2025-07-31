using UnityEngine;

[CreateAssetMenu(fileName = "Status", menuName = "Scriptable Object/StatusData")]
public class StatusData : ScriptableObject
{
    public enum StatusType { HP, Damage, Speed, WeaponNumber }

    [Header("# Status Type")]

    public StatusType statusType;
    public int statusId;
    public string statusName;
    [TextArea]
    public string statusDesc;
    public Sprite statusIcon;

    [Header("# Status Data")]

    public int HP;
    public int Damage;
    public float Speed;
    public int WeaponNumber;

    public int[] HP_Steps;
    public int[] Damage_Steps;
    public float[] Speed_Steps;
    public int[] WeaponNumber_Steps;

    [Header("# Status")]
    public GameObject statusTile;
}
