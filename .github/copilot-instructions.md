# Copilot Instructions

## Project Guidelines
- Follow the Oqtane playbook GOVERNANCE rules for this repository when implementing changes.
- Never modify files in the oqtane.framework folder; only change files within the GIBS.Module.Resource workspace repository.
- When adding migrations in this module, use incremented migration file naming like 01000100_<Name>.cs and increment module/package version accordingly (e.g., 1.0.1).
- When using Oqtane Pager in this repo, preserve paging state during row actions by binding CurrentPage and OnPageChange.
- When adjusting inline consent layouts in this repo, keep the label, checkbox, and consent text on the same row by avoiding flex-wrap and giving the consent text a flex-grow container.