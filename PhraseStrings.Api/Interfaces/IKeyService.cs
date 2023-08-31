﻿using PhraseStrings.Api.Model;

namespace PhraseStrings.Api.Interfaces;

public interface IKeyService
{
    public Task<List<Key>?> GetAll(string projectId);
}