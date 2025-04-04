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
    /// CcdUsage model
    /// </summary>
    [Preserve]
    [DataContract(Name = "ccd.usage")]
    public class CcdUsage
    {
        /// <summary>
        /// Creates an instance of CcdUsage.
        /// </summary>
        /// <param name="projectguid">projectguid param</param>
        /// <param name="quantity">quantity param</param>
        [Preserve]
        public CcdUsage(System.Guid projectguid = default, decimal quantity = default)
        {
            Projectguid = projectguid;
            Quantity = quantity;
        }

        /// <summary>
        /// Parameter projectguid of CcdUsage
        /// </summary>
        [Preserve]
        [DataMember(Name = "projectguid", EmitDefaultValue = false)]
        public System.Guid Projectguid{ get; }
        
        /// <summary>
        /// Parameter quantity of CcdUsage
        /// </summary>
        [Preserve]
        [DataMember(Name = "quantity", EmitDefaultValue = false)]
        public decimal Quantity{ get; }
    
        /// <summary>
        /// Formats a CcdUsage into a string of key-value pairs for use as a path parameter.
        /// </summary>
        /// <returns>Returns a string representation of the key-value pairs.</returns>
        internal string SerializeAsPathParam()
        {
            var serializedModel = "";

            if (Projectguid != null)
            {
                serializedModel += "projectguid," + Projectguid + ",";
            }
            serializedModel += "quantity," + Quantity.ToString();
            return serializedModel;
        }

        /// <summary>
        /// Returns a CcdUsage as a dictionary of key-value pairs for use as a query parameter.
        /// </summary>
        /// <returns>Returns a dictionary of string key-value pairs.</returns>
        internal Dictionary<string, string> GetAsQueryParam()
        {
            var dictionary = new Dictionary<string, string>();

            if (Projectguid != null)
            {
                var projectguidStringValue = Projectguid.ToString();
                dictionary.Add("projectguid", projectguidStringValue);
            }
            
            var quantityStringValue = Quantity.ToString();
            dictionary.Add("quantity", quantityStringValue);
            
            return dictionary;
        }
    }
}
