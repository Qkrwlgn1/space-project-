using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "Scriptable Object/AttackData")]
public class AttackStatus : ScriptableObject
{
    public enum AttackType { Damage, Speed, WeaponNumber }

    [Header("# Status Type")]

    public AttackType statusType;
    public int statusId;
    public string statusDesc;
    public string statusName;
    public Sprite statusIcon;

    [Header("# Status Data")]

    public float Damage;
    public float Speed;
    public int WeaponNumber;
    public float[] Damage_Steps;
    public float[] Speed_Steps;
    public int[] WeaponNumber_Steps;

    [Header("# Status")]
    public GameObject statusTile;
}
