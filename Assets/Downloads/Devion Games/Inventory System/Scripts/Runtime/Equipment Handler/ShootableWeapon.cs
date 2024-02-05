using UnityEngine;

namespace DevionGames.InventorySystem
{
    public class ShootableWeapon : Weapon
    {
        [SerializeField]
        private string m_ReloadState="Bow Reload";
        [Header("Behaviour:")]
        [SerializeField]
        private int m_ReloadClipSize = 1;
        [SerializeField]
        private bool m_ResetClipSize;

        [Header("Projectile:")]

        [SerializeField]
        protected GameObject m_Projectile;
        [SerializeField]
        protected ProjectileVisibility m_ProjectileVisibility = ProjectileVisibility.Always;
        [SerializeField]
        protected float m_ProjectileSpeed = 40f;
        [SerializeField]
        protected Transform m_FirePoint;
        [SerializeField]
        protected Transform m_ReloadPoint;
        [ItemPicker(true)]
        [SerializeField]
        protected Item m_ProjectileItem;
        [SerializeField]
        protected string m_ProjectileItemWindow = "Inventory";

        protected GameObject m_CurrentProjectile;
        private int m_CurrentClipSize;
        private bool m_IsReloading;
        private ItemContainer[] m_ItemContainers;

        protected override void OnItemActivated(bool activated)
        {
            base.OnItemActivated(activated);
            if (activated)
            {
                m_IsReloading = false;
               
                if (m_CurrentClipSize > 0) {
                    CreateCurrentProjectile();
                }
            }
            else {
                if (m_ResetClipSize)
                {
                    if (m_CurrentClipSize > 0)
                    {
                        Item item = Instantiate(m_ProjectileItem);
                        item.Stack = m_CurrentClipSize;
                        ItemContainer.AddItem(m_ProjectileItemWindow, item);
                    }
                    m_CurrentClipSize = 0;
                }
                if (m_CurrentProjectile != null) {
                    Destroy(m_CurrentProjectile);
                }
            }
        }

        protected override void Update()
        {
            base.Update();
            if(IsActive)
                TryReload();
        }


        protected override void Use()
        {
            if (m_CurrentClipSize == 0)
            {
                TryReload();
            }else {
                base.Use();
                if (m_ProjectileVisibility == ProjectileVisibility.OnFire || m_CurrentProjectile == null)
                {
                    CreateCurrentProjectile();
                }

                m_CurrentProjectile.transform.position = m_FirePoint.transform.position;
                m_CurrentProjectile.transform.parent = null;
                Rigidbody projectileRigidbody = m_CurrentProjectile.GetComponent<Rigidbody>();
                m_CurrentProjectile.GetComponent<Projectile>().enabled = true;
                projectileRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
                RaycastHit hit;
                if (Physics.Raycast(m_CameraTransform.position, m_CameraTransform.forward, out hit, float.PositiveInfinity))
                {
                    if(Vector3.Distance(m_CurrentProjectile.transform.position,hit.point)>1f)
                        projectileRigidbody.transform.LookAt(hit.point);

                }
                else
                {
                    projectileRigidbody.transform.forward = m_CameraTransform.forward;
                }

                projectileRigidbody.velocity = projectileRigidbody.transform.forward * m_ProjectileSpeed;
              
                m_CurrentProjectile = null;
            }
        }


        protected override bool CanUse()
        {
            return m_CurrentClipSize > 0;
        }

        private bool TryReload() {
            if (!m_IsReloading && m_CurrentClipSize == 0 ) {
                if (ItemContainer.HasItem(m_ProjectileItemWindow, m_ProjectileItem, m_ReloadClipSize))
                {
                    ItemContainer.RemoveItem(m_ProjectileItemWindow, m_ProjectileItem, m_ReloadClipSize);
                    m_IsReloading = true;
                    m_CharacterAnimator.CrossFadeInFixedTime(m_ReloadState, 0.2f);
                    if (m_ProjectileVisibility == ProjectileVisibility.Always)
                    {
                        CreateCurrentProjectile();
                    }
                    return true;
                }
                if(Input.GetButtonDown(m_ActivationInputName))
                    InventoryManager.Notifications.missingItem.Show(m_ProjectileItem.Name);
            }
            return false;
        }

        protected override void OnStopUse()
        {
            m_CurrentClipSize -= 1;     
        }

        private void OnEndReload()
        {
            if (m_IsReloading)
            {
                m_IsReloading = false;
                m_CurrentClipSize = m_ReloadClipSize;
                m_CharacterAnimator.CrossFadeInFixedTime(m_IdleState, 0.2f);
                m_CurrentProjectile.transform.SetParent(m_FirePoint, false);
            }
        }

        protected virtual void CreateCurrentProjectile()
        {
            m_CurrentProjectile = Instantiate(m_Projectile);
            IgnoreCollision(m_CurrentProjectile);
            m_CurrentProjectile.transform.SetParent(m_ReloadPoint, false);
            m_CurrentProjectile.SetActive(true);
        }

        public enum ProjectileVisibility
        {
            OnFire,
            Always
        }
    }
}