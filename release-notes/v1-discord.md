## Manimal Interchange zzap v1

**- 다른 맵(연구소 등) 쓰레기 메모리 감소, db 파일을 고쳐도 게임이 켜짐**

■ 바뀐 것
- 인터체인지가 아닌 맵에서 1분에 220~250MB씩 생기던 쓰레기 메모리 — 조명마다 매 프레임 장면 이름을 새로 만들던 것 수정 (연구소 실측 156MB → 0.2MB)
- db 폴더 json을 고치면 서버가 모드를 멈춰 접속이 막히던 것 → 경고만 띄우고 그대로 씀 (서버 재시작 필요, 수정본으로는 미실험)

■ 설치
- 원본 모드를 먼저 받아 설치해야 작동 (이 압축엔 맵 없음): <https://sp-mod.com/mod/3025/manimals-10-interchange-expansion-backport>
- Manimal-Interchange-1.0.8-zzap-v1.zip 풀어서 SPT 폴더에 덮어쓰기 (설정 유지)

■ 주의
- 이상하면 원본 모드 압축을 다시 덮어쓰면 원래대로
- 원작자분께 문의하지 말아 주세요

^^7

-----------------
@알림 @테스터

원본모드 : https://github.com/danauraborealis/ManimalInterchange
다운로드 : https://github.com/danyhappy564-cmyk/ManimalInterchange-zzap--Bootleg-/releases/tag/zzap-v1
