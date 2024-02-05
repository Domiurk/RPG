using System.Linq;
using UnityEngine;

namespace DevionGames
{
    public class Sequence
    {
        private ActionStatus m_Status;
        public ActionStatus Status => m_Status;

        private int m_ActionIndex;
        private ActionStatus m_ActionStatus;
        private readonly IAction[] m_AllActions;
        private IAction[] m_Actions;

        public Sequence(GameObject gameObject, PlayerInfo playerInfo, Blackboard blackboard, IAction[] actions)
        {
            m_AllActions = actions;

            foreach(IAction iAction in m_AllActions)
                iAction.Initialize(gameObject, playerInfo, blackboard);

            m_Status = ActionStatus.Inactive;
            m_ActionStatus = ActionStatus.Inactive;
        }

        public void Start()
        {
            m_Actions = m_AllActions.Where(x => x.isActiveAndEnabled).ToArray();

            foreach(IAction action in m_Actions){
                action.OnSequenceStart();
            }

            m_ActionIndex = 0;
            m_Status = ActionStatus.Running;
        }

        public void Stop()
        {
            foreach(IAction action in m_Actions){
                action.OnSequenceEnd();
            }

            m_Status = ActionStatus.Inactive;
        }

        public void Interrupt()
        {
            for(int i = 0; i <= m_ActionIndex; i++){
                if(i < m_Actions.Length)
                    m_Actions[i].OnInterrupt();
            }
        }

        public void Update()
        {
            foreach(IAction action in m_Actions){
                action.Update();
            }
        }

        public bool Tick()
        {
            if(m_Status == ActionStatus.Running){
                if(m_ActionIndex >= m_Actions.Length){
                    m_ActionIndex = 0;
                }

                while(m_ActionIndex < m_Actions.Length){
                    if(m_ActionStatus != ActionStatus.Running){
                        m_Actions[m_ActionIndex].OnStart();
                    }

                    m_ActionStatus = m_Actions[m_ActionIndex].OnUpdate();

                    if(m_ActionStatus != ActionStatus.Running){
                        m_Actions[m_ActionIndex].OnEnd();
                    }

                    if(m_ActionStatus == ActionStatus.Success){
                        ++m_ActionIndex;
                    }
                    else{
                        break;
                    }
                }

                m_Status = m_ActionStatus;

                if(m_Status != ActionStatus.Running){
                    foreach(IAction action in m_Actions){
                        action.OnSequenceEnd();
                    }
                }
            }

            return m_Status == ActionStatus.Running;
        }
    }
}