using LearnHub.Application.Features.Search.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Search.Queries.SearchAutoComplete;

public sealed record SearchAutoCompleteQuery(
    string Query,
    int MaxResults = 5) : IRequest<Result<SearchAutoCompleteDto>>;
