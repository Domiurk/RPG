﻿namespace DevionGames.InventorySystem
{
    [System.Serializable]
    public class CurrencyConversion
    {
        public float factor = 1.0f;
        [CurrencyPicker(true)]
        public Currency currency;
    }
}
