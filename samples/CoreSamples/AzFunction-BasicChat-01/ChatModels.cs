namespace AzFunction_BasicChat_01;

public sealed record ChatRequest(string? Question);

public sealed record ChatResponse(string Question, string Answer, string Model);

public sealed record ErrorResponse(string Error);
