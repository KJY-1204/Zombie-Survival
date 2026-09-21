# 에셋 인벤토리

`asset-inventory.tsv`는 이 저장소의 `Assets` 아래 실제 에셋 파일 경로와 바이트 크기를 기록한 기준 목록입니다.

- 스냅샷 범위는 `Assets` 하위의 모든 파일이며 Unity `.meta` 파일과 빈 폴더는 제외합니다.
- `.meta` 파일은 해당 에셋의 부수 메타데이터이므로 별도로 목록화하지 않습니다.
- `Library`, `Temp`, 루트의 ZIP 압축본은 Unity 에셋이 아니므로 범위에 포함하지 않습니다.
- 기준 목록은 이 문서와 같은 커밋에 고정합니다.

다음 작업을 시작하기 전에는 아래 명령으로 누락, 의도하지 않은 추가, 파일 크기 변경을 확인합니다.

```powershell
powershell -ExecutionPolicy Bypass -File .\Tools\Verify-AssetInventory.ps1
```

검사 결과가 0이 아니면 출력된 경로를 확인합니다. 새 에셋을 의도적으로 추가하거나 제거한 경우에만 검토 후 다음 명령으로 기준 목록을 갱신하고, 에셋 변경과 인벤토리 갱신을 같은 커밋에 포함합니다.

```powershell
powershell -ExecutionPolicy Bypass -File .\Tools\Verify-AssetInventory.ps1 -Update
```
