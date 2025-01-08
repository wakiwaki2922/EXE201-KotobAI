using CursusJapaneseLearningPlatform.Repository.Entities;
using CursusJapaneseLearningPlatform.Service.BusinessModels.PayPalModels;
using CursusJapaneseLearningPlatform.Service.Commons.Exceptions;
using CursusJapaneseLearningPlatform.Service.Commons.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PayPal.Api;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Service.Commons.Implementations
{


    public class PayPalClient : IPayPalClient
    {
        private readonly HttpClient _httpClient;
        private string _accessToken;
        private DateTime _tokenExpiryTime;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _mode;
        private readonly ILogger<PayPalClient> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly int _tokenExpiryBuffer;

        private const string PaymentMethodPayPal = "paypal";
        private const string GrantTypeClientCredentials = "client_credentials";

        public PayPalClient(IConfiguration configuration, HttpClient httpClient, ILogger<PayPalClient> logger, IHttpContextAccessor httpContextAccessor)
        {
            _clientId = configuration["PayPal:ClientId"]
                ?? throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "PayPal Client ID is not configured.");
            _clientSecret = configuration["PayPal:ClientSecret"]
                ?? throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "PayPal Client Secret is not configured.");
            _mode = configuration["PayPal:Mode"]
                ?? throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "PayPal Mode is not configured.");

            _tokenExpiryBuffer = int.TryParse(configuration["PayPal:TokenExpiryBuffer"], out var buffer) ? buffer : 60; // Default to 60 seconds

            _httpClient = httpClient;
            _logger = logger;

            _httpClient.BaseAddress = new Uri(_mode.ToLower() == "sandbox"
                ? "https://api.sandbox.paypal.com"
                : "https://api.paypal.com");

            // Set default headers that are common for all requests
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PayPalPayment> CreatePayment(decimal amount, string currency, string intent,
            string description, string returnUrl, string cancelUrl, CancellationToken cancellationToken = default)
        {
            try
            {
                await EnsureAccessTokenAsync(cancellationToken);

                var payment = new
                {
                    intent = intent.ToLower(),
                    payer = new { payment_method = PaymentMethodPayPal },
                    transactions = new[]
                    {
                    new
                    {
                        amount = new
                        {
                            total = amount.ToString("F2", CultureInfo.InvariantCulture),
                            currency = currency.ToUpper()
                        },
                        description = description
                    }
                },
                    redirect_urls = new
                    {
                        return_url = returnUrl,
                        cancel_url = cancelUrl
                    }
                };

                using var request = new HttpRequestMessage(HttpMethod.Post, "/v1/payments/payment")
                {
                    Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _accessToken) },
                    Content = JsonContent.Create(payment)
                };

                var response = await _httpClient.SendAsync(request, cancellationToken);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("PayPal payment creation failed. Status: {StatusCode}, Response: {Response}, Request: {RequestMethod} {RequestUrl}",
                        response.StatusCode, content, request.Method, request.RequestUri);
                    throw new CustomException(StatusCodes.Status500InternalServerError,
                        ResponseCodeConstants.PAYMENT_FAILED,
                        $"Failed to create PayPal payment: {content}");
                }

                return JsonSerializer.Deserialize<PayPalPayment>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? throw new CustomException(StatusCodes.Status500InternalServerError,
                    ResponseCodeConstants.PAYMENT_FAILED,
                    "Failed to deserialize PayPal payment response");
            }
            catch (Exception ex) when (ex is not CustomException)
            {
                _logger.LogError(ex, "Unexpected error during PayPal payment creation");
                throw new CustomException(StatusCodes.Status500InternalServerError,
                    ResponseCodeConstants.PAYMENT_FAILED,
                    "An unexpected error occurred while creating the PayPal payment");
            }
        }

        public async Task<bool> ExecutePayment(string paymentId, string payerId, string token, CancellationToken cancellationToken = default)
        {
            try
            {
                await EnsureAccessTokenAsync(cancellationToken);

                var requestBody = new
                {
                    payer_id = payerId
                };

                using var request = new HttpRequestMessage(HttpMethod.Post, $"/v1/payments/payment/{paymentId}/execute")
                {
                    Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _accessToken) },
                    Content = JsonContent.Create(requestBody)
                };

                var response = await _httpClient.SendAsync(request, cancellationToken);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("PayPal payment execution failed. Status: {StatusCode}, Response: {Response}, Request: {RequestMethod} {RequestUrl}",
                        response.StatusCode, content, request.Method, request.RequestUri);
                    throw new CustomException(StatusCodes.Status500InternalServerError,
                        ResponseCodeConstants.PAYMENT_FAILED,
                        $"Failed to execute PayPal payment: {content}");
                }

                return true;
            }
            catch (Exception ex) when (ex is not CustomException)
            {
                _logger.LogError(ex, "Unexpected error during PayPal payment execution");
                throw new CustomException(StatusCodes.Status500InternalServerError,
                    ResponseCodeConstants.PAYMENT_FAILED,
                    "An unexpected error occurred while executing the PayPal payment");
            }
        }

        private async Task EnsureAccessTokenAsync(CancellationToken cancellationToken)
        {
            if (_accessToken == null || DateTime.UtcNow >= _tokenExpiryTime)
            {
                var requestBody = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("grant_type", GrantTypeClientCredentials)
            });

                var byteArray = Encoding.ASCII.GetBytes($"{_clientId}:{_clientSecret}");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var response = await _httpClient.PostAsync("/v1/oauth2/token", requestBody, cancellationToken);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to obtain PayPal access token. Status: {StatusCode}, Response: {Response}", response.StatusCode, content);
                    throw new CustomException(StatusCodes.Status500InternalServerError,
                        $"Failed to obtain PayPal access token: {content}");
                }

                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? throw new CustomException(StatusCodes.Status500InternalServerError,
                    "Failed to deserialize PayPal token response");

                _accessToken = tokenResponse.AccessToken;
                _tokenExpiryTime = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - _tokenExpiryBuffer);
            }
        }

        private class TokenResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }

            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }
        }
    }
}

