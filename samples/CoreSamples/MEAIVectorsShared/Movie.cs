using Microsoft.Extensions.VectorData;
public class Movie<T>
{
    [VectorStoreKey]
    public T Key { get; set; }

    [VectorStoreData]
    public string? Title { get; set; }

    [VectorStoreData]
    public int? Year { get; set; }

    [VectorStoreData]
    public string? Category { get; set; }

    [VectorStoreData]
    public string? Description { get; set; }

}

public class MovieVector<T> : Movie<T>
{
    [VectorStoreVector(384)]
    public ReadOnlyMemory<float> Vector { get; set; }
}

public class MovieSQLite<T> : Movie<T>
{
    [VectorStoreVector(Dimensions: 4)]
    public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }

}