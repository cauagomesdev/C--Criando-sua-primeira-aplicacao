---
name: semantic-commits
description: >-
  Generates Conventional Commits for a feature or change set. Creates a branch
  from master, stages only files relevant to the feature, and commits using the
  project's semantic commit pattern. Use when the user asks to commit changes,
  create commits, generate commit messages, or organize work into commits.
disable-model-invocation: true
---

# Semantic Commits

Generates focused, traceable commits following Conventional Commits. Never commits directly to `master`.

## Step 1 — Verify current branch

```bash
git branch --show-current
git status --short
```

If already on `master`, go to Step 2. Otherwise skip to Step 3.

## Step 2 — Create branch from master

Branch naming: `<type>/<short-scope>` — all lowercase, hyphens.

```bash
git checkout master
git pull origin master
git checkout -b <type>/<short-scope>
```

**Examples:** `feat/event-crud`, `fix/registration-conflict`, `chore/devcontainer`

Never commit directly to `master`. If HEAD is already `master`, always create a branch first.

## Step 3 — Identify files for the commit

Run `git status --short` and `git diff --name-only HEAD` to list changed files.

Group files by logical unit — **only stage files that belong to the commit being created**. A single feature may yield multiple focused commits (e.g. entity + repository in one commit, service in the next).

Never mix unrelated files in the same commit.

## Step 4 — Stage only the relevant files

```bash
git add <file1> <file2> ...
```

Verify the staging area before committing:

```bash
git diff --cached --name-only
```

## Step 5 — Write the commit message

### Format

```
<type>(<scope>): <imperative summary in English, max 72 chars>
```

No period at the end of the summary.

### Types

| Type | When to use |
|------|-------------|
| `feat` | New feature or endpoint |
| `fix` | Bug fix |
| `test` | Adding or updating tests |
| `refactor` | Code change without behavior change |
| `chore` | Tooling, config, dependencies, AI rules |
| `docs` | Documentation only |
| `perf` | Performance improvement |

### Scope — use the domain or layer being changed

| Examples |
|----------|
| `event`, `participant`, `registration` |
| `exception`, `config`, `dto` |
| `ai`, `devcontainer` |

### Examples from this project

```
feat(event): add Event entity, repository and request/response DTOs

feat(event): implement EventService with CRUD operations

feat(event): implement EventController REST endpoints

test(event): add unit tests for EventService and EventController

feat(participant): add Participant entity, repository and DTOs

feat(participant): implement ParticipantService and ParticipantController

test(participant): add unit tests for ParticipantService and ParticipantController

feat(registration): add Registration entity, RegistrationStatus enum and repository

feat(registration): implement RegistrationService and RegistrationController

test(registration): add unit tests for RegistrationService and RegistrationController

feat(exception): add GlobalExceptionMiddleware with ResourceNotFoundException and ConflictException

chore(devcontainer): configure dev container with Docker Compose and MySQL

chore(ai): add Cursor rules for project stack, C# style and API conventions

chore(ai): add skills for test-first development and code review

chore(ai): add add-feature workflow
```

## Step 6 — Commit

```bash
git commit -m "$(cat <<'EOF'
<type>(<scope>): <summary>
EOF
)"
```

## Step 7 — Repeat for remaining changes

Return to Step 3 for any files not yet committed. Keep going until `git status` shows a clean tree (or only intentionally untracked files).

## Rules

- One logical unit per commit — entity + repository together, service alone, tests alone.
- Tests for a feature ship in their own commit, after the production code commit.
- `chore(ai)` commits group Cursor rules, skills, and workflows.
- Never use `git add .` or `git add -A` — always name files explicitly.
- Never amend a commit that has already been pushed.
