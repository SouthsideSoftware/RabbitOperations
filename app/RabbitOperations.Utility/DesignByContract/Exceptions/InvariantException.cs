using System;
using System.Collections.Generic;
using System.Linq;

namespace RabbitOperations.Utility.DesignByContract.Exceptions
{
    public class InvariantException : DesignByContractException
    {
        private readonly IList<DomainValidationError> errors = new List<DomainValidationError>();

        public InvariantException(string message, ClientErrorCode clientErrorCode = ClientErrorCode.NotSpecified)
            : base(message)
        {
            ClientErrorCode = clientErrorCode;
        }

        public InvariantException(DomainValidationError error, string message, ClientErrorCode clientErrorCode = ClientErrorCode.NotSpecified)
            : base(message)
        {
            if (error == null)
            {
                throw new NullReferenceException("Domain validation error cannot be null.");
            }

            ClientErrorCode = clientErrorCode;
            errors = new List<DomainValidationError> {error};
        }

        public InvariantException(DomainValidationError error, ClientErrorCode clientErrorCode = ClientErrorCode.NotSpecified)
            : base(error.Message)
        {
            if (error == null)
            {
                throw new NullReferenceException("Domain validation error cannot be null.");
            }

            ClientErrorCode = clientErrorCode;
            errors = new List<DomainValidationError> {error};
        }

        public InvariantException(string propertyName, string message, ClientErrorCode clientErrorCode = ClientErrorCode.NotSpecified)
            : base(message)
        {
            Verify.RequireStringNotNullOrWhitespace(propertyName, "propertyName");

            ClientErrorCode = clientErrorCode;
            errors = new List<DomainValidationError> {new DomainValidationError(propertyName, message)};
        }

        public ClientErrorCode ClientErrorCode { get; protected set; }

        public override string HelpLink
        {
            get
            {
                if (errors != null && errors.Count > 0)
                {
                    return string.Join("; ", errors.Select(e => e.Message));
                }
                return base.HelpLink;
            }
            set { base.HelpLink = value; }
        }

        public IList<DomainValidationError> Errors
        {
            get
            {
                if (errors.Count == 0)
                {
                    errors.Add(new DomainValidationError("__form", base.Message));
                }
                return errors;
            }
        }

        public void Add(DomainValidationError error)
        {
            if (error == null)
            {
                throw new NullReferenceException("Domain validation error cannot be null.");
            }

            errors.Add(error);
        }

        public string ToLoggableString()
        {
            if (errors != null)
            {
                var message = string.Format("{0} {1}", string.Join(", ", errors.Select(e => e.Message)), base.ToString());
                return message;
            }

            return base.ToString();
        }
    }
}