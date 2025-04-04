using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Unity.Services.Economy.Model
{
    /// <summary>
    /// The result from redeeming a Google Play Store purchase.
    /// </summary>
    public class RedeemGooglePlayPurchaseResult
    {
        /// <summary>Create an instance of the RedeemGooglePlayPurchaseResult class </summary>
        /// <param name="verification">The receipt verification details from the Google Play Store validation service.</param>
        /// <param name="rewards">The Rewards given in exchange for this purchase.</param>
        [Preserve]
        [JsonConstructor]
        public RedeemGooglePlayPurchaseResult(GoogleVerification verification, Rewards rewards)
        {
            Verification = verification;
            Rewards = rewards;
        }

        /// <summary>
        /// The receipt verification details from the Google Play Store validation service.
        /// </summary>
        [Preserve]
        public GoogleVerification Verification;

        /// <summary>
        /// The Rewards given in exchange for this purchase.
        /// </summary>
        [Preserve]
        public Rewards Rewards;
    }
}
