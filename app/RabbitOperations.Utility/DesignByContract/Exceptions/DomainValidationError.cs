namespace RabbitOperations.Utility.DesignByContract.Exceptions
{
    public class DomainValidationError
    {
        public string PropertyName { get; set; }
        public string Message { get; set; }

        public DomainValidationError(string propertyName, string message)
        {
            Message = message;
            PropertyName = propertyName;
        }

        public DomainValidationError(string message)
        {
            Message = message;
        }

        #region Equals and GetHashCode

        public override bool Equals(object obj)
        {
            DomainValidationError domainValidationError = obj as DomainValidationError;
            if (domainValidationError == null)
                return false;
            if (PropertyName != domainValidationError.PropertyName)
                return false;
            if (Message != domainValidationError.Message)
                return false;
            return true;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap 
            {
                int hash = 17;
                hash = hash * 23 + PropertyName.GetHashCode();
                hash = hash * 23 + Message.GetHashCode();
                return hash;
            }
        }

        #endregion Equals and GetHashCode
    }
}
