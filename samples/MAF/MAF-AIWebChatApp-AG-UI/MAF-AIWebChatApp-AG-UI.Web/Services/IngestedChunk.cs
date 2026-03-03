using Microsoft.Extensions.VectorData;

namespace MAF_AIWebChatApp_AG_UI.Web.Services;

public class IngestedChunk
{
    private const int VectorDimensions = 1536; // 1536 is the default vector size for the OpenAI text-embedding-3-small model
    private const string VectorDistanceFunction = DistanceFunction.CosineDistance;

    [VectorStoreKey]
    public string Key { get; set; } = string.Empty;

    [VectorStoreData(IsIndexed = true)]
    public string DocumentId { get; set; } = string.Empty;

    [VectorStoreData]
    public int PageNumber { get; set; }

    [VectorStoreData]
    public string Text { get; set; } = string.Empty;

    [VectorStoreVector(VectorDimensions, DistanceFunction = VectorDistanceFunction)]
    public string? Vector => Text;
}
