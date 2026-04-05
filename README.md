# OECS — Simple Entity Component System for Unity

A lightweight, educational **Entity Component System (ECS)** framework for Unity that leverages built-in ScriptableObjects and MonoBehaviours. It provides a clean pattern for decoupling game data from logic through entities, groups, and systems — without any external dependencies.

---

## Table of Contents

- [Requirements](#requirements)
- [Installation](#installation)
- [Architecture Overview](#architecture-overview)
- [Core Classes](#core-classes)
  - [Entity](#entity)
  - [EntitiesGroup](#entitiesgroup)
  - [EntitySystem](#entitysystem)
  - [ScriptableObject Variables](#scriptableobject-variables)
- [How to Use](#how-to-use)
  - [1. Create an Entity](#1-create-an-entity)
  - [2. Create an EntitiesGroup asset](#2-create-an-entitiesgroup-asset)
  - [3. Create a System](#3-create-a-system)
  - [4. Wire everything in the Inspector](#4-wire-everything-in-the-inspector)
- [Examples](#examples)
  - [LookAtEnemy — Tanks looking at a target](#lookaenemy--tanks-looking-at-a-target)
  - [Sea — Water height simulation](#sea--water-height-simulation)
- [Project Structure](#project-structure)

---

## Requirements

- **Unity 2018.1.0f2** (the project was built and tested on this version)
- No external packages or dependencies

---

## Installation

1. Clone this repository:
   ```
   git clone https://github.com/omar92/OECS.git
   ```
2. Open the project folder in **Unity Hub** and select Unity **2018.1.0f2** as the editor version.
3. Unity will import all assets automatically — no additional setup is needed.

---

## Architecture Overview

OECS follows the classic ECS pattern adapted to Unity's component model:

| Concept | Unity type | Role |
|---|---|---|
| **Entity** | `MonoBehaviour` | Marks a GameObject as a participant in one or more groups; holds references to its own data components |
| **EntitiesGroup** | `ScriptableObject` | Owns a list of live entities and provides methods to execute logic over all of them |
| **EntitySystem** | `MonoBehaviour` | Contains the logic (the *how*); fetches an `EntitiesGroup` and operates on every entity inside it |
| **Variables** | `ScriptableObject` | Typed data containers (e.g., `TransformVariable`) shared between systems and entities via inspector references |

The key insight is that **EntitiesGroup is a ScriptableObject asset**, so it can be referenced simultaneously by many scenes and systems without any singleton or hard-coded dependency.

```
┌──────────────────────────────┐
│  EntitySystem (MonoBehaviour)│
│  - holds EntitiesGroup ref   │
│  - calls ExcuteInEntities()  │
└──────────┬───────────────────┘
           │ iterates
           ▼
┌──────────────────────────────┐
│  EntitiesGroup (ScriptableObj│
│  - List<Entity>              │
│  - Register / Unregister     │
└──────────┬───────────────────┘
           │ contains
           ▼
┌──────────────────────────────┐
│  Entity (MonoBehaviour)      │
│  - OnEnable  → Register      │
│  - OnDisable → Unregister    │
└──────────────────────────────┘
```

---

## Core Classes

### Entity

**File:** `Assets/OECS/Entity.cs`

Abstract base class for every game object that participates in the ECS. Extend this class to attach game-specific data fields.

```csharp
public abstract class Entity : MonoBehaviour
{
    // Assign one or more EntitiesGroup assets in the Inspector.
    public List<EntitiesGroup> EntitiesGroups;
}
```

- `OnEnable` — automatically registers this entity with all groups in `EntitiesGroups`.
- `OnDisable` — automatically unregisters from all groups (safe even if the group asset is `null`).

---

### EntitiesGroup

**File:** `Assets/OECS/EntitiesGroup.cs`

A `ScriptableObject` created via **Assets → Create → Entities Group**. Acts as a runtime registry and batch-execution engine.

```csharp
// Execute an action on every registered entity immediately (synchronous, reverse order).
public void ExcuteInEntities(Action<Entity> action);

// Execute an action on every registered entity as a coroutine.
// sysInstance  — the MonoBehaviour that owns the coroutine
// IsLoop       — if true, repeats indefinitely
public Coroutine ExcuteOnEntitiesCo(Action<Entity> action,
                                     EntitySystem sysInstance,
                                     bool IsLoop);

// Register / unregister entities (called automatically by Entity).
public void RegisterListener(Entity entity);
public void UnregisterListener(Entity entity);

// Direct index access.
public Entity GetEntity(int i);
```

> **Tip:** The coroutine variant spreads work across frames when the per-frame budget (`Time.fixedDeltaTime`) is exceeded, keeping the frame rate smooth for large groups.

---

### EntitySystem

**File:** `Assets/OECS/EntitySystem.cs`

Abstract base class for systems. Provides the `entitiesGroup` field and a starting point for per-system logic.

```csharp
public abstract class EntitySystem : MonoBehaviour
{
    public EntitiesGroup entitiesGroup;
}
```

Override `Start` (or `Update`) in your subclass and call `entitiesGroup.ExcuteInEntities(...)` or `entitiesGroup.ExcuteOnEntitiesCo(...)` to act on every entity.

---

### ScriptableObject Variables

Typed wrappers that expose a single value as a sharable asset, avoiding direct scene-to-scene references.

| Class | Asset menu | Stored value |
|---|---|---|
| `TransformVariable` | *Transform Variable* | `Transform value` |
| `TransformArray2DVariable` | *Transform Array 2D Variable* | `Transform[,] value` |

Both expose a `SetValue(...)` method for runtime writes.

---

## How to Use

### 1. Create an Entity

Create a C# script that extends `Entity` and add the fields your system needs:

```csharp
using OECS;

public class TankEntity : Entity
{
    public Transform Head; // exposed in the Inspector
}
```

Attach this script to a prefab or scene GameObject. In the Inspector, assign one or more `EntitiesGroup` assets to **Entities Groups**.

---

### 2. Create an EntitiesGroup asset

In the **Project** window: **right-click → Create → Entities Group**.

Name it something descriptive (e.g., `TanksGroup`). This asset will automatically collect all active `TankEntity` instances at runtime.

---

### 3. Create a System

Create a C# script that extends `EntitySystem`:

```csharp
using OECS;
using UnityEngine;

public class EnemiesSystem : EntitySystem
{
    public TransformVariable Target;

    void Start()
    {
        // Process all entities every frame as a looping coroutine.
        entitiesGroup.ExcuteOnEntitiesCo(LookAtTarget, this, true);
    }

    void LookAtTarget(Entity entity)
    {
        TankEntity tank = (TankEntity)entity;
        tank.Head.LookAt(Target.value);
    }
}
```

---

### 4. Wire everything in the Inspector

1. Add the system script to a GameObject in the scene.
2. Assign the `EntitiesGroup` asset to **Entities Group**.
3. Assign any `Variable` assets (e.g., `Target`) to the corresponding fields.
4. Ensure every entity prefab has its **Entities Groups** list pointing to the same group asset.

---

## Examples

### LookAtEnemy — Tanks looking at a target

**Scene:** `Assets/LookAtEnemy/LookAtEnemy.unity`

Demonstrates batch entity processing using a coroutine loop.

- **TankEntity** holds a `Head` transform.
- **EnemiesSystem** calls `ExcuteOnEntitiesCo` each frame; for each entity it calls `Head.LookAt(Target.value)`.
- Moving the **Target** prefab at runtime causes all tank turrets to track it immediately.

**Key assets:**

| Asset | Purpose |
|---|---|
| `EnemiesGroup.asset` | Runtime list of all active tanks |
| `Target.asset` | `TransformVariable` pointing to the target |
| `Tank.prefab` | Tank GameObject with `TankEntity` attached |
| `Target.prefab` | The object tanks will look at |

---

### Sea — Water height simulation

**Scene:** `Assets/Sea/Water.unity`

Demonstrates grid-based entity processing and shared 2D data arrays.

- **MapBuildr** procedurally creates an N×M grid of cubes and stores their transforms in a `TransformArray2DVariable` asset (`SeaCells`).
- Each cube has a **SeaCellEntity** component and registers with `ESeaGroup`.
- **SeaSystem** runs every frame. For each cell it reads the four neighbours and transfers a fraction (`transfareRatio`, default `0.1`) of the height surplus to each lower neighbour, simulating water seeking equilibrium.

**To try it:**
1. Open the `Water.unity` scene and press **Play**.
2. Select any cube in the Hierarchy and set its **Scale Y** to a value greater than `1` in the Inspector.
3. Watch the height ripple outward to neighbouring cells.

**Key assets:**

| Asset | Purpose |
|---|---|
| `ESeaGroup.asset` | Runtime list of all active sea cells |
| `SeaCells.asset` | `TransformArray2DVariable` — the full grid |
| `Cube.prefab` | Single water-cell GameObject |

**Inspector settings on MapBuildr:**

| Field | Description |
|---|---|
| `Mapsize` | `Vector2` — number of columns and rows (e.g., `10, 10`) |
| `MapCell` | Prefab to instantiate for each cell |
| `MapCellsRef` | Reference to the `SeaCells` asset |

**Inspector settings on SeaSystem:**

| Field | Description |
|---|---|
| `Entities Group` | Reference to `ESeaGroup.asset` |
| `MapCellsRef` | Reference to `SeaCells.asset` |
| `Transfareatio` | Height transfer fraction per frame (default `0.1`) |

---

## Project Structure

```
OECS/
├── Assets/
│   ├── OECS/                          # Core ECS framework
│   │   ├── Entity.cs                  # Abstract entity base class
│   │   ├── EntitiesGroup.cs           # ScriptableObject group & batch executor
│   │   └── EntitySystem.cs            # Abstract system base class
│   │
│   ├── SO_Variables/                  # Typed ScriptableObject data containers
│   │   ├── TransformVariable.cs
│   │   └── TransformArray2DVariable.cs
│   │
│   ├── MonoBehaviourEventsHandler.cs  # Exposes Unity lifecycle events as UnityEvents
│   │
│   ├── LookAtEnemy/                   # Example 1: tank turret targeting
│   │   ├── TankEntity.cs
│   │   ├── EnemiesSystem.cs
│   │   ├── EnemiesGroup.asset
│   │   ├── Target.asset
│   │   ├── Tank.prefab
│   │   ├── Target.prefab
│   │   └── LookAtEnemy.unity
│   │
│   └── Sea/                           # Example 2: water equilibrium simulation
│       ├── SeaCellEntity.cs
│       ├── SeaSystem.cs
│       ├── MapBuildr.cs
│       ├── ESeaGroup.asset
│       ├── SeaCells.asset
│       ├── Cube.prefab
│       └── Water.unity
│
├── Packages/
│   └── manifest.json                  # No external packages
└── ProjectSettings/                   # Unity project settings
```

