namespace MyMediaCollection.Interfaces
{
    public interface IValidatable
    {

        #region Methods

        void Validate(string memberName, object value);

        #endregion

    }
}
