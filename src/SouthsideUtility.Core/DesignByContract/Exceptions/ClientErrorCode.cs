

namespace SouthsideUtility.Core.DesignByContract.Exceptions
{
    public enum ClientErrorCode
    {
        NotSpecified = 0,

        // 400's are rrrors during order process
        PromoCodeNoLongerValid = 400,
        OrderAlreadyPlaced = 401,
        CartNotEligibleForExpressCheckout = 402,
        CartLockedForCheckout = 403,

        // 500's are THD API errors
        ThdApiError = 500,
    }
}
