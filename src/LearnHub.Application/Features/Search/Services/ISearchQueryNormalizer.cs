namespace LearnHub.Application.Features.Search.Services;

public interface ISearchQueryNormalizer
{
    string Normalize(string query);
    IReadOnlyList<string> Tokenize(string query);
    IReadOnlyList<string> ExpandSynonyms(string term);
}
