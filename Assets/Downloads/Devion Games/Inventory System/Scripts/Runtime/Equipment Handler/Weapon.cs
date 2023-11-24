using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    public abstract class Weapon : VisibleItem
    {
        public override string[] Callbacks
            => new List<string>(base.Callbacks){
                "OnUse",
                "OnEndUse"
            }.ToArray();

        [Header("Activation:")]
        [InspectorLabel("Input Name")]
        [SerializeField]
        protected string m_ActivationInputName;
        [SerializeField]
        protected ActivationType m_ActivationType;

        private bool m_IsActive;
        protected bool IsActive
        {
            get => m_IsActive;
            private set{
                if(m_IsActive != value){
                    m_IsActive = value;
                    OnItemActivated(m_IsActive);
                }
            }
        }

        [Header("Use:")]
        [InspectorLabel("Input Name")]
        [SerializeField]
        protected string m_UseInputName = "Fire1";
        [SerializeField]
        protected StartType m_StartType;
        [SerializeField]
        protected StopType m_StopType;

        [Header("Animator IK:")]
        [SerializeField]
        protected Transform m_RightHandIKTarget;
        [SerializeField]
        protected float m_RightHandIKWeight = 1f;
        [SerializeField]
        protected float m_RightHandIKSpeed = 1f;
        protected float m_RightHandIKLerp;

        [SerializeField]
        protected Transform m_LeftHandIKTarget;
        [SerializeField]
        protected float m_LeftHandIKWeight = 1f;
        [SerializeField]
        protected float m_LeftHandIKSpeed = 1f;
        protected float m_LeftHandIKLerp;

        [Header("Animator States:")]
        [SerializeField]
        public int m_ItemID;
        [SerializeField]
        protected string m_IdleState = "Movement";
        [SerializeField]
        protected string m_UseState = "Sword Slash";

        protected bool m_InUse;
        protected float m_UseClipLength;

        private AnimatorStateInfo[] m_DefaultStates;

        public override void OnItemEquip(Item item)
        {
            base.OnItemEquip(item);

            if(m_ActivationType == ActivationType.Automatic){
                IsActive = true;
            }
        }

        public override void OnItemUnEquip(Item item)
        {
            base.OnItemUnEquip(item);
            IsActive = false;
        }

        protected override void Update()
        {
            if(m_Pause || !m_Handler.enabled || UnityTools.IsPointerOverUI() ||
               !m_CharacterAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Default") ||
               ItemSlot.dragObject != null){
                m_CharacterAnimator.SetBool(propertyItemUse, false);
                return;
            }

            switch(m_ActivationType){
                case ActivationType.Button:
                    IsActive = Input.GetButton(m_ActivationInputName);
                    break;
                case ActivationType.Toggle:
                    if(Input.GetButtonDown(m_ActivationInputName))
                        IsActive = !IsActive;
                    break;
            }

            if(!IsActive){
                return;
            }

            if(string.IsNullOrEmpty(m_UseInputName))
                return;

            if(m_StartType != StartType.Down || !Input.GetButtonDown(m_UseInputName)){
                if(m_StopType == StopType.Up &&
                   (Input.GetButtonUp(m_UseInputName) || !Input.GetButton(m_UseInputName))){
                    TryStopUse();
                }
            }
            else if(!m_InUse && m_StartType == StartType.Down){
                TryStartUse();
            }

            if(m_StartType == StartType.Press && Input.GetButton(m_UseInputName)){
                TryStartUse();
            }

            if(!IsActive || !m_CharacterAnimator.GetCurrentAnimatorStateInfo(0).IsName(m_IdleState) ||
               m_InUse){
                m_RightHandIKLerp = 0f;
                m_LeftHandIKLerp = 0f;
                return;
            }

            m_RightHandIKLerp = Mathf.Lerp(m_RightHandIKLerp, 1f, Time.deltaTime * m_RightHandIKSpeed);
            m_LeftHandIKLerp = Mathf.Lerp(m_LeftHandIKLerp, 1f, Time.deltaTime * m_LeftHandIKSpeed);
        }

        private void TryStartUse()
        {
            if(!m_InUse && CanUse()){
                StartUse();
            }

            if(m_InUse)
                m_CharacterAnimator.SetBool("Item Use", true);
        }

        protected virtual bool CanUse()
        {
            int layers = m_CharacterAnimator.layerCount;

            for(int i = 0; i < layers; i++){
                if(m_CharacterAnimator.HasState(i, Animator.StringToHash(m_UseState)) &&
                   (m_CharacterAnimator.GetCurrentAnimatorStateInfo(i).IsName(m_UseState) ||
                    m_CharacterAnimator.IsInTransition(i))){
                    return false;
                }
            }

            if(!m_CharacterAnimator.GetCurrentAnimatorStateInfo(1).IsTag("Interruptable"))
                return false;

            Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit)){
                BaseTrigger trigger = hit.collider.GetComponentInParent<BaseTrigger>();

                return trigger == null || !trigger.enabled;
            }

            return true;
        }

        private void TryStopUse()
        {
            if(m_InUse && CanUnuse()){
                StopUse();
            }
        }

        protected virtual bool CanUnuse()
        {
            return true;
        }

        protected void StopUse()
        {
            if(IsActive){
                OnStopUse();
                m_InUse = false;
                m_CharacterAnimator.CrossFadeInFixedTime(m_IdleState, 0.15f);
                CallbackEventData data = new CallbackEventData();
                data.AddData("Item", m_CurrentEquipedItem);
                Execute("OnEndUse", data);
                m_CharacterAnimator.SetBool("Item Use", false);
            }
        }

        protected virtual void OnStopUse() { }

        protected void StartUse()
        {
            OnStartUse();
            m_InUse = true;
            Use();
        }

        protected virtual void OnStartUse() { }

        protected virtual void Use()
        {
            m_CharacterAnimator.CrossFadeInFixedTime(m_UseState, 0.15f);
            m_CharacterAnimator.Update(0f);
            CallbackEventData data = new CallbackEventData();
            data.AddData("Item", m_CurrentEquipedItem);
            Execute("OnUse", data);
            m_CharacterAnimator.SetBool("Item Use", true);
        }

        private void UseItem()
        {
            if(m_IsActive)
                (m_CurrentEquipedItem as EquipmentItem)?.Use();
        }

        private void OnEndUse()
        {
            if(m_InUse && m_StopType == StopType.EndUseEvent)
                StopUse();
        }

        protected bool m_Pause;
        private static readonly int propertyItemUse = Animator.StringToHash("Item Use");
        private static readonly int propertyItemID = Animator.StringToHash("Item ID");

        private void PauseItemUpdate(bool state)
        {
            m_Pause = state;

            ItemContainer[] containers = UIWidgets.WidgetUtility.FindAll<ItemContainer>();
            foreach(ItemContainer container in containers)
                container.Lock(m_Pause);
        }

        protected virtual void OnItemActivated(bool activated)
        {
            if(activated){
                m_CharacterAnimator.Update(1f);
                m_DefaultStates = new AnimatorStateInfo[m_CharacterAnimator.layerCount];

                for(int j = 0; j < m_CharacterAnimator.layerCount; j++){
                    AnimatorStateInfo stateInfo = m_CharacterAnimator.GetCurrentAnimatorStateInfo(j);
                    m_DefaultStates[j] = stateInfo;
                }

                m_CharacterAnimator.SetInteger("Item ID", m_ItemID);
                m_CharacterAnimator.CrossFadeInFixedTime(m_IdleState, 0.15f);
                m_InUse = false;
            }
            else{
                m_CharacterAnimator.SetBool(propertyItemUse, false);
                m_CharacterAnimator.SetInteger(propertyItemID, 0);

                foreach(AnimatorStateInfo stateInfo in m_DefaultStates){
                    m_CharacterAnimator.CrossFadeInFixedTime(stateInfo.shortNameHash, 0.15f);
                    m_CharacterAnimator.Update(0f);
                }
            }
        }

        protected virtual void OnAnimatorIK(int layerIndex)
        {
            if(!IsActive || layerIndex == 0 ||
               !m_CharacterAnimator.GetCurrentAnimatorStateInfo(0).IsName(m_IdleState) || m_InUse){
                return;
            }

            if(m_RightHandIKTarget != null){
                m_CharacterAnimator.SetIKPosition(AvatarIKGoal.RightHand, m_RightHandIKTarget.position);
                m_CharacterAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand,
                                                             m_RightHandIKWeight * m_RightHandIKLerp);

                m_CharacterAnimator.SetIKRotation(AvatarIKGoal.RightHand, m_RightHandIKTarget.rotation);
                m_CharacterAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand,
                                                             m_RightHandIKWeight * m_RightHandIKLerp);
            }

            if(m_LeftHandIKTarget != null){
                m_CharacterAnimator.SetIKPosition(AvatarIKGoal.LeftHand, m_LeftHandIKTarget.position);
                m_CharacterAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand,
                                                             m_LeftHandIKWeight * m_LeftHandIKLerp);

                m_CharacterAnimator.SetIKRotation(AvatarIKGoal.LeftHand, m_LeftHandIKTarget.rotation);
                m_CharacterAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand,
                                                             m_LeftHandIKWeight * m_LeftHandIKLerp);
            }
        }

        public enum ActivationType
        {
            Automatic,
            Button,
            Toggle
        }

        public enum StartType
        {
            Automatic,
            Down,
            Press
        }

        public enum StopType
        {
            Up,
            EndUseEvent,
            Manual,
        }
    }
}