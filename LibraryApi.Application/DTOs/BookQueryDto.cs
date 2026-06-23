namespace LibraryApi.Application.DTOs;

public class BookQueryDto
{
    public string? Title { get; set; }
    public string? AuthorName { get; set; }
    public int? PublicationYear { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}