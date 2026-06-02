# Copilot Instructions

## Project Guidelines
- When user requests efficiency improvements or optimization of a process, comprehensively check all related files (those that reference or are referenced by the process), review relevant files, and identify additional optimization opportunities beyond the primary file. Modify all relevant documents to maximize efficiency gains.

## Dark Mode Implementation
- InventoryTracker implements dark mode with CSS at InventoryTracker/wwwroot/css/darkmode.css and JavaScript at InventoryTracker/wwwroot/js/darkmode.js.
- Ensure changes do not break dark mode functionality when adding, modifying, or removing code.
- Do not modify styles that have dark-mode overrides.
- Preserve dark-mode class checks and selectors in CSS.
- Keep darkmode.js toggle functionality intact; preserve its API and behavior.
- Test new or changed styles in both light and dark modes.
- Add dark-mode CSS rules for any new UI elements that will not automatically inherit correct styling.
- When refactoring UI or theming, update darkmode.css and darkmode.js references as needed and verify the toggle and styles still work.
- Include automated or manual visual checks for the dark mode toggle and key screens when feasible.

## Repository Memory Requests
- When user requests to add repository memories to user memories for use on different computers, extract all project-specific guidelines and preferences from repository instructions (including UI/theming and dark mode implementation details such as file paths and toggle behavior) and consolidate them into a single memory entry for cross-machine consistency.