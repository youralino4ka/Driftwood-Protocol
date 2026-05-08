# Water + Platform Quick Setup

## 1) Water Object
- Create a GameObject named `Water`.
- Add your Stylized Water mesh/material as usual.
- Add components:
  - `WaterSurface`
  - `WaterScroller` (optional if your asset already animates itself)

## 2) Platform Object
- Create `Platform` object (cube/mesh), add `Rigidbody`.
- Recommended Rigidbody values:
  - Mass: `40-120` (depends on size)
  - Use Gravity: `true`
  - Interpolate: `Interpolate`
  - Collision Detection: `Continuous`
- Add component: `RaftPlatformFloat`.

## 3) Float Points
- Create 4-6 empty child objects under `Platform`:
  - Example names: `Float_FL`, `Float_FR`, `Float_BL`, `Float_BR`
  - Place them near platform corners, slightly below deck level.
- Assign all points to `RaftPlatformFloat.floatPoints`.
- Assign `Water` object (with `WaterSurface`) to `RaftPlatformFloat.waterSurface`.

## 4) Optional Floating Loot
- For barrels/debris with Rigidbody, add `SimpleFloatingBody`.
- Assign `waterSurface` reference.

## 5) Start Values
- `WaterSurface`:
  - baseWaterLevel: `0`
  - amplitudeA: `0.35`
  - wavelengthA: `10`
  - speedA: `1.2`
  - amplitudeB: `0.2`
  - wavelengthB: `5`
  - speedB: `1.8`
- `RaftPlatformFloat`:
  - buoyancyForce: `20`
  - maxSubmergeDepth: `1.5`
  - waterDrag: `1.5`
  - waterAngularDrag: `1.2`

If the platform sinks: increase `buoyancyForce` or add more float points.
If it is too jittery: reduce amplitudes in `WaterSurface` and increase Rigidbody mass.
