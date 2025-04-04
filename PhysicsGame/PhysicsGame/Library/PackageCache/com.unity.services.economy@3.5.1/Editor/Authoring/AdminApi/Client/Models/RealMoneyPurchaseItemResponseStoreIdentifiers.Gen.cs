//-----------------------------------------------------------------------------
// <auto-generated>
//     This file was generated by the C# SDK Code Generator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//-----------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Scripting;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Unity.Services.Economy.Editor.Authoring.AdminApi.Client.Http;


namespace Unity.Services.Economy.Editor.Authoring.AdminApi.Client.Models
{
    /// <summary>
    /// Store identifiers.
    /// </summary>
    [Preserve]
    [DataContract(Name = "real_money_purchase_item_response_storeIdentifiers")]
    internal class RealMoneyPurchaseItemResponseStoreIdentifiers
    {
        /// <summary>
        /// Store identifiers.
        /// </summary>
        /// <param name="appleAppStore">Apple App Store identifier.</param>
        /// <param name="googlePlayStore">Google Play Store identifier.</param>
        [Preserve]
        public RealMoneyPurchaseItemResponseStoreIdentifiers(string appleAppStore = default, string googlePlayStore = default)
        {
            AppleAppStore = appleAppStore;
            GooglePlayStore = googlePlayStore;
        }

        /// <summary>
        /// Apple App Store identifier.
        /// </summary>
        [Preserve]
        [DataMember(Name = "appleAppStore", EmitDefaultValue = false)]
        public string AppleAppStore{ get; }

        /// <summary>
        /// Google Play Store identifier.
        /// </summary>
        [Preserve]
        [DataMember(Name = "googlePlayStore", EmitDefaultValue = false)]
        public string GooglePlayStore{ get; }

        /// <summary>
        /// Formats a RealMoneyPurchaseItemResponseStoreIdentifiers into a string of key-value pairs for use as a path parameter.
        /// </summary>
        /// <returns>Returns a string representation of the key-value pairs.</returns>
        internal string SerializeAsPathParam()
        {
            var serializedModel = "";

            if (AppleAppStore != null)
            {
                serializedModel += "appleAppStore," + AppleAppStore + ",";
            }
            if (GooglePlayStore != null)
            {
                serializedModel += "googlePlayStore," + GooglePlayStore;
            }
            return serializedModel;
        }

        /// <summary>
        /// Returns a RealMoneyPurchaseItemResponseStoreIdentifiers as a dictionary of key-value pairs for use as a query parameter.
        /// </summary>
        /// <returns>Returns a dictionary of string key-value pairs.</returns>
        internal Dictionary<string, string> GetAsQueryParam()
        {
            var dictionary = new Dictionary<string, string>();

            if (AppleAppStore != null)
            {
                var appleAppStoreStringValue = AppleAppStore.ToString();
                dictionary.Add("appleAppStore", appleAppStoreStringValue);
            }

            if (GooglePlayStore != null)
            {
                var googlePlayStoreStringValue = GooglePlayStore.ToString();
                dictionary.Add("googlePlayStore", googlePlayStoreStringValue);
            }

            return dictionary;
        }
    }
}
