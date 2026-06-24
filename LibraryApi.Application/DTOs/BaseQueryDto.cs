namespace LibraryApi.Application.DTOs;

public abstract class BaseQueryDto
{
    public string? OrderBy { get; set; }
    public string? OrderDirection { get; set; } = "asc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}