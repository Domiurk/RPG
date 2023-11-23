using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DevionGames
{
    public class Blackboard : MonoBehaviour
    {
        [SerializeReference] protected List<Variable> m_Variables = new List<Variable>();

        public T GetValue<T>(string name)
        {
            return GetValue<T>(GetVariable(name));
        }

        public T GetValue<T>(Variable variable)
        {
            if(variable != null){
                if(!variable.isShared)
                    return (T)variable.RawValue;

                Variable p = GetVariable(variable.name);
                if(p != null)
                    return (T)p.RawValue;
            }

            return default(T);
        }

        public void SetValue<T>(string nameValue, object value)
        {
            Variable variable = GetVariable(nameValue);

            if(variable != null){
                variable.RawValue = value;
            }
            else{
                AddVariable<T>(nameValue, value);
            }
        }

        public void SetValue(string nameValue, object value, Type type)
        {
            Variable variable = GetVariable(nameValue);

            if(variable != null){
                variable.RawValue = value;
            }
            else{
                AddVariable(nameValue, value, type);
            }
        }

        public bool DeleteVariable(string nameVariable)
        {
            return m_Variables.RemoveAll(x => x.name == nameVariable) > 0;
        }

        public Variable GetVariable(string nameVariable)
        {
            return m_Variables.FirstOrDefault(x => x.name == nameVariable);
        }

        public void AddVariable(Variable variable)
        {
            if(GetVariable(variable.name) != null){
                Debug.LogWarning("Variable with the same name (" + name + ") already exists!");
                return;
            }

            m_Variables.Add(variable);
        }

        public Variable AddVariable<T>(string name, object value)
        {
            return AddVariable(name, value, typeof(T));
        }

        public Variable AddVariable(string name, object value, Type type)
        {
            if(GetVariable(name) != null){
                Debug.LogWarning("Variable with the same name (" + name + ") already exists!");
                return null;
            }

            Variable variable = null;

            if(typeof(bool).IsAssignableFrom(type)){
                variable = new BoolVariable(name);
            }
            else if(typeof(float).IsAssignableFrom(type)){
                variable = new FloatVariable(name);
            }
            else if(typeof(Color).IsAssignableFrom(type)){
                variable = new ColorVariable(name);
            }
            else if(typeof(GameObject).IsAssignableFrom(type)){
                variable = new GameObjectVariable(name);
            }
            else if(typeof(int).IsAssignableFrom(type)){
                variable = new IntVariable(name);
            }
            else if(typeof(Object).IsAssignableFrom(type)){
                variable = new ObjectVariable(name);
            }
            else if(typeof(string).IsAssignableFrom(type)){
                variable = new StringVariable(name);
            }
            else if(typeof(Vector2).IsAssignableFrom(type)){
                variable = new Vector2Variable(name);
            }
            else if(typeof(Vector3).IsAssignableFrom(type)){
                variable = new Vector3Variable(name);
            }
            else if(typeof(ArrayList).IsAssignableFrom(type)){
                variable = new ArrayListVariable(name);
            }

            if(variable != null){
                variable.RawValue = value;
                m_Variables.Add(variable);
            }
            else{
                Debug.LogWarning("Variable type (" + type + ") is not supported.");
            }

            return variable;
        }
    }
}