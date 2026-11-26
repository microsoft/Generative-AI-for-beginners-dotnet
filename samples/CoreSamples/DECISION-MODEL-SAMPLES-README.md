# Decision model samples

These samples use native typed-decision APIs to return choices, scores, assessments, and probabilities instead of generated reasoning text.

| Sample | Provider | Demonstrates |
|---|---|---|
| [13. Foundry Local](./Decision-13-FoundryLocal/README.md) | Foundry Local | Cached local decision model, choice, score, and assessment |
| [14. Llama App text](./Decision-14-LlamaText/README.md) | Llama App | Text-based choice, score, and assessment |
| [15. Llama App image](./Decision-15-LlamaImage/README.md) | Llama App | Image input with a typed visual choice and probability distribution |

The samples use the community-maintained [`ElBruno.AI.Decisions`](https://github.com/elbruno/ElBruno.AI.Decisions) provider packages. These are not official Microsoft.Extensions.AI APIs. Models must be installed locally before running the examples. The Foundry Local sample requires a model cached by the Foundry Local CLI, and the Llama App samples require a running local endpoint with the selected model loaded.

Model probabilities and confidence are estimates, not guarantees of correctness. Evaluate decisions against representative data and retain human review for uncertain or consequential outcomes.
