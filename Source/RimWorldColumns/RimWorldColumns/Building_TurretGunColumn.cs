using RimWorld;
using Verse;
using Verse.AI;

namespace RimWorldColumns
{
    public class Building_TurretGunColumn : Building_TurretGun
    {
        public new bool ThreatDisabled(IAttackTargetSearcher other)
        {
            var compAmmo = this.TryGetComp<CompRefuelable>();
            if (compAmmo != null && !compAmmo.HasFuel)
                return true;

            return base.ThreatDisabled(other);
        }
    }
}
