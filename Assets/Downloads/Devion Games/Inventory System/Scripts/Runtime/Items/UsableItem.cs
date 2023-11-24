using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DevionGames.InventorySystem.ItemActions;
using System.Linq;

namespace DevionGames.InventorySystem
{
    [System.Serializable]
	public class UsableItem : Item
	{
        [SerializeField]
        private bool m_UseCategoryCooldown = true;
        [SerializeField]
        private float m_Cooldown = 1f;
        public float Cooldown => m_UseCategoryCooldown ? Category.Cooldown : m_Cooldown;

        [SerializeReference]
        public List<Action> actions = new();

        private Sequence m_ActionSequence;
        private IEnumerator m_ActionBehavior;

        protected override void OnEnable()
        {
            base.OnEnable();

            foreach(Action a in actions) {
                if (a is ItemAction action)
                    action.item = this;
            }
        }

        public override void Use()
        {
            m_ActionSequence ??= new Sequence(InventoryManager.current.PlayerInfo.gameObject,
                                                   InventoryManager.current.PlayerInfo,
                                                   InventoryManager.current.PlayerInfo.gameObject.GetComponent<Blackboard>(),
                                                   actions.Cast<IAction>().ToArray());

            if (m_ActionBehavior != null) {
                UnityTools.StopCoroutine(m_ActionBehavior);
            }
            m_ActionBehavior = SequenceCoroutine();
            UnityTools.StartCoroutine(m_ActionBehavior);
        }

        protected IEnumerator SequenceCoroutine() {
            m_ActionSequence.Start();
            while (m_ActionSequence.Tick()) {
                yield return null;
            }
        }
    }
}