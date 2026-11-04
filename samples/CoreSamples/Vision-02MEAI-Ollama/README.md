# Optional local vision with Ollama

Requires .NET 10 or later and a vision-capable Ollama model. The default `llama3.2-vision` is a much larger download than the core text models; check available RAM/VRAM before pulling it.

```powershell
ollama pull llama3.2-vision
dotnet run --project .\samples\CoreSamples\Vision-02MEAI-Ollama
```

Run from the repository root. The build copies sample JPEGs to the output `images` directory; the application resolves the image relative to that output directory, not the current terminal directory.

Expect a description of the running-shoes image. Text and JPEG bytes are sent in the same user message. This demonstrates image understanding, not a full PDF extraction or managed document-processing service.

For a smaller alternative, `moondream` (approximately 830 MB to download) was verified locally to identify the runners and colorful shoes in this photo:

```powershell
ollama pull moondream
$env:OLLAMA_VISION_MODEL = "moondream"
dotnet run --project .\samples\CoreSamples\Vision-02MEAI-Ollama
python .\scripts\test_ollama.py --run --vision-model moondream
```

Check that the answer actually identifies runners or footwear. A nonempty response alone does not establish that the model understood the image. During local validation, an installed `gemma4:e2b` advertised vision support but described this photo as a document, including when called directly through Ollama's HTTP API. The acceptance runner rejects that answer; `moondream` passed the same grounding check. This verifies the supplied photo, not every vision or OCR task. The default `llama3.2-vision` was not installed on the validation host.

`OLLAMA_ENDPOINT` overrides the server and `OLLAMA_VISION_MODEL` overrides the model. A text-only model such as `phi4-mini` is not a valid replacement. Requests have a three-minute timeout; slow CPU-only inference or insufficient memory may require a smaller vision-capable model.

[Vision lesson](../../../03-AIPatternsAndApplications/03-vision-document-understanding.md) | [Ollama path](../../../docs/ollama-learning-path.md)
