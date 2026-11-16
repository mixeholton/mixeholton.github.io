# Session Brief: "The Grand Buffet" — a food-themed roguelike deckbuilder in Unity

> **For the Claude session reading this:** this is your design brief and work plan. Read it fully before writing code.
> It describes a game far too large for one session. Your job is to build the **foundation and the current milestone**
> (see §12), leave the project in a working, tested state, and update `PROGRESS.md` so the next session can continue.
> Never sacrifice the data pipeline or tests to hit a content count faster.

---

## 0. Ground rules

1. **Genre inspiration, not a clone.** The game takes strong inspiration from the *Slay the Spire 2* style of roguelike
   deckbuilder (turn-based card combat, visible enemy intents, branching map, relics, potions, rest sites, shops, events,
   ascension-style difficulty). **Do not copy** any names, card text, relic effects one-to-one, characters, enemies, art,
   UI layout, music, or story from Slay the Spire 1 or 2. Every card, relic, enemy and character must be original and
   food-themed. Where a mechanic is a genre convention (energy, block, draw pile, exhaust), rename it with food flavor
   (see §2) but keep it readable.
2. **Everything is data.** Cards, relics, enemies, encounters, events and characters live in data files, not in
   hand-written MonoBehaviours. 1,500 cards cannot be maintained any other way.
3. **Logic is engine-independent.** All game rules live in a plain C# library with **zero `UnityEngine` references**, so
   they compile and test with `dotnet test` in a cloud container where the Unity Editor cannot run. Unity is only the
   presentation layer.
4. **Deterministic.** All randomness goes through a seeded RNG owned by the run state. Same seed + same inputs = same run.
5. **Work in small, verified steps.** Commit after each green test run. Update `PROGRESS.md` at the end of every session.

---

## 1. Pitch

A cursed buffet tower has appeared in the middle of the city. Its floors are endless kitchens, cellars and banquet halls
run by food that came to life and grew hungry. At the top waits **The Insatiable Maw**, the hunger that started it all.
Fifteen cooks, eaters and food oddballs climb the Buffet, each fighting with a deck of "recipes" (cards), collecting
"heirloom utensils" (relics) and "condiments" (potions).

Tone: bright, cartoonish, punny, never gross for its own sake. Rotten/fermented enemies are allowed but stay comedic.

---

## 2. Glossary: genre concept → in-game name

| Genre concept | In-game name | Notes |
|---|---|---|
| HP | **Health** | Keep plain for readability |
| Energy | **Heat** (shown as stove burners, base 3) | |
| Block | **Crust** | Removed at the start of your turn unless kept by an effect |
| Gold | **Tips** | |
| Potions | **Condiments** | 3 slots base |
| Relics | **Utensils** | Starter Utensil per character |
| Draw pile / Discard / Exhaust | **Pantry / Sink / Compost** | "Compost" = exhaust |
| Card removal | **Toss** | |
| Strength / Dexterity | **Spice / Zest** | +damage / +Crust gained |
| Weak / Vulnerable / Frail | **Soggy / Tenderized / Stale** | −25% damage / +50% damage taken / −25% Crust |
| Poison-like | **Spoilage** | Damage at turn start, then −1 |
| Curse cards | **Spoiled** cards | |
| Status cards | **Mess** cards (Grease, Crumbs, Burnt) | |
| Rest site | **Kitchen Station** | Simmer (heal 30%) or Season (upgrade a card) + character-specific options from Utensils |
| Shop | **Night Market** | |
| Event | **Mystery Dish** | |
| Treasure | **Pantry Raid** | |
| Ascension | **Heat Levels 1–20** | |
| Card upgrade marker | `+` → shown as **"Seasoned"** | |

Inspired by newer genre features (verify details yourself; adapt, don't copy):
- **Garnishes** – permanent enchantments placed on individual cards via events/shops (e.g. *Truffle Shaving: +3 damage*).
- **Spoiling** – some events/enemies apply negative modifiers to individual cards (e.g. *Moldy: costs +1*).
- **Grand Chefs** – ancient spirit-chefs met at the start of each act who offer a choice of 3 powerful boons
  (replaces a classic "boss relic swap" moment).
- **Co-op (4-player)** is **out of scope** for this project. Design state so it is single-player only.

---

## 3. Structure of a run

- **4 acts**: Acts 1–3 have ~16 floors each + boss. Act 4 is a short gauntlet (rest → shop → elite → final boss).
- **Map**: branching node graph per act, 7 columns wide, generated from seed. Node types: Fight, Elite, Kitchen Station,
  Night Market, Mystery Dish, Pantry Raid, Boss. Rules: no elite before floor 5, no back-to-back Kitchen Stations,
  floor 8 is always Pantry Raid, floor before boss is always a Kitchen Station.
- **Rewards**: after a fight choose 1 of 3 cards (or skip), Tips, chance of a Condiment. Elites always drop a Utensil.
  Bosses drop a rare card choice and Grand Chef boon (Acts 1–2).
- **Act boss** is randomly picked from that act's 3 bosses and shown on the map from the start.

---

## 4. Content counts (the targets)

| Content | Count | Notes |
|---|---|---|
| Playable characters | **15** | Each with unique mechanic + starter Utensil |
| Cards per character | **100** | = 1,500 character cards |
| Shared cards | ~30 Pantry (colorless), 10 Spoiled (curses), 6 Mess (status) | Not counted in the 1,500 |
| Utensils (relics) | **100 shared + 15 starter** | |
| Normal enemies | **75** | 25 per act for Acts 1–3 |
| Elite enemies | **20** | |
| Bosses | **10** | 3 per act for Acts 1–3 + 1 final boss |
| Condiments (potions) | 30 | Supporting content |
| Mystery Dishes (events) | 40 | Supporting content |
| Grand Chefs | 4 chefs × 6 boons | Supporting content |

> 75 normals + 20 elites + 10 bosses = **105 unique combatants**.

### Card rarity split per character (100 cards)

| Rarity | Count | Role |
|---|---|---|
| Starter | 3 | Chop (basic attack), Cover (basic Crust), 1 Signature card |
| Common | 30 | Core of every archetype, simple text |
| Uncommon | 45 | Archetype engines and bridges between archetypes |
| Rare | 22 | Build-defining payoffs, powers, big swings |

Within a character: aim for ~40% Attack, ~40% Skill, ~20% **Recipe** (power-type card that stays in play).
Each character has **3 archetypes**; every archetype needs ≥ 8 commons, ≥ 10 uncommons, ≥ 5 rares, and at least
10 cards should be "bridge" cards useful to two archetypes.

---

## 5. The 15 characters

Each character: base Heat 3, 10-card starting deck (unless noted), one starter Utensil, one unique mechanic, three
archetypes. **Difficulty** guides how complex their card text may be. **HP** is base max health.

| # | Character | HP | Difficulty | Unique mechanic | Archetypes | Starter Utensil |
|---|---|---|---|---|---|---|
| 1 | **Bruno the Butcher** | 82 | ★☆☆ | **Tenderize**: attacks stack Tenderize on enemies; *Carve* cards consume all stacks for +3 damage each. | Carve burst · Spice scaling · Self-damage "Bone Saw" | **Butcher's Block** – start each combat with 1 Spice. |
| 2 | **Saffron, Spice Merchant** | 70 | ★★☆ | **Spice Level 0–10**: many cards raise it; damage scales with it; at 10 you take 5 damage each turn ("Burnout"). Risk meter. | Ride-the-edge · Cooling control · Scoville AoE | **Pepper Grinder** – gain 1 Spice Level at start of turn; at 10 reset to 5 instead of Burnout once per combat. |
| 3 | **Mother Dough, the Baker** | 76 | ★★☆ | **Proofing**: put cards in 3 Oven slots; they bake for N turns and return to hand upgraded and free. Delayed payoff. | Oven engine · Big loaves (slow huge cards) · Crumb tokens | **Sourdough Starter** – the first card Proofed each combat bakes 1 turn faster. |
| 4 | **Min-ji, the Fermenter** | 68 | ★★☆ | **Ferment**: a debuff that *grows* by 1 each turn instead of shrinking, then pops when enemy hits a threshold. | Ferment stacking · Jar catalysts · Contagion spread | **Onggi Jar** – the first Ferment you apply each combat is doubled. |
| 5 | **Kaito, the Itamae** | 66 | ★★★ | **Freshness**: cards are *Fresh* the turn they're drawn and gain bonuses; un-played Fresh cards go *Stale* (weaker) next turn. | Precision combos (cost-order chains) · Knife flurry (0-cost) · Omakase (single big turn) | **Honed Yanagiba** – the first 0-cost card you play each turn deals +3 damage. |
| 6 | **Volt, the Appliance Tinkerer** | 72 | ★★★ | **Outlets**: plug up to 3 Appliances (Toaster, Blender, Microwave…) that trigger each turn; using too many overheats and blows a fuse (lose Heat next turn). | Power grid · Overclock (fuse risk) · Gadget swarm | **Power Strip** – start each combat with 1 Toaster plugged in. |
| 7 | **Bea, the Barista** | 70 | ★★☆ | **Caffeine**: generates extra draw and Heat; at 5+ Caffeine you get **Jitters** (discard a random card each draw). End of combat Crash costs HP if over threshold. | Espresso tempo · Decaf control · Latte art (retain) | **Portafilter** – first time each turn you draw your 3rd extra card, gain 1 Heat. |
| 8 | **Madame Glaze, Confectioner** | 74 | ★☆☆ | **Frosting Layers**: special Crust that persists between turns in layers; when a layer breaks it deals damage to the attacker. Defensive scaling. | Layer cake turtle · Sugar rush offense · Sprinkle multi-hit | **Piping Bag** – at the end of your turn, if you have no Frosting, gain 1 Layer. |
| 9 | **Moss, the Forager** | 64 | ★★☆ | **Mushroom Patch**: summons fungus minions that grow each turn (Sprout → Cap → Giant) and can be sacrificed for effects. | Grow tall (one big mushroom) · Spore swarm (many small) · Decay (sacrifice payoffs) | **Wicker Basket** – start each combat with a Sprout. |
| 10 | **Grease Lightning, Fry Cook** | 78 | ★☆☆ | **Fryer Oil**: cards add Grease (Mess) into piles; Grease can be *Splattered* for AoE. Chaotic, AoE-centric. | Splatter AoE · Mess payoff · Deep-fry (exhaust-for-value) | **Spatula of Speed** – whenever you Compost a Grease, deal 3 damage to ALL enemies. |
| 11 | **Vintner Valentina, Sommelier** | 70 | ★★★ | **Vintage**: cards gain +1 Vintage each time the Pantry is reshuffled while they're in it; Vintage boosts their numbers. Patient scaling, retain-heavy. | Cellar (slow aging) · Pairing (play two linked cards) · Decanting (burst release) | **Corkscrew** – when the Pantry is shuffled, the top card gains 1 Vintage. |
| 12 | **Rico, Taco Truck Vendor** | 74 | ★★☆ | **Order Tickets**: each turn a random ticket appears ("Play 2 Attacks", "Gain 10 Crust"…); fulfilling it gives Tips/Heat/cards. Adaptive. | Rush orders (tempo) · Tip economy (gold scaling) · Menu specials (flexible cards) | **Order Pad** – you see the next turn's ticket in advance. |
| 13 | **Dr. Sphere, Molecular Gastronomist** | 66 | ★★★ | **States of Matter**: you are Solid (defense), Liquid (draw/flow) or Gas (damage); cards shift state and have bonuses per state. | Phase-shifting · Foam (temporary card copies) · Spherification (control) | **Liquid Nitrogen Flask** – first time you change state each turn, draw 1 card. |
| 14 | **Nonna Rosa** | 80 | ★☆☆ | **Leftovers**: Composted cards go to a Leftovers pile and can be reheated (returned to hand, cost 0) by certain cards. Sustain and healing. | Reheat recursion · Comfort food (healing & max HP) · Sunday dinner (big Crust + overheal into temporary HP) | **Wooden Spoon** – at the end of combat, heal 4 HP. |
| 15 | **Chomp, Competitive Eater** | 88 | ★★★ | **Fullness 0–10**: *Devour* cards eat enemy buffs, debuffs or minions to gain stats; at 10 Fullness you are *Stuffed* (−1 Heat) until you Digest. High risk, high reward. | Devour (steal enemy buffs) · Feast (max HP scaling) · Speed eating (cheap cards, Digest synergies) | **Bib of Champions** – Devouring a non-boss enemy grants +2 max HP permanently. |

### Balance identity per character
For every character, write a one-paragraph **balance profile** in `Data/Characters/<id>.json` → `designNotes`:
- Strongest phase (early/mid/late fight, early/late run).
- Main weakness (e.g. Saffron: self-damage; Sommelier: weak early fights; Chomp: Stuffed lockout).
- Target simulator win rate (see §10) at Heat Level 0: **55–65%** for ★☆☆, **45–55%** for ★★☆, **40–50%** for ★★★.

### Starting decks (default)
5× Chop (1 Heat, 6 dmg), 4× Cover (1 Heat, 5 Crust), 1× Signature. Characters may swap one Chop/Cover for a
mechanic-teaching card (e.g. Mother Dough swaps a Cover for *Knead*; Moss swaps a Chop for *Plant Spore*).

---

## 6. Enemies (105 combatants)

Each enemy needs: id, name, act, tier (normal/elite/boss), HP range (and Heat Level scaling), move list with intents,
a move-selection pattern (fixed cycle, weighted random with "no 3 repeats", or conditional), and a one-line flavor text.
Encounters are defined separately (an encounter = 1–4 enemies). Each act has an **easy pool** (first 3 fights) and a
**hard pool**.

### Act 1 — The Pantry

**Normals (25):** Moldy Crouton · Jelly Bean Swarm · Angry Onion (makes you cry: Soggy) · Raisin Gang (group of 3) ·
Breadcrumb Mite · Feral Biscuit · Pickle Sentry · Rice Weevil · Stale Pretzel Knight · Gummi Grub · Pepper Imp ·
Salted Slug · Tin Can Rattler · Expired Yogurt · Wild Garlic Clove · Popcorn Kernel (explodes after 3 turns) ·
Marshmallow Blob (splits at half HP) · Fruit Fly Cloud · Sprouted Potato (grows eyes = more attacks) ·
Rogue Meatball · Soggy Cereal Swarm · Cracker Crab · Sugar Ant Column · Bitter Lemon · Burnt Toast Brute

**Elites (6):** Rotten Cabbage Ogre · Salt & Pepper Twins · The Moldfather · Rolling Pin Golem · Gingerbread Brute ·
Cereal Box Mimic

**Bosses (3):** **The Big Cheese** (wheel that rolls & gains Crust; holes spawn mice) · **Queen Honeycomb** (spawns bee
drones, sticky Honey slows your draws) · **Colonel Cornucopia** (horn that spills random produce enemies every 2 turns)

### Act 2 — The Fermentation Cellar

**Normals (25):** Sauerkraut Shambler · Kombucha Scoby · Vinegar Wraith · Blue Cheese Lurker · Miso Monk ·
Yeast Swarm · Sourdough Shade · Brine Shrimp School · Cave Truffle · Hot Sauce Hexer · Pickled Egg Sentinel ·
Natto String-Spinner · Fig Wasp · Cork Golem · Tempeh Tortoise · Kefir Specter · Fungus Gnat Horde · Anchovy Pack ·
Pickled Herring · Sake Lees Spirit · Fish Sauce Siren · Rotten Tomato Thrower · Sour Cream Slime · Mead Drone ·
Limburger Bog

**Elites (7):** Lutefisk Leviathan · The Cask Knight · Durian Behemoth · Surströmming Abomination ·
Black Garlic Witch · Roquefort Hydra (regrows heads) · Century Egg Oracle (predicts & punishes your next card type)

**Bosses (3):** **The Mother Culture** (grows each turn; splits into cultures) · **Baron von Brine** (pickles your cards:
turns them Spoiled until cleansed) · **The Vintage Phantom** (ages: gets stronger every reshuffle of *your* Pantry)

### Act 3 — The Banquet Hall

**Normals (25):** Champagne Sentry · Caviar Swarm · Truffle Hound · Foie Gras Duelist · Lobster Gladiator ·
Croquembouche Tower · Soufflé Sentinel (deflates if hit 3× in a turn) · Macaron Twins · Oyster Pearl-Spitter ·
Flambé Wisp · Gold Leaf Gilder · Escargot Templar · Crème Brûlée Guard (hard shell) · Canapé Courtier · Pâté Golem ·
Crêpe Cartographer · Sorbet Elemental · Wagyu Bull · Saffron Thread Weaver · Tiramisu Knight ·
Profiterole Bomber · Bisque Bard · Petit Four Quartet · Consommé Mirage · Beef Wellington Warden

**Elites (7):** The Head Waiter (serves "courses" that change his moves) · Baked Alaska (fire/ice phase swap) ·
Turducken (three nested phases) · Peking Duck Assassin · Golden Pineapple Paladin · The Michelin Inspector
(rates your turns; low score = buff for him) · Chocolate Fountain Colossus

**Bosses (3):** **The Wedding Cake Titan** (tiers = phases; each tier removed changes moveset) · **The Maître D'**
(seats waiters, takes "reservations" on your cards) · **The Seven-Course Hydra** (seven heads, one per course)

### Act 4 — The Gullet

**Final boss (1):** **The Insatiable Maw** — 3 phases. Eats cards from your Pantry (returned on its defeat),
grows more hungry each turn, and must be "fed" (Crust or cards) or it devours HP. Preceded by a mandatory elite
gauntlet using one Act-3 elite with Heat Level modifiers.

Totals: 75 normals ✔ · 20 elites ✔ · 10 bosses ✔ = 105

**Encounter variety rule:** a run only has ~5–6 normal fights per act, so a player meets roughly a quarter of an act's
normals per run. To keep all 25 relevant: build ≥ 20 encounters per act, ensure each normal enemy appears in ≥ 2
encounters, never repeat an encounter within a run, and give each enemy one memorable gimmick so it reads distinctly
on first sight.

---

## 7. Utensils (relics): 100 shared + 15 starter

Starter Utensils are listed in §5. The 100 shared Utensils are split:

| Rarity | Count | Examples (write the rest in this spirit) |
|---|---|---|
| Common | 32 | **Lucky Fork** – first attack each combat deals +8. **Oven Mitt** – start combat with 6 Crust. **Lunchbox** – +1 Condiment slot. |
| Uncommon | 30 | **Egg Timer** – every 3rd turn, gain 1 Heat. **Garlic Press** – whenever an enemy dies, apply 2 Tenderized to a random enemy. |
| Rare | 18 | **Golden Ladle** – whenever you heal, also gain that much Crust. **Bottomless Bowl** – the first card each turn returns to your hand once. |
| Grand Chef boon | 10 | Big upside + real drawback. **Chef's Hat of Ambition** – +1 Heat; you can no longer Simmer at Kitchen Stations. |
| Night Market only | 6 | **Loyalty Card** – Night Market prices −25%. |
| Event only | 4 | Obtained only via specific Mystery Dishes. |

Rules: each Utensil has one clear trigger, numbers in data, optional character restriction (max 20 of the 100 may be
character-specific), and a pickup-sound tag. No Utensil may make a fight infinitely winnable without a card combo.

---

## 8. Cards: data format and balance rules

### Card data (example)
```json
{
  "id": "butcher_cleave_through",
  "name": "Cleave Through",
  "character": "butcher",
  "rarity": "common",
  "type": "attack",
  "cost": 1,
  "target": "single_enemy",
  "archetypes": ["carve"],
  "effects": [
    { "op": "deal_damage", "amount": 7 },
    { "op": "apply_status", "status": "tenderize", "amount": 1 }
  ],
  "upgrade": { "effects[0].amount": 10 },
  "keywords": [],
  "text": "Deal {0} damage. Apply 1 Tenderize.",
  "flavor": "Against the grain, always."
}
```

- **Effects use a small op vocabulary** (`deal_damage`, `gain_crust`, `apply_status`, `draw`, `gain_heat`, `compost`,
  `add_card`, `heal`, `summon`, `conditional`, `repeat`, `for_each`…). Character mechanics add ops
  (`proof_card`, `shift_state`, `devour`, `plug_appliance`…). Unique weird cards may reference a named C# handler,
  but **≤ 10%** of a character's cards may need custom code.
- Card text is generated from data where possible so numbers can't drift from text.
- Every card has an upgraded version.

### Baseline numbers (adjust via simulator, not gut feeling)
| Item | Baseline |
|---|---|
| Common attack | ~6–7 damage per Heat |
| Common Crust skill | ~5–6 Crust per Heat |
| Draw | 1 card ≈ 0.5 Heat of value |
| 0-cost cards | ~4 damage or ~3 Crust, or a pure utility effect |
| Upgrade | +30–50% value or −1 cost |
| Rare power (Recipe) | Should change how the deck plays, not just add numbers |

---

## 9. Supporting content (brief)

- **Condiments (30)**: e.g. *Hot Sauce* (deal 20 damage), *Ketchup* (heal 20%), *Maple Syrup* (gain 2 Heat),
  *Soy Sauce* (draw 3), plus 1 per character tied to its mechanic.
- **Mystery Dishes (40)**: text events with 2–4 choices. At least 8 should offer Garnishes, 6 should risk Spoiling.
- **Grand Chefs (4)**: The Gourmand (power), The Ascetic (thin deck), The Alchemist (transform), The Host (Tips &
  Utensils). Each offers 3 of its 6 boons.
- **Heat Levels 1–20**: stacking difficulty modifiers (stronger elites, less healing, Spoiled starter card, etc.).
- **Meta progression**: unlock characters in order of difficulty; unlocks are cards/Utensils added to pools, never raw
  power. Run history and per-character stats.

---

## 10. Technical architecture

- **Engine**: Unity 6 LTS (latest 6000.x), 2D, URP, C#. Target PC (Windows/macOS/Linux), 1920×1080, mouse + controller.
- **Solution layout**
  ```
  /Core                    # netstandard2.1 class library, NO UnityEngine. All rules.
    Combat/ Cards/ Effects/ Statuses/ Enemies/ Map/ Run/ Rng/ Events/
  /Core.Tests              # xUnit (or NUnit) tests, runs with `dotnet test`
  /Sim                     # console app: headless bot plays N runs per character, prints win-rate report
  /Data                    # JSON: cards/<character>.json, utensils.json, enemies/<act>.json, encounters, events…
  /Tools/Validate          # console app: schema + reference validation for all data
  /Unity/GrandBuffet       # Unity project; references Core as a DLL or via asmdef copy; presentation only
  PROGRESS.md              # what is done, what is next, known issues
  ```
- **Core design**: run state → combat state → turn loop; an **event bus** with hooks (`OnCombatStart`, `OnTurnStart`,
  `OnCardPlayed`, `OnDamageDealt`, `OnShuffle`, `OnStatusApplied`…) that cards, statuses, Utensils and enemies subscribe
  to. An **action queue** resolves effects in order (no recursion), which makes animations easy to sequence in Unity.
- **Save/Load**: serialize run state + RNG state to JSON at every node.
- **Unity layer**: card hand with hover/drag-to-target, intent icons, status tooltips, map screen, reward screens,
  shop, rest, event panels. Placeholder art (colored shapes + emoji-free text labels) until real art exists; every
  visual references an asset key from data so art can be dropped in later.
- **Cloud-session limits**: the Unity Editor and Play mode cannot be run in a cloud container. Therefore all
  verification happens in `Core.Tests`, `Tools/Validate` and `Sim`. Unity-side code must stay thin; list anything that
  needs manual Editor checking in `PROGRESS.md` under "Needs human testing in Unity".

### Balance simulator (`/Sim`)
- A greedy-with-lookahead bot that picks cards, paths and rewards with simple heuristics.
- Report per character: win rate, average floor reached, most/least picked cards, cards with win-rate delta,
  Utensils with outlier impact, enemies that cause most deaths.
- Flag: any card picked < 2% or > 40% of the time when offered; any enemy that alone causes > 15% of deaths in its act.
- Re-run after every content batch; save reports in `/Sim/reports/<date>-<batch>.md`.

---

## 11. Quality bar / definition of done for a content item

A card / Utensil / enemy is "done" when:
1. It exists in data and passes `Tools/Validate`.
2. Its text renders correctly (base and upgraded).
3. Any new op or custom handler has a unit test.
4. It has appeared in at least one simulator run batch without crashing.
5. It is original (no name or effect lifted from another game) and on-theme.

---

## 12. Milestones (one or more sessions each)

Work on exactly one milestone per session unless it finishes early. Start each session by reading `PROGRESS.md`.

| # | Milestone | Exit criteria |
|---|---|---|
| M0 | Repo skeleton | Solution builds, empty test passes, Unity project opens (human check), `PROGRESS.md` exists |
| M1 | Combat core | Turn loop, Heat, Crust, statuses, action queue, effect ops, 1 character (Bruno) with 20 cards, 5 Act-1 enemies, all tested |
| M2 | Data pipeline | JSON loading, validator, generated card text, simulator v1 running Bruno vs Act-1 encounters |
| M3 | Run loop | Map generation, rewards, Kitchen Station, Night Market, Pantry Raid, save/load, Act 1 complete (25+6+3 enemies) |
| M4 | Unity vertical slice | Playable Act 1 with Bruno in Unity with placeholder art |
| M5 | Acts 2–4 | All 105 combatants + encounters, simulator coverage |
| M6 | Characters batch A | Bruno (complete to 100), Madame Glaze, Grease Lightning, Nonna Rosa, Saffron — 100 cards each, balanced in sim |
| M7 | Characters batch B | Mother Dough, Min-ji, Bea, Moss, Rico |
| M8 | Characters batch C | Kaito, Volt, Valentina, Dr. Sphere, Chomp |
| M9 | Utensils & Condiments | All 100 + 15 starters, 30 Condiments |
| M10 | Events, Grand Chefs, Garnishes, Spoiling | 40 events, 4 chefs |
| M11 | Heat Levels, meta progression, unlocks | Heat 1–20, run history |
| M12 | Polish | Tooltips, keyword glossary, controller support, audio hooks, settings, accessibility (text size, colorblind intents) |

For character batches: write each character's 100 cards **one archetype at a time**, validate, run the simulator,
fix outliers, then move on. Never dump 100 cards untested.

---

## 13. What to report at the end of every session

Update `PROGRESS.md` with: milestone status, counts (characters/cards/enemies/utensils done vs target), latest
simulator win rates per character, open bugs, and "Needs human testing in Unity". Commit and push.
