using RimWorld;
using Verse;
using CombatExtended;

namespace RimWorldColumns
{
    public class Building_TurretGunColumn : Building_TurretGun
    {
        public override bool ThreatDisabled(Thing other = null)
        {
            var compAmmo = this.TryGetComp<CompAmmoUser>();
            if (compAmmo != null && !compAmmo.HasAmmo)
                return true;

            return base.ThreatDisabled(other);
        }
    }
}
