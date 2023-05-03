using System.Collections.Generic;
using System.Linq;
using Items;
using UnityEngine;

namespace Runtime.UI_Components
{
    public abstract class BaseContainer : MonoBehaviour, IName
    {
        public string Name => _name;

        [SerializeField] private string _name;


    }
}