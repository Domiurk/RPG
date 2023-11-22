namespace DevionGames.InventorySystem
{
    public class CurrencyPickerAttribute : PickerAttribute
    {

        public CurrencyPickerAttribute() : this(false) { }

        public CurrencyPickerAttribute(bool utility) : base(utility) { }
    }
}