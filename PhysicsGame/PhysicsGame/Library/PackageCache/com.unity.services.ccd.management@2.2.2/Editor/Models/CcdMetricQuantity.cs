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
using Unity.Services.Ccd.Management.Http;



namespace Unity.Services.Ccd.Management.Models
{
    /// <summary>
    /// CcdMetricQuantity model
    /// </summary>
    [Preserve]
    [DataContract(Name = "ccd.metricQuantity")]
    public class CcdMetricQuantity
    {
        /// <summary>
        /// Creates an instance of CcdMetricQuantity.
        /// </summary>
        /// <param name="quantity">quantity param</param>
        [Preserve]
        public CcdMetricQuantity(int quantity = default)
        {
            Quantity = quantity;
        }

        /// <summary>
        /// Parameter quantity of CcdMetricQuantity
        /// </summary>
        [Preserve]
        [DataMember(Name = "quantity", EmitDefaultValue = false)]
        public int Quantity{ get; }
    
        /// <summary>
        /// Formats a CcdMetricQuantity into a string of key-value pairs for use as a path parameter.
        /// </summary>
        /// <returns>Returns a string representation of the key-value pairs.</returns>
        internal string SerializeAsPathParam()
        {
            var serializedModel = "";

            serializedModel += "quantity," + Quantity.ToString();
            return serializedModel;
        }

        /// <summary>
        /// Returns a CcdMetricQuantity as a dictionary of key-value pairs for use as a query parameter.
        /// </summary>
        /// <returns>Returns a dictionary of string key-value pairs.</returns>
        internal Dictionary<string, string> GetAsQueryParam()
        {
            var dictionary = new Dictionary<string, string>();

            var quantityStringValue = Quantity.ToString();
            dictionary.Add("quantity", quantityStringValue);
            
            return dictionary;
        }
    }
}
