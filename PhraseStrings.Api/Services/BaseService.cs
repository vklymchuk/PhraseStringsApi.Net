﻿using PhraseStrings.Api.Utilities;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace PhraseStrings.Api.Services;

internal abstract class BaseService
{
    protected HttpClient HttpClient;
    protected JsonSerializerOptions JsonSerializerOptions;

    internal BaseService(HttpClient httpClient)
    {
        HttpClient = httpClient;
        JsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumMemberConverter() }
        };
    }

    protected async Task<TResult?> Get<TResult>(string requestUri)
    {
        var result = await HttpClient.GetAsync(requestUri);

        if (result.StatusCode == System.Net.HttpStatusCode.NotFound)
            return default;

        HandleError(result);

        return await HandleResult<TResult>(result);
    }

    protected async Task<TResult?> GetList<TResult>(string requestUri)
    {
        var result = await HttpClient.GetAsync(requestUri);

        HandleError(result);

        return await HandleResult<TResult>(result);
    }

    protected async Task<TResult?> Post<TRequest, TResult>(string requestUri, TRequest request, bool contentAsFormData = false)
    {
        HttpResponseMessage result;

        if (contentAsFormData)
        {
            var data = request.ToPhraseDatasKeyValue();
            result = await HttpClient.PostAsync(requestUri, new FormUrlEncodedContent(data));
        }
        else
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = new StringContent(JsonSerializer.Serialize(request, JsonSerializerOptions), Encoding.UTF8, "application/json")
            };
            result = await HttpClient.SendAsync(requestMessage);
        }

        HandleError(result);

        return await HandleResult<TResult>(result);
    }

    protected async Task<TResult?> Patch<TRequest, TResult>(string requestUri, TRequest request)
    {
        var data = request.ToPhraseDatasKeyValue();
        var result = await HttpClient.PatchAsync(requestUri, new FormUrlEncodedContent(data));

        HandleError(result);

        return await HandleResult<TResult>(result);
    }

    protected bool MatchesWildcard(string data, string wildcard)
    {
        var pattern = $"^{wildcard.Replace("*", ".*?")}$";
        return Regex.IsMatch(data, pattern, RegexOptions.IgnoreCase);
    }

    private async Task<TResult?> HandleResult<TResult>(HttpResponseMessage result)
    {
        var json = await result.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TResult?>(json);
    }

    private void HandleError(HttpResponseMessage responseMessage)
    {
        if (responseMessage.IsSuccessStatusCode)
            return;

        switch (responseMessage.StatusCode)
        {
            case System.Net.HttpStatusCode.Unauthorized:
                throw new Exception("Unauthorized, please check your Api Token");

            case System.Net.HttpStatusCode.NotFound:
                throw new Exception("NotFound, Please check your parameters (ProjectId)");

            case System.Net.HttpStatusCode.UnprocessableEntity:
                throw new Exception("UnprocessableEntity, Please check your content object, the item may already be used");

            default:
                throw new Exception($"An error occured: {responseMessage.StatusCode}");
        }
    }
}
