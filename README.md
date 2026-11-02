# Manimal Interchange Rework Backport (zzap 부트렉)

### ⚠️ IMPORTANT NOTICE / DISCLAIMER

- **Original Author:** Manimal (danauraborealis)
- **Original Repository:** ManimalInterchange
- **Original Link:** https://github.com/danauraborealis/ManimalInterchange
- **License:** 원본 저장소에 라이선스 파일 없음 — 원본 저장소 참고
- **This Port By:** R_F (danyhappy564-cmyk) — unofficial, AI-assisted port. Not affiliated with or endorsed by the original author.

1. **Reflection & Take-Downs:** I deeply reflect on the ECOT incident. As an AI-assisted "vibe coder," I will immediately delete files if the original authors ask.
2. **No Re-Distribution:** These ported builds are unverified, temporary fixes. Please do NOT re-upload or share them anywhere else.
3. **Do Not Pester Original Authors:** Never report bugs or pester original modders regarding issues from my unofficial ports.
4. **Full Credit & Respect:** I will always credit original creators on GitHub and prioritize their decisions above all else.
5. **Support Original Creators:** Instead of using my ports, please visit the original authors' Forge pages to leave kind words or tips.

---

## 설치 전에 꼭 — 원본 모드부터 (Install the original mod first)

이 저장소의 빌드는 **원본 인터체인지 확장(Manimal's 1.0 Interchange Expansion Backport) 모드 위에 덮어쓰는 수정본**입니다. 맵 번들 같은 원본 파일은 들어 있지 않습니다.
**원본 모드를 먼저 받아 설치한 뒤, 이 저장소의 압축을 그 위에 덮어써야 작동합니다.**

- 원본 모드: https://sp-mod.com/mod/3025/manimals-10-interchange-expansion-backport

This build only **overwrites the original Manimal's 1.0 Interchange Expansion Backport** - it does not contain the original files (map bundles etc.).
**Install the original mod first, then extract this build over it.** Original: https://sp-mod.com/mod/3025/manimals-10-interchange-expansion-backport

## 변경 이력 (zzap)

- 2026-11-02 — **연구소 등 다른 맵에서 메모리 쓰레기 220~250MB/분 수정.** 면광원(AreaLight) 초기화 함수가 면광원마다 매 프레임 불리는데(연구소 약 140개, 분당 70~80만 회), 인터체인지 장면인지 확인하려고 매번 장면 경로 문자열을 두 번 새로 만들었습니다. 이제 '인터체인지 장면이 아님'으로 판정한 장면은 기억해 두고 다시 묻지 않습니다(인터체인지 장면 경로는 그 장면이 로드되기 전에 등록됨). 확인: 빌드 오류 0, 게임 미확인.

---

아래는 원작 README입니다.

# Manimal Interchange Rework Backport

Backport implementation in progress for replacing SPT 4.1.5's Interchange with the installed retail EFT 1.1.5.47242 map.

Version 1.0.8 supports a separate server installation without `EscapeFromTarkov_Data`. The server validates the installed WTT assemblies, bundle catalog, and declared WTT bundle files and dependencies. Dependencies supplied by the game are resolved on the client; server startup no longer searches for a client game directory. The expanded navmesh from 1.0.7 is retained.

Version 1.0.7 integrates the supplied expanded Interchange navmesh (4,940 tiles) into `manimal_interchange_assets_0e.bundle`. The existing AI scene reference and Waypoints compatibility remain in use. The supplied `interchange_navmesh.asset` has SHA-256 `139a6d198d104c8ffb92ca62bf7c51c85ce67ee94ce02e9a465abe353104afd4` and uses Unity 2022.3.43f1.

After rebuilding the map bundles, run `integrate-navmesh.py` with Python and UnityPy 1.25.3, passing `--asset` for the supplied asset, `--bundle` for the processed `manimal_interchange_assets_0e.bundle`, `--output` for a separate output bundle, and `--report` for verification evidence. The tool preserves bundle object identity and checks every unrelated object and resource stream. Stage that output under the same bundle filename, update its `Sha256` and `Bytes` in both identical content manifests, and use the version from `Directory.Build.props`. Keep the retail source export immutable; repeat this step after every full-map rebuild. The supplied `.meta` GUID must not replace the map's existing GUID.

The full package includes all map assets. When creating a smaller update package for this change, pass `-UpdatedAssetPath bundles/manimal_interchange_assets_0e.bundle` to `package-release.ps1` in PowerShell 7 so the changed bundle accompanies its new manifest. Expanded-navmesh packages remain test packages until bot navigation has been checked in a raid; static asset and Unity loading checks do not establish gameplay coverage.

- [Complete backport plan](docs/INTERCHANGE-BACKPORT-PLAN.md)
- [Implementation status and reproduction](docs/IMPLEMENTATION-STATUS.md)
- [Full-map sky, cable, and outdoor audio corrections](docs/VISUAL-AUDIO-REPAIRS.md)
- [Recovered retail assemblies and source layouts](docs/RETAIL-ASSEMBLY-RECOVERY.md)
- [Verified SPT AI Bridge loader tests](verification/loader-probe.json)
- [Verified native texture import and SPT AI Bridge tests](verification/native-texture-probe.json)
- [Verified shared-asset donor loading through SPT AI Bridge](verification/native-reuse-probe.json)
- [Verified rotated-room geometry through SPT AI Bridge](verification/room-geometry-probe.json)
- [Verified area-light initialization and active scene lifecycle](verification/area-light-probe.json)
- [Verified seasonal terrain and mesh selection](verification/season-probe.json)
- [Verified ballistic hit policies and native explosions](verification/ballistic-trigger-probe.json)
- [Verified retail cloud shader, GPU rendering and cleanup](verification/cloud-probe.json)
- [Preserved AI mine-point and core links](verification/ai-point-links.json)
- [Target conversion progress](verification/conversion-progress.json)
- [Measured findings and comparison tables](docs/AUDIT-FINDINGS.md)
- [Audit methodology and reproduction](analysis/README.md)
- [Scene comparison](analysis/output/scene_diff.csv)
- [Component comparison](analysis/output/component_diff.csv)
- [Asset comparison](analysis/output/asset_diff.csv)
- [Pinned upstream server-data comparison](analysis/output/upstream_comparison.json)
- [Actual SPT 4.1.5 model validation](analysis/output/upstream-model-verification.json)
- [Audit reconciliation and source verification](analysis/output/verification.json)

The source snapshot, exact-target authoring SDK, paired development plugins and an 18-scene loader probe are implemented. Decrypted retail metadata provides 146 analysis assemblies and complete structural recovery of all 232,278 scene components and 989 referenced script assets. Current conversion counts, explicit omissions and unresolved references are recorded in [conversion progress](verification/conversion-progress.json). These count prepared map records; they do not count rewritten game scripts or completed runtime tests. The full replacement map now loads in the development install; broader gameplay validation remains ongoing.

SPT AI Bridge loader tests cover target script binding, repeat loading, cancellation and cleanup. Two native-texture tests verify loading, native graphics allocation, readable byte hashes and cleanup for 16 assets representing 26 source identities. Two shared-asset tests verify all 302 native donor candidates, including file hashes, graphics buffers, audio data loading and identical native instances on repeat. Two room-geometry tests verify the actual patched target method, imported component, oriented boundaries, multiple areas, cleanup and zero warmed allocations. Two area-light tests verify all 1,620 authored brightness/color/shape settings and three active bundled lights through Awake/OnEnable and natural scene cleanup. These tests do not establish a playable full map.

The full-map development package is installed in `D:\SPT41Dev` with a rollback inventory under `build/deployments`. With its development gate enabled, Interchange raids load the replacement map; the user and SPT AI Bridge have confirmed it loads. Visual and ambient-audio regression work is tracked in the correction report above. No core level/database files or reference projects were changed. There is no publication-ready release package yet. Root `Directory.Build.props` defines the mod identity; its source URL remains empty because this project's source repository has not been supplied or published.
