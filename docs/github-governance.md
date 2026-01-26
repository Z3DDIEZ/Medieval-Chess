# GitHub Governance & Branch Rules

> [!IMPORTANT]
> The following rules MUST be configured manually in the GitHub Repository Settings.

## 1. Main Branch Protection

**Target Branch:** `main`

### Required Settings:
1.  **Require a pull request before merging**
    *   [x] Require approvals (Min: 1)
    *   [x] Dismiss stale pull request approvals when new commits are pushed
2.  **Require status checks to pass before merging**
    *   [x] Require branches to be up to date before merging
    *   **Required Checks:**
        *   `build-dotnet`
        *   `build-react`
        *   `Analyze (csharp)`
        *   `Analyze (javascript-typescript)`
3.  **Do not allow bypassing the above settings**
    *   [x] Apply to administrators (optional but recommended)

## 2. Merge Strategy
*   **Squash and Merge**: Recommended to keep the main history clean.

## 3. Labeling Schema
*   `type: feature`: New game mechanics or UI components.
*   `type: bug`: Incorrect logic or implementation errors.
*   `type: documentation`: Updates to `docs/` or `README.md`.
*   `security`: Vulnerability fixes or anti-cheat updates.
