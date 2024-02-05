namespace DevionGames.InventorySystem
{
    [System.Serializable]
    public class Currency : Item
    {
        public override int MaxStack => int.MaxValue;

        public CurrencyConversion[] currencyConversions;

       
    }
}