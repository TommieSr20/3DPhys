using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using UnityEngine;
using Unity.Services.Economy.Internal;
using Unity.Services.Economy.Internal.Apis.Currencies;
using Unity.Services.Economy.Internal.Currencies;
using Unity.Services.Economy.Internal.Http;
using Unity.Services.Economy.Internal.Models;
using Unity.Services.Economy.Model;
using Unity.Services.Core.Configuration.Internal;

[assembly: InternalsVisibleTo("Unity.Services.Economy.Tests")]

namespace Unity.Services.Economy
{
    /// <summary>
    /// The PlayerBalances methods provide access to the current player's balances, and allow you to update them.
    /// </summary>
    public interface IEconomyPlayerBalancesApiClient
    {
        /// <summary>
        /// Fires when the SDK updates a player's balance. The called action will be passed the currency ID that was updated.
        ///
        /// Note that this will NOT fire for balance changes from elsewhere not in this instance of the SDK, for example other
        /// server-side updates or updates from other devices.
        /// </summary>
        event Action<string> BalanceUpdated;

        /// <summary>
        /// Gets the current balances for the currently signed in player.
        /// The balances are available on the returned object using the <c>Balances</c> property.
        /// The results are paginated - the first set of results are initially returned, and more can be requested with the <c>GetNextAsync</c> method.
        /// The <c>HasNext</c> property indicates whether there are more results to be returned.
        /// Throws a EconomyException with a reason code and explanation if the request is badly formed, unauthorized or uses a missing resource.
        /// </summary>
        /// <param name="options">(Optional) Use to set the number of items to fetch per call.</param>
        /// <returns>A GetBalancesResult object, with properties as specified above.</returns>
        /// <exception cref="EconomyException">Thrown if request is unsuccessful</exception>
        /// <exception cref="EconomyRateLimitedException">Thrown if the service returned rate limited error.</exception>
        Task<GetBalancesResult> GetBalancesAsync(GetBalancesOptions options = null);

        /// <summary>
        /// Increments the balance of the specified currency for the currently logged in user.
        ///
        /// This method optionally takes a writeLock string. If provided, then an exception will be thrown unless the writeLock matches the writeLock received by a previous read, in order to provide optomistic concurrency.
        /// If not provided, the transaction will proceed regardless of any existing writeLock in the data.
        /// Throws a EconomyException with a reason code and explanation if the request is badly formed, unauthorized or uses a missing resource.
        /// </summary>
        /// <param name="currencyId">The currency ID to update</param>
        /// <param name="amount">The amount to increment by</param>
        /// <param name="options">(Optional) Use to set a write lock for optimistic concurrency</param>
        /// <returns>The updated player balance for the relevant currency.</returns>
        /// <exception cref="EconomyException">Thrown if request is unsuccessful</exception>
        /// <exception cref="EconomyValidationException">Thrown if the service returned validation error.</exception>
        /// <exception cref="EconomyRateLimitedException">Thrown if the service returned rate limited error.</exception>
        Task<PlayerBalance> IncrementBalanceAsync(string currencyId, int amount, IncrementBalanceOptions options = null);

        /// <summary>
        /// Decrements the balance of the specified currency for the currently logged in user.
        ///
        /// This method optionally takes a writeLock string. If provided, then an exception will be thrown unless the writeLock matches the writeLock received by a previous read, in order to provide optimistic concurrency.
        /// If not provided, the transaction will proceed regardless of any existing writeLock in the data.
        /// Throws a EconomyException with a reason code and explanation if the request is badly formed, unauthorized or uses a missing resource.
        /// </summary>
        /// <param name="currencyId">The currency ID to update</param>
        /// <param name="amount">The amount to decrement by</param>
        /// <param name="options">(Optional) Use to set a write lock for optimistic concurrency</param>
        /// <returns>The updated player balance for the relevant currency.</returns>
        /// <exception cref="EconomyException">Thrown if request is unsuccessful</exception>
        /// <exception cref="EconomyValidationException">Thrown if the service returned validation error.</exception>
        /// <exception cref="EconomyRateLimitedException">Thrown if the service returned rate limited error.</exception>
        Task<PlayerBalance> DecrementBalanceAsync(string currencyId, int amount, DecrementBalanceOptions options = null);

        /// <summary>
        /// Sets the balance of the specified currency for the currently logged in user.
        /// Will throw an exception if the currency doesn't exist, or if the set amount will take the balance above/below the maximum/minimum allowed for that currency.
        ///
        /// This method optionally takes a writeLock string. If provided, then an exception will be thrown unless the writeLock matches the writeLock received by a previous read, in order to provide optimistic concurrency.
        /// If not provided, the transaction will proceed regardless of any existing writeLock in the data.
        /// Throws a EconomyException with a reason code and explanation if the request is badly formed, unauthorized or uses a missing resource.
        /// </summary>
        /// <param name="currencyId">The currency ID to update</param>
        /// <param name="balance">The amount to set the balance to</param>
        /// <param name="options">(Optional) Used to set a write lock for optimistic concurrency</param>
        /// <returns>The updated player balance for the relevant currency.</returns>
        /// <exception cref="EconomyException">Thrown if request is unsuccessful</exception>
        /// <exception cref="EconomyValidationException">Thrown if the service returned validation error.</exception>
        /// <exception cref="EconomyRateLimitedException">Thrown if the service returned rate limited error.</exception>
        Task<PlayerBalance> SetBalanceAsync(string currencyId, long balance, SetBalanceOptions options = null);
    }

    internal class PlayerBalancesInternal : IEconomyPlayerBalancesApiClient
    {
        readonly ICloudProjectId m_CloudProjectId;
        readonly ICurrenciesApiClient m_CurrenciesApiClient;
        readonly IEconomyAuthentication m_EconomyAuthentication;

        internal PlayerBalancesInternal(ICloudProjectId cloudProjectId, ICurrenciesApiClient currenciesApiClient, IEconomyAuthentication economyAuthWrapper)
        {
            m_CloudProjectId = cloudProjectId;
            m_CurrenciesApiClient = currenciesApiClient;
            m_EconomyAuthentication = economyAuthWrapper;
        }

        public event Action<string> BalanceUpdated;

        public async Task<GetBalancesResult> GetBalancesAsync(GetBalancesOptions options = null)
        {
            if (options == null)
            {
                options = new GetBalancesOptions();
            }

            m_EconomyAuthentication.CheckSignedIn();

            EconomyAPIErrorHandler.HandleItemsPerFetchExceptions(options.ItemsPerFetch);

            GetPlayerCurrenciesRequest request = new GetPlayerCurrenciesRequest(
                m_CloudProjectId.GetCloudProjectId(),
                m_EconomyAuthentication.GetPlayerId(),
                configAssignmentHash: m_EconomyAuthentication.configAssignmentHash,
                unityInstallationId: m_EconomyAuthentication.GetUnityInstallationId(),
                analyticsUserId: m_EconomyAuthentication.GetAnalyticsUserId(),
                limit: options.ItemsPerFetch
            );

            try
            {
                Response<PlayerCurrencyBalanceResponse> response = await m_CurrenciesApiClient.GetPlayerCurrenciesAsync(request);
                List<PlayerBalance> playerBalances = ConvertToPlayerBalances(response.Result.Results);

                GetBalancesResult result = new GetBalancesResult(playerBalances, !string.IsNullOrEmpty(response.Result.Links?.Next), this);

                return result;
            }
            catch (HttpException<BasicErrorResponse> e)
            {
                throw EconomyAPIErrorHandler.HandleException(e);
            }
            catch (HttpException e)
            {
                throw EconomyAPIErrorHandler.HandleException(e);
            }
        }

        public async Task<PlayerBalance> IncrementBalanceAsync(string currencyId, int amount, IncrementBalanceOptions options = null)
        {
            m_EconomyAuthentication.CheckSignedIn();

            IncrementPlayerCurrencyBalanceRequest request = new IncrementPlayerCurrencyBalanceRequest(
                m_CloudProjectId.GetCloudProjectId(),
                m_EconomyAuthentication.GetPlayerId(),
                currencyId,
                new CurrencyModifyBalanceRequest(amount, currencyId, options?.WriteLock),
                m_EconomyAuthentication.configAssignmentHash,
                m_EconomyAuthentication.GetUnityInstallationId(),
                m_EconomyAuthentication.GetAnalyticsUserId()
            );

            try
            {
                Response<CurrencyBalanceResponse> response = await m_CurrenciesApiClient.IncrementPlayerCurrencyBalanceAsync(request);

                FireBalanceUpdatedEvent(currencyId);

                return ConvertToPlayerBalance(response.Result);
            }
            catch (HttpException<BasicErrorResponse> e)
            {
                throw EconomyAPIErrorHandler.HandleException(e);
            }
            catch (HttpException<ValidationErrorResponse> e)
            {
                throw EconomyAPIErrorHandler.HandleException(e);
            }
            catch (HttpException e)
            {
                throw EconomyAPIErrorHandler.HandleException(e);
            }
        }

        public async Task<PlayerBalance> DecrementBalanceAsync(string currencyId, int amount, DecrementBalanceOptions options = null)
        {
            m_EconomyAuthentication.CheckSignedIn();

            DecrementPlayerCurrencyBalanceRequest request = new DecrementPlayerCurrencyBalanceRequest(
                m_CloudProjectId.GetCloudProjectId(),
                m_EconomyAuthentication.GetPlayerId(),
                currencyId,
                new CurrencyModifyBalanceRequest(amount, currencyId, options?.WriteLock),
                m_EconomyAuthentication.configAssignmentHash,
                m_EconomyAuthentication.GetUnityInstallationId(),
                m_EconomyAuthentication.GetAnalyticsUserId()
            );

            try
            {
                Response<CurrencyBalanceResponse> response = await m_CurrenciesApiClient.DecrementPlayerCurrencyBalanceAsync(request);

                FireBalanceUpdatedEvent(currencyId);

                return ConvertToPlayerBalance(response.Result);
            }
            catch (HttpException<BasicErrorResponse> e)
            {
                throw EconomyAPIErrorHandler.HandleException(e);
            }
            catch (HttpException<ValidationErrorResponse> e)
            {
                throw EconomyAPIErrorHandler.HandleException(e);
            }
            catch (HttpException e)
            {
                throw EconomyAPIErrorHandler.HandleException(e);
            }
        }

        public async Task<PlayerBalance> SetBalanceAsync(string currencyId, long balance, SetBalanceOptions options = null)
        {
            m_EconomyAuthentication.CheckSignedIn();

            SetPlayerCurrencyBalanceRequest request = new SetPlayerCurrencyBalanceRequest(
                m_CloudProjectId.GetCloudProjectId(),
                m_EconomyAuthentication.GetPlayerId(),
                currencyId,
                new CurrencyBalanceRequest(balance, currencyId, options?.WriteLock),
                m_EconomyAuthentication.configAssignmentHash,
                m_EconomyAuthentication.GetUnityInstallationId(),
                m_EconomyAuthentication.GetAnalyticsUserId());

            try
            {
                Response<CurrencyBalanceResponse> response = await m_CurrenciesApiClient.SetPlayerCurrencyBalanceAsync(request);

                EconomyDate created = response.Result.Created.Date == null ? null : new EconomyDate { Date = (DateTime)response.Result.Created.Date };
                EconomyDate modified = response.Result.Modified.Date == null ? null : new EconomyDate { Date = (DateTime)response.Result.Modified.Date };

                PlayerBalance convertedResponse = new PlayerBalance(response.Result.CurrencyId, response.Result.Balance,
                    response.Result.WriteLock, created, modified);

                FireBalanceUpdatedEvent(currencyId);

                return convertedResponse;
            }
            catch (HttpException<BasicErrorResponse> e)
            {
                throw EconomyAPIErrorHandler.HandleException(e);
            }
            catch (HttpException<ValidationErrorResponse> e)
            {
                throw EconomyAPIErrorHandler.HandleException(e);
            }
            catch (HttpException e)
            {
                throw EconomyAPIErrorHandler.HandleException(e);
            }
        }

        internal async Task<GetBalancesResult> GetNextBalancesAsync(string afterCurrencyId, int itemsPerFetch = 20)
        {
            m_EconomyAuthentication.CheckSignedIn();

            EconomyAPIErrorHandler.HandleItemsPerFetchExceptions(itemsPerFetch);

            GetPlayerCurrenciesRequest request = new GetPlayerCurrenciesRequest(
                m_CloudProjectId.GetCloudProjectId(),
                m_EconomyAuthentication.GetPlayerId(),
                m_EconomyAuthentication.configAssignmentHash,
                m_EconomyAuthentication.GetUnityInstallationId(),
                m_EconomyAuthentication.GetAnalyticsUserId(),
                afterCurrencyId,
                itemsPerFetch);

            try
            {
                Response<PlayerCurrencyBalanceResponse> response = await m_CurrenciesApiClient.GetPlayerCurrenciesAsync(request);
                List<PlayerBalance> playerBalances = ConvertToPlayerBalances(response.Result.Results);

                GetBalancesResult result = new GetBalancesResult(playerBalances, !string.IsNullOrEmpty(response.Result.Links?.Next), this);

                return result;
            }
            catch (HttpException<BasicErrorResponse> e)
            {
                throw EconomyAPIErrorHandler.HandleException(e);
            }
            catch (HttpException e)
            {
                throw EconomyAPIErrorHandler.HandleException(e);
            }
        }

        internal static List<PlayerBalance> ConvertToPlayerBalances(List<CurrencyBalanceResponse> currencyBalanceResponses)
        {
            List<PlayerBalance> convertedBalances = new List<PlayerBalance>();

            foreach (var balanceResponse in currencyBalanceResponses)
            {
                convertedBalances.Add(ConvertToPlayerBalance(balanceResponse));
            }

            return convertedBalances;
        }

        internal void FireBalanceUpdatedEvent(string currencyID)
        {
            BalanceUpdated?.Invoke(currencyID);
        }

        static PlayerBalance ConvertToPlayerBalance(CurrencyBalanceResponse currencyBalanceResponse)
        {
            PlayerBalance convertedBalance = new PlayerBalance();

            convertedBalance.CurrencyId = currencyBalanceResponse.CurrencyId;
            convertedBalance.Balance = currencyBalanceResponse.Balance;
            convertedBalance.WriteLock = currencyBalanceResponse.WriteLock;
            convertedBalance.Created = currencyBalanceResponse.Created.Date == null ? null : new EconomyDate { Date = currencyBalanceResponse.Created.Date.Value };
            convertedBalance.Modified = currencyBalanceResponse.Modified.Date == null ? null : new EconomyDate { Date = currencyBalanceResponse.Modified.Date.Value };

            return convertedBalance;
        }
    }

    /// <summary>
    /// Options for a GetBalancesAsync call.
    /// </summary>
    public class GetBalancesOptions
    {
        /// <summary>
        /// The number of items to fetch per call
        /// </summary>
        public int ItemsPerFetch = 20;
    }

    /// <summary>
    /// Options for a IncrementBalanceAsync call.
    /// </summary>
    public class IncrementBalanceOptions
    {
        /// <summary>
        /// A write lock for optimistic concurrency.
        /// </summary>
        public string WriteLock = null;
    }

    /// <summary>
    /// Options for a DecrementBalanceAsync call.
    /// </summary>
    public class DecrementBalanceOptions
    {
        /// <summary>
        /// A write lock for optimistic concurrency.
        /// </summary>
        public string WriteLock = null;
    }

    /// <summary>
    /// Options for a SetBalancesAsync call.
    /// </summary>
    public class SetBalanceOptions
    {
        /// <summary>
        /// A write lock for optimistic concurrency
        /// </summary>
        public string WriteLock = null;
    }
}
