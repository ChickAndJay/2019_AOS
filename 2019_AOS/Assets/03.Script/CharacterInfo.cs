using UnityEngine;

public enum FightType
{
    Shelly,
    Colt,
    Nita,
    Barley
}

[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class CharacterInfo : ScriptableObject {
    public FightType _fightType;
    public int _health;
    public int _damage;
    public float _bulletVelocity;
    public float _bulletRange;
    public int _ammo;
    public float _reloadTime;

    #region Skill variable
    public int _skillEnergyLimit;
    public float _skillRange;
    public float _skillDamage;
    #endregion

}
