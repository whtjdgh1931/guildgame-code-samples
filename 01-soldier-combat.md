# 병사 전투 FSM

병사는 적을 찾고, 사거리까지 이동하고, 공격합니다. 대상이 사라지면 탐색 상태로 돌아갑니다. 전투가 진행되는 동안 기본 공격과 스킬의 경과 시간을 각각 갱신하고, 스턴 중에는 FSM을 실행하지 않습니다.

## FSM.cs 발췌

```csharp
public void Update()
{
		if (!SoldierManager.Instance().isBattle) return;
		curTime += Time.deltaTime;
		curAttackTime += Time.deltaTime;


		if(soldier_Anim != null)
		soldier_Anim.SetAnimMove(soldier_Move.soldierNav.velocity);


		if (SoldierManager.Instance().isBattle &&!isStun)
				DoFsm();


}

public void DoFsm()
{
		if (targetSoldier == null) condition = Soldier_Condition.SEARCH;
		switch (condition)
		{
				case Soldier_Condition.NONE:
						break;
				case Soldier_Condition.SEARCH:
						SearchEnemy(tag);

						break;
				case Soldier_Condition.MOVE:
						SoldierMove();
						break;
				case Soldier_Condition.ATTACK:
						SoldierAttack();
						break;
		}
}

public void SoldierAttack()
{
		if (targetSoldier == null)
		{
				condition = Soldier_Condition.SEARCH;
				return;
		}
		if(soldier.name == Constants.NAME_Player&&!isTaunt)
		{
				player_Attack.DoAutoAttack();
				return;
		}
		if (curTime > skillCoolTime && !isTaunt)
		{
				curTime = 0;
				curAttackTime = 0;
				soldier_Attack.DoSkill(targetSoldier);
				if(soldier_Anim!=null)
				soldier_Anim.SetAnimSkill();

		}
		else if (curAttackTime > attackSpeed)
		{
				soldier_Attack.DoAttack(targetSoldier);
				curAttackTime = 0;
				if (soldier_Anim != null)
				soldier_Anim.SetAnimAttack();
		}

		condition = Soldier_Condition.MOVE;

}
```
