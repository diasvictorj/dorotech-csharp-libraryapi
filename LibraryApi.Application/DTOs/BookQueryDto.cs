namespace LibraryApi.Application.DTOs;

public class BookQueryDto : BaseQueryDto
{
    public string? Title { get; set; }
    public string? AuthorName { get; set; }
    public int? PublicationYear { get; set; }
}