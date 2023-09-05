﻿using PhraseStrings.Api.Model;

namespace PhraseStrings.Api.Interfaces;

public interface ITranslationService
{
    public Task<List<Translation>?> GetAll(string projectId, int perPage = 100, int? page = null);

    public Task<List<Translation>?> GetByLocale(string projectId, string localeId);
}
