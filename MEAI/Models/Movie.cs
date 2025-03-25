namespace MEAI.Models
{
    using Microsoft.Extensions.VectorData;

    public class Movie<T>
    {
        [VectorStoreRecordKey]
        public T Key { get; set; }

        [VectorStoreRecordData]
        public string? Title { get; set; }

        [VectorStoreRecordData]
        public int? Year { get; set; }

        [VectorStoreRecordData]
        public string? Category { get; set; }

        [VectorStoreRecordData]
        public string? Description { get; set; }

    }

    public class MovieVector<T> : Movie<T>
    {
        [VectorStoreRecordVector(384, DistanceFunction.CosineSimilarity)]
        public ReadOnlyMemory<float> Vector { get; set; }
    }

    public class MovieSQLite<T> : Movie<T>
    {
        [VectorStoreRecordVector(Dimensions: 4, DistanceFunction.CosineDistance)]
        public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }

    }
}
