﻿using PhraseStrings.Api.Interfaces;
using PhraseStrings.Api.Model;
using System.Text.Json;

namespace PhraseStrings.Api.Services;

internal class KeyService : BaseService, IKeyService
{
    internal KeyService(HttpClient httpClient, JsonSerializerOptions jsonSerializerOptions) : base(httpClient, jsonSerializerOptions)
    {
    }

    public async Task<List<Key>?> GetAll(string projectId)
    {
        var result = await GetListAsync<List<Key>>($"projects/{projectId}/keys");

        return result;
    }
}