# 스테이지 해금과 버튼 재사용

첫 스테이지는 바로 열립니다. 같은 월드의 다음 스테이지는 직전 스테이지의 클리어를, 새 월드의 첫 스테이지는 이전 월드의 마지막 스테이지 클리어를 확인합니다. 패널은 클리어·해금·잠김 상태에 맞는 버튼을 선택합니다. 버튼을 반환하면 자식 목록이 바뀔 수 있으므로 반환할 버튼을 먼저 모읍니다.

## StagePanel.cs 발췌

```csharp
public bool IsStageUnlocked(int world, int level, int maxLevelPerWorld)
{
    if (world == 1 && level == 1) return true;

    if (level > 1)
    {
        return IsStageCleared(world, level - 1);
    }
    else
    {
        return IsStageCleared(world - 1, maxLevelPerWorld);
    }
}

public LoadBattleSceneBtn UpdateStageButton(int world, int level, int maxLevelPerWorld)
{
    bool cleared = IsStageCleared(world, level);
    bool unlocked = IsStageUnlocked(world, level, maxLevelPerWorld);

    if (cleared)
    {
        return clearedStage;
    }
    else if (unlocked)
    {
        return defaultStage;
    }
    else
    {
        return lockedStage;
    }
}

public void ReleaseStageButtons()
{
    List<GameObject> stageButtons = new List<GameObject>();

    foreach (Transform child in transform)
    {
        LoadBattleSceneBtn stageBtn = child.GetComponent<LoadBattleSceneBtn>();
        if (stageBtn != null)
        {
            stageButtons.Add(stageBtn.gameObject);
        }
    }

    for (int i = 0; i < stageButtons.Count; i++)
    {
        GameManager.Instance.ObjectPool.ReturnToPool(stageButtons[i]);
    }
}
```
