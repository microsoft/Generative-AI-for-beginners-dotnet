using Microsoft.Extensions.VectorData;

namespace AgentFx_AIWebChatApp_Persisting.Web.Services;

public class IngestedDocument
{
    private const int VectorDimensions = 2;
    private const string VectorDistanceFunction = DistanceFunction.CosineDistance;

    [VectorStoreKey]
    public string Key { get; set; } = string.Empty;

    [VectorStoreData(IsIndexed = true)]
    public string SourceId { get; set; } = string.Empty;

    [VectorStoreData]
    public string DocumentId { get; set; } = string.Empty;

    [VectorStoreData]
    public string DocumentVersion { get; set; } = string.Empty;

    // The vector is not used but required for some vector databases
    [VectorStoreVector(VectorDimensions, DistanceFunction = VectorDistanceFunction)]
    public ReadOnlyMemory<float> Vector { get; set; } = new ReadOnlyMemory<float>([0, 0]);
}
