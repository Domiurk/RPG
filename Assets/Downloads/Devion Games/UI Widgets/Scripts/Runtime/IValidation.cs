namespace DevionGames.UIWidgets{
	public interface IValidation<in T> {
		bool Validate(T item);
	}
}