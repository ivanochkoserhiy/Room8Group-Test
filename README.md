# Lyra UI & Gameplay Automation Tests

Automated tests for **Lyra Starter Game (UE5.2)** using **AltTester** (AltDriver). This solution includes smoke tests for the main menu and gameplay automation that implements **aim → shoot → confirm kill** against an enemy bot.

---

## Table of Contents

- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [Smoke Test Design](#smoke-test-design)
- [Gameplay Automation Approach](#gameplay-automation-approach)
- [Invasiveness Assessment](#invasiveness-assessment)
- [Lyra Project Modifications](#lyra-project-modifications)
- [Known Limitations](#known-limitations)

---

## Prerequisites

- **.NET SDK** (e.g. .NET 10.0 or the version targeted in the test project)
- **Lyra** (UE5.2 LyraSampleTemplate or equivalent) with **AltTester Unreal SDK** installed and a build running with the AltTester server enabled
- AltTester driver connects by default to `127.0.0.1:13000` — ensure the game is listening before running tests

## Getting Started

1. **Clone the repository**
   ```bash
   git clone https://github.com/ivanochkoserhiy/Room8Group-Test
   cd lyra-ui-tests
   ```

2. **Restore and build**
   ```bash
   dotnet restore
   dotnet build
   ```

3. **Run the Lyra game** (with AltTester enabled) so it is ready to accept driver connections.

4. **Run tests**
   ```bash
   dotnet test --project Room8Group.Lyra.UI.Tests
   ```
   Or run from your IDE (e.g. Rider, Visual Studio) via the test explorer.

---

## Project Structure

```
lyra-ui-tests/
├── Room8Group.Lyra.UI.Tests/     # NUnit test project (AltDriver, FluentAssertions)
│   ├── Tests/
│   │   ├── BaseTest.cs            # AltDriver lifecycle, logger
│   │   ├── DemoTests.cs           # Main menu smoke tests
│   │   └── AimingTests.cs         # Aim / shoot / kill gameplay test
│   └── Room8Group.Lyra.UI.Tests.csproj
├── Room8Group.Lyra.Logic/        # Page objects, components, helpers
│   ├── Pages/
│   │   ├── BasePage.cs
│   │   ├── MainMenuPage.cs       # L_LyraFrontEnd
│   │   └── ShooterGymPage.cs     # L_ShooterGym, EnemyBot
│   ├── Components/
│   │   ├── Gameplay/
│   │   │   └── EnemyBotComponent.cs
│   │   ├── MainMenu/
│   │   │   └── ExperienceSelectionButton.cs
│   │   └── Common/
│   │       ├── BaseComponent.cs
│   │       ├── ButtonComponent.cs
│   │       └── TextComponent.cs
│   ├── Helpers/
│   │   ├── AimingHelper.cs       # Rotate, focus, fire
│   │   ├── DriverExtensionMethods.cs
│   │   └── Wait.cs
│   └── Room8Group.Lyra.Logic.csproj
├── README.md
└── lyra-ui-tests.sln
```

---

## Smoke Test Design

### 1. **TestStartGame** (DemoTests)

**What it does**

- Loads the main menu scene (`L_LyraFrontEnd`).
- Asserts the main menu buttons exist and are enabled: Play, Options, Credits, Show Replay, Quit.
- Asserts that experience-selection buttons (Browse, Host, Quickplay) are **not** visible yet.
- Clicks **Play**.
- Waits until the menu state updates and asserts that experience-selection options **are** visible and enabled, and that the original top-level buttons are no longer visible.

**Rationale**

- Validates that the front-end loads and that the primary entry point (Play) works.
- Covers the risk of broken main menu layout, missing or disabled buttons, or navigation not updating after Play.
- Keeps scope to one critical path (Play → experience selection) so the test stays a fast, stable smoke check.

**Risks covered**

- Main menu not loading or wrong scene.
- Play button missing, disabled, or not triggering navigation.
- UI state not updating after Play (stale or broken flow).

---

### 2. **AimAtEnemyBot_Shoot_ConfirmKill** (AimingTests)

**What it does**

- Loads the gameplay scene that contains the player and an enemy bot (`L_ShooterGym` via `ShooterGymPage`).
- Asserts the enemy bot is present (`EnemyBot.Displayed`).
- Retries the **aim → fire** sequence (rotate to enemy, focus, shoot) using `AimingHelper.Fire` and re-checks `EnemyBot.Displayed`.
- Asserts the enemy is no longer displayed (bot destroyed → **kill confirmed**).

**Rationale**

- Proves that the automation can rotate toward the enemy, focus on him, and shoot, and that the outcome is validated by the bot disappearing (kill), not just by performing input.

**Risks covered**

- Gameplay scene or bot not available.
- Aim/fire automation failing (input not reaching Lyra or not causing damage).
- False positive “success” without a real kill (we assert the bot is gone).

---

## Gameplay Automation Approach

### “Aim, shoot, kill” method

The flow is implemented in **AimingHelper** and used from **AimingTests**:

1. **Rotate to enemy** — `RotateToEnemy(driver, enemyBot, ...)`  
   For a fixed duration (default 1.5 s), the helper repeatedly:
   - Gets the enemy’s current screen position from the **EnemyBotComponent** (backed by AltDriver’s object finding).
   - Moves the mouse to that position via `driver.MoveMouse(..., moveDurationPerStep)` so the in-game view rotates toward the bot.
   - Waits a short poll interval (default 80 ms) and repeats.  
   This turns the camera/aim toward the enemy and, to some extent, tracks if he moves.

2. **Focus on enemy** — `FocusOnEnemy(driver, enemyBot, ...)`  
   One final mouse move to the enemy’s current screen position (default 0.25 s). This fine-tunes the aim so the crosshair is on target.

3. **Shoot** — `driver.PressKey(fireKey, pressDurationSeconds)`  
   Default: left mouse (Mouse0), 0.15 s. Lyra’s Enhanced Input receives this if AltTester input is active.

`Fire(driver, enemyBot, ...)` runs these in order: **RotateToEnemy → FocusOnEnemy → PressKey**.

### Technical choices

- **Screen-space aiming**  
  We use the enemy’s **screen position** from AltTester and `MoveMouse` to drive look and aim. No direct Lyra APIs for “aim at actor” are used. This keeps the solution on the test side and avoids binding to Lyra’s internal APIs.

- **EnemyBotComponent**  
  The enemy is represented by a page-object style component that wraps the AltObject for the bot (found by name, e.g. `"Enemy"` in `ShooterGymPage`). The component exposes `GetScreenPosition()` so AimingHelper can rotate and focus on the same object the test asserts on.

- **Page objects**  
  Scenes are represented as pages (e.g. `MainMenuPage`, `ShooterGymPage`). The gameplay page exposes `EnemyBot` and uses a stable scene name (`L_ShooterGym`) so tests are readable and resilient to small UI hierarchy changes.

### How we validate the kill (not just the aim)

- **Kill = bot no longer in the scene.**  
  We do **not** only assert that Fire ran; we assert that the enemy is **gone** after the sequence.

- In **AimingTests**: after the retried aim/fire loop, we assert  
  `gameplayPage.EnemyBot.Displayed.Should().BeFalse()`.  
  `Displayed` is derived from the AltDriver object for the enemy (e.g. by name). If the bot is destroyed and removed from the level, AltDriver no longer finds it, so `EnemyBot` is null or not displayed → **kill confirmed**.

- So validation is: **enemy object absent after shooting**, not just “we pressed fire.”

### Challenges and resolutions

| Challenge | Resolution |
|----------|------------|
| **Lyra uses Enhanced Input** | AltTester injects input (e.g. mouse, keys) that Unreal receives. We use `MoveMouse` and `PressKey` so the game gets look and fire. No Lyra code changes were required for input. |
| **Ensuring we actually hit** | Rotate + focus use the bot’s live screen position. We retry the full Fire sequence so multiple shots can be attempted if the first misses. |
| **Knowing when the bot is dead** | We avoid relying on Lyra-specific events. We use AltDriver object presence: when the enemy actor is no longer found (e.g. by name), we treat that as a confirmed kill. |
| **AltTester object finding** | We use `TryFindObject` (with optional timeout) so missing or not-yet-spawned objects don’t crash the test; we assert on presence/absence explicitly. |

### Reliability considerations

- **Retries**  
  The aim/fire sequence is run inside `Wait.Retry(...)` so transient misses (e.g. one shot doesn’t kill) can be overcome by repeating the sequence until timeout or success.

- **Timing**  
  Rotate/focus use configurable durations and poll intervals. Defaults (e.g. 1.5 s rotate, 80 ms poll) give the view time to align and allow for some target movement.

- **Scene and object names**  
  Tests depend on scene name (`L_ShooterGym`) and enemy object name (e.g. `"Enemy"`). If your Lyra build uses different names, update `ShooterGymPage` (and any selectors) to match.

- **Stability across runs**  
  If the bot starts in different positions or the level loads slowly, the first rotation might not be perfect. The combination of rotate duration, focus step, and retries is intended to make the test pass consistently as long as the bot is in view and killable within the retry window.

---

## Invasiveness Assessment

### Changes beyond installing the AltTester plugin

This repository contains **only the C# test project and shared logic**. It does **not** include the Lyra Unreal project.

- **No Lyra source or content is modified in this repo.**  
  All automation is implemented in the test solution using AltDriver (find object, move mouse, press key, load scene).

- **Assumptions about Lyra:**  
  - A build of Lyra (e.g. LyraSampleTemplate) is run with the **AltTester Unreal SDK** installed and the AltTester server enabled.  
  - A level named `L_ShooterGym` (or the name you configure in `ShooterGymPage`) exists and contains an enemy object that AltTester can find by name (e.g. `"Enemy"`).  
  - The game accepts AltTester-injected input for look (mouse) and fire (e.g. Mouse0).

**If you need to change Lyra** (e.g. add a test map, rename an actor, or expose an object to AltTester):

- **Why it might be necessary**  
  For example: no level with a single, findable enemy; or object names/tags not exposed in a way AltTester can query.

- **Less invasive options first**  
  Prefer reusing an existing map and setting the enemy actor’s name/tag in the editor so AltTester can find it. Add a dedicated test map or “Shooter Gym” only if no existing level fits.

- **Clean removal**  
  Any new map or actor is just another asset; renaming or removing it reverts the change. No core Lyra code need be touched if you only add/configure levels and object names for testability.

---

## Lyra Project Modifications

This section applies **only if** you introduce changes in the Lyra project to support these tests.

### If you add or adjust a level for the gameplay test

1. **Create or duplicate a level** (e.g. “Shooter Gym”) that includes:
   - A player start.
   - An enemy bot (or Lyra bot pawn) that can be damaged and destroyed (e.g. has health and is removed on death).

2. **Ensure the level is loadable** by AltDriver: the **level name** (as used in `LoadScene`) must match what Unreal reports (e.g. `L_ShooterGym`). Confirm via AltTester/Unreal docs or a quick `GetCurrentScene()` after load.

3. **Set the enemy object name** (or tag) so AltDriver can find it. In the test we use a single enemy found by **name** (e.g. `"Enemy"` in `ShooterGymPage`). In Unreal:
   - Set the enemy actor’s **Name** (or the relevant identifier) to that value in the level or via spawn logic.

4. **No code changes are required** in Lyra for the current approach: we use only AltTester’s object finding, `MoveMouse`, and `PressKey`.

### If you need to expose or rename objects

- Use the editor or minimal spawn/init logic to set the enemy’s **name** (or tag) to the value used in `ShooterGymPage` / `EnemyBotComponent` (e.g. `"Enemy"`).
- If you add a `/ProjectMods/` folder to this repo, you can put there:
  - Short instructions (e.g. “Set enemy name to ‘Enemy’ in L_ShooterGym”).
  - Any small snippets (e.g. Blueprint or C++ that sets the actor label) that you want to share.  
  We do **not** upload the full Lyra project.

---

## Known Limitations

- **Scene and object names**  
  Tests are tied to scene name `L_ShooterGym` and enemy name `"Enemy"` (or whatever is configured in `ShooterGymPage`). Different builds or levels require updating the page object.

- **Single enemy**  
  The gameplay test assumes one enemy in the level (or at least one findable by the given name). Multiple enemies with the same name may make “kill confirmed” ambiguous (e.g. we might kill one but find another).

- **Input and Enhanced Input**  
  We rely on AltTester’s input injection being received by Lyra. If Lyra or the build disables or overrides this input, rotate/fire may not work without project-side changes (e.g. ensuring AltTester input is active).

- **Timing and difficulty**  
  Default rotate/focus durations and retry timeouts are tuned for a typical case. Very fast or distant targets might need longer rotate time or more retries; this may require parameter tuning per level.

- **No Lyra code in this repo**  
  This solution does not include Unreal/Lyra code. Any required level setup or object naming must be done in your Lyra project and documented (e.g. in this README or in `/ProjectMods/`).

- **Improvements with more time**  
  - Parameterize scene and enemy names (e.g. via config or env) for different builds.  
  - Add a small delay after load before starting rotate/fire to allow physics and AI to settle.  
  - Optionally use AltTester’s `CallMethod` or similar to query health/death from the game if Lyra exposes it, for stricter kill validation.

---

## License

See repository license file (if present).
