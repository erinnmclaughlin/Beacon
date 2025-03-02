namespace Beacon.Common;

public abstract class PagedList
{
    public int CurrentPage { get; }
    public int TotalPages { get; }
    public int PageSize { get; }
    public int TotalCount { get; }

    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    protected PagedList(int count, int currentPage, int pageSize)
    {
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = currentPage;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }
}

public class PagedList<T> : PagedList
{
    public T[] Items { get; }

    public int Count => Items.Length;
    
    public PagedList(T[] items, int totalCount, int currentPage, int pageSize) : base(totalCount, currentPage, pageSize)
    {
        Items = items;
    }

    public static implicit operator T[](PagedList<T> list) => list.Items;
}
