namespace CherryStudio.Combat
{
    /// <summary>
    /// An enemy that when it's attacked, it will start to attack the player
    /// </summary>
    public class FightingBackEnemy : AttackerEnemy
    {
        protected override void AfterDamaged()
        {
            base.AfterDamaged();

            AttackPlayer();
        }
    }
}
