using UnityEngine;

namespace CherryStudio.Combat
{
    /// <summary>
    /// Simple demo player controller, using the infrastructure of the Attacker script.
    /// It will:
    /// - Apply a targeted attack when clicking on an entity
    /// - Apply a close attack when click is not on an entity
    /// - Apply a free ranged shot when clicking space (shooting forward)
    /// - Get larger when it kills
    /// - Get back to start scale when dead
    ///
    /// Some of the actions are set via editor. Use those freely to show the designers of your game what the entity does!
    /// </summary>
    public class DemoPlayer : Attacker
    {
        public Transform gunTip;
        private Camera mainCamera;
        private Vector3 startScale;

        private void Start()
        {
            startScale = transform.localScale;
        }

        public void ScaleSize(float delta)
        {
            transform.localScale = new Vector3(
                transform.localScale.x + delta,
                transform.localScale.y + delta,
                transform.localScale.z + delta);
        }

        public void ResetScaleSize()
        {
            transform.localScale = startScale;
        }

        protected override void Awake()
        {
            base.Awake();
            EntitiesLocator.Register<Attacker>(EntitiesLocator.Player, this);
            mainCamera = Camera.main;
        }

        protected void UpdatePlayer()
        {
            UpdateEntity();

            if (Input.GetMouseButtonDown(0))
            {
                var mouseRay = mainCamera.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(mouseRay.origin, mouseRay.direction, out var hitInfo);
                var mouseHitEntity = hitInfo.collider?.GetComponentInParent<Entity>() ?? hitInfo.collider?.GetComponentInChildren<Entity>();

                if (mouseHitEntity != null)
                {
                    var targetedAttack = GetRandomTargetedAttack();
                    DoTargetedAttack(targetedAttack, mouseHitEntity, gunTip.transform.position);
                }
                else if (!HasCloseAttackPending)
                {
                    var attack = GetRandomCloseAttack();
                    DoCloseAttack(attack);
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                var attack = GetRandomRangedAttack();
                DoRangedAttack(attack, gunTip.transform.position, gunTip.transform.forward);
            }
        }

        private void Update()
        {
            UpdatePlayer();
        }
    }
}
