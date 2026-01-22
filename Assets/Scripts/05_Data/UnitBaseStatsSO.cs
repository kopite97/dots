using UnityEngine;

[CreateAssetMenu(fileName = "UnitBaseStats",menuName = "RTS/UnitBaseStats")]
public class UnitBaseStatsSO : ScriptableObject
{
    [Header("Base Stats")] 
    public float MaxHealth = 100f;
    public float MoveSpeed = 5.0f;
    public float AttackRange = 10.0f;
    public float AttackInterval = 1.0f;
    public float BulletSpeed = 10.0f;
    public float AttackDamage = 50.0f;

}