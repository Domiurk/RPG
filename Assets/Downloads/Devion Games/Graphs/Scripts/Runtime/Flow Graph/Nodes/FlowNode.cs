using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace DevionGames.Graphs
{
    [Serializable]
    public abstract class FlowNode : Node
    {
        [SerializeField]
        private List<Port> m_Ports= new();
        public List<Port> Ports => m_Ports;

        public List<Port> InputPorts
        {
            get
            {
                return m_Ports.Where(x=>x.direction == PortDirection.Input).ToList();
            }
        }

        public List<Port> OutputPorts
        {
            get
            {
                return m_Ports.Where(x => x.direction == PortDirection.Output).ToList();
            }
        }

        public FlowNode()
        {
  
        }

        public abstract object OnRequestValue(Port port);

        public T GetInputValue<T>(string portName, T defaultValue = default)
        {
            Port port = GetPort(portName);
            if (port == null || port.direction == PortDirection.Output)
            {
                throw new ArgumentException(
                    $"[{name}] No input port named `{portName}`"
                );
            }

            return port.GetValue(defaultValue);
        }

        public T GetOutputValue<T>(string portName)
        {
            Port port = GetPort(portName);
            if (port == null || port.direction == PortDirection.Input)
            {
                throw new ArgumentException(
                    $"<b>[{name}]</b> No output port named `{portName}`"
                );
            }

            return port.GetValue(default(T));
        }

        public Port GetPort(string fieldName)
        {
            return m_Ports.Find((port) => port.fieldName == fieldName);
        }

        public Port GetPort(int index)
        {
            return m_Ports[index];
        }

        public void AddPort(Port port)
        {
            m_Ports.Add(port);
            port.node = this;
        }

        public void DisconnectAllPorts()
        {
            foreach (Port port in m_Ports)
            {
                port.DisconnectAll();
            }
        }

        public override void OnAfterDeserialize()
        {
            for (int i = 0; i < m_Ports.Count; i++)
            {
                if (m_Ports[i].m_FieldTypeName == "String")
                    m_Ports[i].m_FieldTypeName = "System.String";

                m_Ports[i].node = this;
                for (int j = 0; j < m_Ports[i].Connections.Count; j++)
                {
                    Edge edge = m_Ports[i].Connections[j];
                    FlowNode connected = graph.nodes.Find(x => x.id == edge.nodeId) as FlowNode;
                    edge.port = connected.Ports.Find(x => x.fieldName == edge.fieldName);
                    m_Ports[i].Connections[j] = edge;
                }
            }
        }

    }
}