using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine.Scripting;

namespace SimpleGraphQL
{
    [PublicAPI]
    [Serializable]
    public class Query
    {
        /// <summary>
        /// The filename that this query is located in.
        /// This is mostly used for searching and identification purposes, and is not
        /// necessarily needed for dynamically created queries.
        /// </summary>
        [CanBeNull]
        public string FileName;

        /// <summary>
        /// The operation name of this query.
        /// It may be null, in which case it should be the only anonymous query in this file.
        /// </summary>
        [CanBeNull]
        public string OperationName;

        /// <summary>
        /// The type of query this is.
        /// </summary>
        public OperationType OperationType;

        /// <summary>
        /// The actual query itself.
        /// </summary>
        public string Source;

        public override string ToString()
        {
            return $"{FileName}:{OperationName}:{OperationType}";
        }
    }

    // We just need something we can serialize
    [Preserve]
    public class Request
    {
        [Preserve]
        [DataMember(Name = "query")]
        public string query { get; set; }

        [Preserve]
        [DataMember(Name = "operationName")]
        public string operationName { get; set; }

        [Preserve]
        [DataMember(Name = "variables")]
        public Dictionary<string, object> variables { get; set; }

        [Preserve]
        public Request() {}
    }

    [PublicAPI]
    public static class QueryExtensions
    {
        public static byte[] ToBytes(this Query query, Dictionary<string, object> variables = null)
        {
            return Encoding.UTF8.GetBytes(ToJson(query, variables));
        }

        public static string ToJson(this Query query, Dictionary<string, object> variables = null,
            bool prettyPrint = false)
        {
            return JsonConvert.SerializeObject
            (
                new Request
                {
                    query = query.Source,
                    operationName = query.OperationName,
                    variables = variables
                },
                prettyPrint ? Formatting.Indented : Formatting.None,
                new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore}
            );
        }
    }

    [PublicAPI]
    [Serializable]
    public enum OperationType
    {
        Query,
        Mutation,
        Subscription
    }
}