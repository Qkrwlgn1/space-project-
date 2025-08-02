using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Status", menuName = "Scriptable Object/StatusData")]
public class StatusData : ScriptableObject
{
    public enum StatusType { HP, Damage, Speed, WeaponNumber, BulletSize}

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
    public float BulletSize;

    public int[] Status_INT;
    public float[] Status_FLOAT;

    [Header("# Status")]
    public GameObject statusTile;
}
