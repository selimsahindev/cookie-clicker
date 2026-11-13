# Economy tuning guide

Where every balancing number lives and what it does.

## Before you start: Play mode

- **Changes to scene components in Play mode are lost** when you press Stop. Unity restores the scene. Use Play mode to try values, then copy the ones you like and set them again after stopping. Right-click a component, choose *Copy Component*, stop, then *Paste Component Values*.
- **Changes to assets are kept**, even in Play mode. This covers the upgrade definitions in `Assets/Data/Upgrades`. You can tune them live while the game runs, and the result stays.

## 1. Upgrades: `Assets/Data/Upgrades/*.asset`

The four upgrades each have one `UpgradeDefinition` asset. Select one in the Project window to edit it in the Inspector.

| Field | Meaning |
|---|---|
| Display Name | Text shown on the card. |
| Icon | Sprite shown at the top of the card. |
| Base Cost | Price of the first level-up (from Lv 1 to Lv 2). |
| Cost Growth | Multiplier per level. `1.15` means every level costs 15% more. Price at level L = `Base Cost × Cost Growth^(L - 1)`. |
| Max Level | Level cap. `0` means no cap. |
| Base Value | The value at Lv 1, which is the game's starting value. |
| Growth | `Add`: value = Base + Per Level × (L - 1). `Multiply`: value = Base × Per Level^(L - 1). |
| Value Per Level | Step per level: an amount for `Add`, a factor for `Multiply`. |
| Value Format | How the value is written on the card. `{0:0.00}s` → `1.35s`, `+{0:0%}` → `+25%`, `{0:0} $` → `3 $`. |

Current setup:

| Asset | Controls | Base | Per level | Base cost |
|---|---|---|---|---|
| BakeSpeed | Seconds per cookie | 4 s | × 0.88 (12% faster), max Lv 25 (≈0.2 s) | 10 $ |
| CookiePrice | Money per sold cookie | 1 $ | + 1 $ | 25 $ |
| TapPower | Share of a bake added per tap | 20% | + 5%, max Lv 17 (100%) | 15 $ |
| Customers | Average seconds between new customers | 9 s | × 0.88 (12% more often), max Lv 20 (≈0.8 s) | 20 $ |

> The game starts slow on purpose: one cookie every 4 s and a customer about every 9 s. Upgrades are what make the stand busy. Tap Power is the early way to speed things up.

> The upgrade's Lv 1 value **overrides** the same setting on the game component when play starts. To change the starting bake time, tap power, cookie price or customer rate, edit **Base Value** here, not the component.

## 2. Scene components: `Assets/Scenes/Bakery.unity`

Settings that no upgrade controls yet. Select the object in the Hierarchy.

| Object → Component | Field | Meaning |
|---|---|---|
| CookieCart → Cookie Table | Capacity | How many cookies fit on the cart before the vendor waits. Default 10. |
| CookieCart/CookieStacks → Cookie Stack View | Cookie Scale, Stack Spacing, Cookie Height, Cookie Flatness, Cookies Per Stack | Look of the cookie stacks on the cart. Live-tunable in Play mode. |
| Customers → Customer Queue | Max Length | How many customers can stand in line. Default 4. |
| Customers → Customer Queue | Spacing | Distance between customers in the line. |
| Customers → Customer Spawner | Min / Max Cookies Wanted | Each customer wants a random amount in this range. Default 1–5. |
| Customers → Cookie Server | Serve Interval | Minimum seconds between two hand-overs. A customer takes their whole remaining order in one hand-over when the cart has enough cookies. Default 0.35. |
| Economy → Wallet | Starting Balance | Money at the start of the game. Default 0. |
| LevelGoal → Level Goal | Target Earnings | Money to earn from sales to fill the Dream Shop bar and finish the level. Default 10,000. |
| Customers → Customer Spawner | Street | When set, customers walk in along the street from either side, like passers-by who decide to stop, and leave the way they were walking. When empty, they come from *SpawnPoint* and leave to *ExitPoint*. |
| Street → Street | Left X / Right X | Where people enter and leave the screen. |
| Street → Street | Lane Y | Random height range of the walking line. Set a bit below the customer line, so the street is in front of it. |
| Street → Street | Lane Z | Slightly negative, so people on the street are drawn in front of the waiting line. |
| Street → Street | Walk Speed Range | Walking speed of passers-by and of arriving customers. |
| Street → Pedestrian Spawner | Spawn Interval Range, Max Pedestrians | How busy the street is. Passers-by are decoration only: they never buy. How many people stop to buy is the *Customers* upgrade. |
| Street → Pedestrian Spawner | Scale Range | Small size variety among passers-by. |
| Upgrades → Customers → Customer Attraction Effect | Spread | How random customer arrival is. `0.4` means ±40% around the average. |

## 3. Prefabs: `Assets/Prefabs`

| Prefab → Component | Field | Meaning |
|---|---|---|
| Customer → Character Walker | Speed | Walking speed of customers who come from *SpawnPoint*. Street customers use the Street's *Walk Speed Range*. |
| Customer → Customer | Pause Before Leaving | Seconds a served customer stands still before walking off. |
| Pedestrian | (look) | Passers-by. Same sorting order as customers. |
| UpgradeButton | (layout, colors) | Look of every upgrade card at once. |

## Adding a new upgrade later

1. Create a definition: *Assets → Create → Cookie Clicker → Upgrade Definition*.
2. Write an effect, a small `UpgradeEffect` subclass in `Assets/Scripts/Upgrades/Effects` that pushes the value into a game system.
3. Add a child under `Upgrades` in the scene with the effect and an `Upgrade` component. Then add an `UpgradeButton` prefab to `HUD/UpgradeBar` and point it at the upgrade and the shop.

## Save data

- Money, upgrade levels, cookies on the cart, bake progress and Dream Shop progress are saved to `save.json` in `Application.persistentDataPath`.
- *Cookie Clicker → Game Data* shows the current save and resets it. *Cookie Clicker → Reset Game Data* resets it directly.
- Upgrade levels are saved under the definition's file name (`BakeSpeed`, `CookiePrice`...). Renaming a definition asset resets that upgrade's saved level.
- When balancing, reset the save so you start from Lv 1 and 0 $.
