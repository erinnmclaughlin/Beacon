namespace Beacon.Common;

public class PagedList<T>
{
    public T[] Items { get; set; } = Array.Empty<T>();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    public int Count => Items.Length;
    public T this[int index] => Items[index];

    public PagedList()
    {
        
    }

    public PagedList(T[] items, int count, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }

    public static implicit operator T[](PagedList<T> list) => list.Items;
}
