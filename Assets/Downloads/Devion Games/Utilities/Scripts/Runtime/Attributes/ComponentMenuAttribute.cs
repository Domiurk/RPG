using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    public sealed class ComponentMenu : Attribute
    {
        private string m_ComponentMenu;

        public string componentMenu => this.m_ComponentMenu;

        public ComponentMenu(string menuName)
        {
            this.m_ComponentMenu = menuName;
        }
    }
}