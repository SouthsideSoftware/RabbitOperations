namespace RabbitOperations.Utility.DesignByContract.Exceptions
{
    public class Invariant
    {
        public bool Condition { get; protected set; }
        public string PropertyName { get; protected set; }
        public string Message { get; protected set; }

        public Invariant(bool condition, string propertyName, string message)
        {
            Condition = condition;
            PropertyName = propertyName;
            Message = message;
        }

        #region Equals and GetHashCode implementations

        public override bool Equals(object obj)
        {
            Invariant invariant = obj as Invariant;
            if (invariant == null)
                return false;

            if (Condition != invariant.Condition)
            {
                return false;
            }
            if (PropertyName != invariant.PropertyName)
            {
                return false;
            }
            if (Message != invariant.Message)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 7;
                hash = (hash * 397) ^ Condition.GetHashCode();
                hash = (hash * 397) ^ PropertyName.GetHashCode();
                hash = (hash * 397) ^ Message.GetHashCode();

                return hash;
            }
        }

        #endregion
    }
}
