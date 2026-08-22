#!/usr/bin/env bash
set -euo pipefail

if [[ -z "${AZURE_SUBSCRIPTION_ID:-}" ||
      -z "${FOUNDRY_RESOURCE_GROUP_NAME:-}" ||
      -z "${FOUNDRY_ACCOUNT_NAME:-}" ]]; then
  echo 'Foundry cleanup settings are incomplete; no Foundry role assignment could have been provisioned.'
  exit 0
fi

resource_group="${AZURE_RESOURCE_GROUP:-}"
if [[ -z "$resource_group" && -n "${AZURE_ENV_NAME:-}" ]]; then
  resource_group="rg-$AZURE_ENV_NAME"
fi

if [[ -z "$resource_group" ]]; then
  echo 'No application resource group is available for Foundry role cleanup.'
  exit 0
fi

resource_group_exists="$(
  az group exists \
    --subscription "$AZURE_SUBSCRIPTION_ID" \
    --name "$resource_group"
)"

if [[ "$resource_group_exists" != 'true' ]]; then
  echo 'The application resource group does not exist; no Foundry role assignment needs removal.'
  exit 0
fi

managed_identity_name="${MANAGED_IDENTITY_NAME:-}"
if [[ -z "$managed_identity_name" ]]; then
  managed_identity_name="$(
    az identity list \
      --subscription "$AZURE_SUBSCRIPTION_ID" \
      --resource-group "$resource_group" \
      --query '[0].name' \
      --output tsv
  )"
fi

if [[ -z "$managed_identity_name" ]]; then
  echo 'No managed identity exists; no Foundry role assignment needs removal.'
  exit 0
fi

principal_id="$(
  az identity show \
    --subscription "$AZURE_SUBSCRIPTION_ID" \
    --resource-group "$resource_group" \
    --name "$managed_identity_name" \
    --query principalId \
    --output tsv
)"

if [[ -z "$principal_id" ]]; then
  echo 'The managed identity has no principal; no Foundry role assignment needs removal.'
  exit 0
fi

foundry_scope="/subscriptions/$AZURE_SUBSCRIPTION_ID/resourceGroups/$FOUNDRY_RESOURCE_GROUP_NAME/providers/Microsoft.CognitiveServices/accounts/$FOUNDRY_ACCOUNT_NAME"
role_name='Cognitive Services OpenAI User'

assignment_ids=()
assignment_output="$(
  az role assignment list \
    --subscription "$AZURE_SUBSCRIPTION_ID" \
    --assignee-object-id "$principal_id" \
    --scope "$foundry_scope" \
    --role "$role_name" \
    --query '[].id' \
    --output tsv
)"

while IFS= read -r assignment_id; do
  if [[ -n "$assignment_id" ]]; then
    assignment_ids+=("$assignment_id")
  fi
done <<< "$assignment_output"

if (( ${#assignment_ids[@]} == 0 )); then
  echo 'No managed identity Foundry role assignment needs removal.'
  exit 0
fi

az role assignment delete \
  --subscription "$AZURE_SUBSCRIPTION_ID" \
  --ids "${assignment_ids[@]}"

echo 'Removed the managed identity Foundry role assignment.'
