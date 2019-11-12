// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Gov.Lclb.Cllb.Interfaces
{
    using Microsoft.Rest;
    using Models;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for Adoxiocorporateroles.
    /// </summary>
    public static partial class AdoxiocorporaterolesExtensions
    {
            /// <summary>
            /// Get entities from adoxio_corporateroles
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='orderby'>
            /// Order items by property values
            /// </param>
            /// <param name='select'>
            /// Select properties to be returned
            /// </param>
            /// <param name='expand'>
            /// Expand related entities
            /// </param>
            public static MicrosoftDynamicsCRMadoxioCorporateroleCollection Get(this IAdoxiocorporateroles operations, IList<string> orderby = default(IList<string>), IList<string> select = default(IList<string>), IList<string> expand = default(IList<string>))
            {
                return operations.GetAsync(orderby, select, expand).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Get entities from adoxio_corporateroles
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='orderby'>
            /// Order items by property values
            /// </param>
            /// <param name='select'>
            /// Select properties to be returned
            /// </param>
            /// <param name='expand'>
            /// Expand related entities
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<MicrosoftDynamicsCRMadoxioCorporateroleCollection> GetAsync(this IAdoxiocorporateroles operations, IList<string> orderby = default(IList<string>), IList<string> select = default(IList<string>), IList<string> expand = default(IList<string>), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetWithHttpMessagesAsync(orderby, select, expand, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Get entities from adoxio_corporateroles
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='orderby'>
            /// Order items by property values
            /// </param>
            /// <param name='select'>
            /// Select properties to be returned
            /// </param>
            /// <param name='expand'>
            /// Expand related entities
            /// </param>
            /// <param name='customHeaders'>
            /// Headers that will be added to request.
            /// </param>
            public static HttpOperationResponse<MicrosoftDynamicsCRMadoxioCorporateroleCollection> GetWithHttpMessages(this IAdoxiocorporateroles operations, IList<string> orderby = default(IList<string>), IList<string> select = default(IList<string>), IList<string> expand = default(IList<string>), Dictionary<string, List<string>> customHeaders = null)
            {
                return operations.GetWithHttpMessagesAsync(orderby, select, expand, customHeaders, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Add new entity to adoxio_corporateroles
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='body'>
            /// New entity
            /// </param>
            /// <param name='prefer'>
            /// Required in order for the service to return a JSON representation of the
            /// object.
            /// </param>
            public static MicrosoftDynamicsCRMadoxioCorporaterole Create(this IAdoxiocorporateroles operations, MicrosoftDynamicsCRMadoxioCorporaterole body, string prefer = "return=representation")
            {
                return operations.CreateAsync(body, prefer).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Add new entity to adoxio_corporateroles
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='body'>
            /// New entity
            /// </param>
            /// <param name='prefer'>
            /// Required in order for the service to return a JSON representation of the
            /// object.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<MicrosoftDynamicsCRMadoxioCorporaterole> CreateAsync(this IAdoxiocorporateroles operations, MicrosoftDynamicsCRMadoxioCorporaterole body, string prefer = "return=representation", CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.CreateWithHttpMessagesAsync(body, prefer, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Add new entity to adoxio_corporateroles
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='body'>
            /// New entity
            /// </param>
            /// <param name='prefer'>
            /// Required in order for the service to return a JSON representation of the
            /// object.
            /// </param>
            /// <param name='customHeaders'>
            /// Headers that will be added to request.
            /// </param>
            public static HttpOperationResponse<MicrosoftDynamicsCRMadoxioCorporaterole> CreateWithHttpMessages(this IAdoxiocorporateroles operations, MicrosoftDynamicsCRMadoxioCorporaterole body, string prefer = "return=representation", Dictionary<string, List<string>> customHeaders = null)
            {
                return operations.CreateWithHttpMessagesAsync(body, prefer, customHeaders, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Get entity from adoxio_corporateroles by key
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='adoxioCorporateroleid'>
            /// key: adoxio_corporateroleid of adoxio_corporaterole
            /// </param>
            /// <param name='select'>
            /// Select properties to be returned
            /// </param>
            /// <param name='expand'>
            /// Expand related entities
            /// </param>
            public static MicrosoftDynamicsCRMadoxioCorporaterole CorporaterolesByKey(this IAdoxiocorporateroles operations, System.Guid adoxioCorporateroleid, IList<string> select = default(IList<string>), IList<string> expand = default(IList<string>))
            {
                return operations.CorporaterolesByKeyAsync(adoxioCorporateroleid, select, expand).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Get entity from adoxio_corporateroles by key
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='adoxioCorporateroleid'>
            /// key: adoxio_corporateroleid of adoxio_corporaterole
            /// </param>
            /// <param name='select'>
            /// Select properties to be returned
            /// </param>
            /// <param name='expand'>
            /// Expand related entities
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<MicrosoftDynamicsCRMadoxioCorporaterole> CorporaterolesByKeyAsync(this IAdoxiocorporateroles operations, System.Guid adoxioCorporateroleid, IList<string> select = default(IList<string>), IList<string> expand = default(IList<string>), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.CorporaterolesByKeyWithHttpMessagesAsync(adoxioCorporateroleid, select, expand, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Get entity from adoxio_corporateroles by key
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='adoxioCorporateroleid'>
            /// key: adoxio_corporateroleid of adoxio_corporaterole
            /// </param>
            /// <param name='select'>
            /// Select properties to be returned
            /// </param>
            /// <param name='expand'>
            /// Expand related entities
            /// </param>
            /// <param name='customHeaders'>
            /// Headers that will be added to request.
            /// </param>
            public static HttpOperationResponse<MicrosoftDynamicsCRMadoxioCorporaterole> CorporaterolesByKeyWithHttpMessages(this IAdoxiocorporateroles operations, System.Guid adoxioCorporateroleid, IList<string> select = default(IList<string>), IList<string> expand = default(IList<string>), Dictionary<string, List<string>> customHeaders = null)
            {
                return operations.CorporaterolesByKeyWithHttpMessagesAsync(adoxioCorporateroleid, select, expand, customHeaders, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Update entity in adoxio_corporateroles
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='adoxioCorporateroleid'>
            /// key: adoxio_corporateroleid of adoxio_corporaterole
            /// </param>
            /// <param name='body'>
            /// New property values
            /// </param>
            public static void CorporaterolesByKey1(this IAdoxiocorporateroles operations, System.Guid adoxioCorporateroleid, MicrosoftDynamicsCRMadoxioCorporaterole body)
            {
                operations.CorporaterolesByKey1Async(adoxioCorporateroleid, body).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Update entity in adoxio_corporateroles
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='adoxioCorporateroleid'>
            /// key: adoxio_corporateroleid of adoxio_corporaterole
            /// </param>
            /// <param name='body'>
            /// New property values
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task CorporaterolesByKey1Async(this IAdoxiocorporateroles operations, System.Guid adoxioCorporateroleid, MicrosoftDynamicsCRMadoxioCorporaterole body, CancellationToken cancellationToken = default(CancellationToken))
            {
                (await operations.CorporaterolesByKey1WithHttpMessagesAsync(adoxioCorporateroleid, body, null, cancellationToken).ConfigureAwait(false)).Dispose();
            }

            /// <summary>
            /// Update entity in adoxio_corporateroles
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='adoxioCorporateroleid'>
            /// key: adoxio_corporateroleid of adoxio_corporaterole
            /// </param>
            /// <param name='body'>
            /// New property values
            /// </param>
            /// <param name='customHeaders'>
            /// Headers that will be added to request.
            /// </param>
            public static HttpOperationResponse CorporaterolesByKey1WithHttpMessages(this IAdoxiocorporateroles operations, System.Guid adoxioCorporateroleid, MicrosoftDynamicsCRMadoxioCorporaterole body, Dictionary<string, List<string>> customHeaders = null)
            {
                return operations.CorporaterolesByKey1WithHttpMessagesAsync(adoxioCorporateroleid, body, customHeaders, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Delete entity from adoxio_corporateroles
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='adoxioCorporateroleid'>
            /// key: adoxio_corporateroleid of adoxio_corporaterole
            /// </param>
            /// <param name='ifMatch'>
            /// ETag
            /// </param>
            public static void CorporaterolesByKey2(this IAdoxiocorporateroles operations, System.Guid adoxioCorporateroleid, string ifMatch = default(string))
            {
                operations.CorporaterolesByKey2Async(adoxioCorporateroleid, ifMatch).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Delete entity from adoxio_corporateroles
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='adoxioCorporateroleid'>
            /// key: adoxio_corporateroleid of adoxio_corporaterole
            /// </param>
            /// <param name='ifMatch'>
            /// ETag
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task CorporaterolesByKey2Async(this IAdoxiocorporateroles operations, System.Guid adoxioCorporateroleid, string ifMatch = default(string), CancellationToken cancellationToken = default(CancellationToken))
            {
                (await operations.CorporaterolesByKey2WithHttpMessagesAsync(adoxioCorporateroleid, ifMatch, null, cancellationToken).ConfigureAwait(false)).Dispose();
            }

            /// <summary>
            /// Delete entity from adoxio_corporateroles
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='adoxioCorporateroleid'>
            /// key: adoxio_corporateroleid of adoxio_corporaterole
            /// </param>
            /// <param name='ifMatch'>
            /// ETag
            /// </param>
            /// <param name='customHeaders'>
            /// Headers that will be added to request.
            /// </param>
            public static HttpOperationResponse CorporaterolesByKey2WithHttpMessages(this IAdoxiocorporateroles operations, System.Guid adoxioCorporateroleid, string ifMatch = default(string), Dictionary<string, List<string>> customHeaders = null)
            {
                return operations.CorporaterolesByKey2WithHttpMessagesAsync(adoxioCorporateroleid, ifMatch, customHeaders, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
            }

    }
}
