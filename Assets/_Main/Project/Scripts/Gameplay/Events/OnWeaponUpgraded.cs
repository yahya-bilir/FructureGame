using System;
using UI;
using WeaponSystem;

namespace Events
{
    public class OnWeaponUpgraded
    {
        public WeaponStagesSO Stage { get; set; }
        public int Damage { get; set; }
        public int BaseDamage { get; set; }
        public float AttackSpeed { get; set; }
        public int Level { get; set; }
        public ObjectUIIdentifierSO ObjectUIIdentifierSo { get; set; }

        public OnWeaponUpgraded(WeaponStagesSO weaponStagesSo, int damage, int baseDamage, float attackSpeed, int level, ObjectUIIdentifierSO objectUIIdentifierSo)
        {
            Stage = weaponStagesSo;
            Damage = damage;
            BaseDamage = baseDamage;
            AttackSpeed = attackSpeed;
            Level = level;
            ObjectUIIdentifierSo = objectUIIdentifierSo;
        }
        
    }
}