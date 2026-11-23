using UnityEngine;

public sealed class PortfolioDemo : DemoCanvas
{
    private enum State { Deploy, Search, Move, Attack, Cleared, Defeated }

    protected override string Title => "GuildGame";
    protected override string Subtitle => "Deploy a guard and an archer, then watch the squad advance.";
    protected override Color Accent => new Color32(133, 218, 155, 255);

    private State _state;
    private int _guards;
    private int _archers;
    private int _stage = 1;
    private int _enemyHp;
    private int _squadHp;
    private float _progress;
    private float _nextAttack;

    private void Update()
    {
        Tick(Time.deltaTime);
    }

    private void StartBattle()
    {
        if (_state != State.Deploy || _guards + _archers == 0)
        {
            return;
        }

        _enemyHp = _stage == 1 ? 68 : 102;
        _squadHp = _guards * 37 + _archers * 17;
        _state = State.Search;
    }

    private void Tick(float delta)
    {
        switch (_state)
        {
            case State.Search:
                _state = State.Move;
                break;
            case State.Move:
                _progress = Mathf.Min(1, _progress + delta * 0.65f);
                if (_progress >= 1)
                {
                    _state = State.Attack;
                }
                break;
            case State.Attack:
                _nextAttack -= delta;
                if (_nextAttack > 0)
                {
                    break;
                }

                _nextAttack = 0.45f;
                _enemyHp = Mathf.Max(0, _enemyHp - (_guards * 8 + _archers * 14));
                if (_enemyHp == 0)
                {
                    _state = State.Cleared;
                    break;
                }

                _squadHp = Mathf.Max(0, _squadHp - (_stage == 1 ? 13 : 19));
                if (_squadHp == 0)
                {
                    _state = State.Defeated;
                }
                break;
        }
    }

    private void ResetBattle(bool advance)
    {
        if (advance && _state == State.Cleared && _stage == 1)
        {
            _stage = 2;
        }

        _state = State.Deploy;
        _guards = 0;
        _archers = 0;
        _enemyHp = 0;
        _squadHp = 0;
        _progress = 0;
        _nextAttack = 0;
    }

    protected override void DrawDemo()
    {
        Fill(new Rect(34, 125, 892, 275), Panel);
        Fill(new Rect(60, 333, 840, 3), Pale);
        float position = 74 + 466 * _progress;
        for (int i = 0; i < _guards; i++)
        {
            Fill(new Rect(position + i * 28, 294, 24, 38), Accent);
        }

        for (int i = 0; i < _archers; i++)
        {
            Fill(new Rect(position + (_guards + i) * 28, 306, 22, 26), new Color32(118, 186, 250, 255));
        }

        if (_enemyHp > 0)
        {
            Fill(new Rect(790, 264, 51, 68), new Color32(241, 126, 115, 255));
        }

        Label(new Rect(59, 146, 806, 36), $"STAGE {_stage}  |  {_state}  |  Squad HP {_squadHp}  |  Enemy HP {_enemyHp}", Pale);
        bool deploying = _state == State.Deploy && _guards + _archers < 4;
        if (Button(new Rect(34, 422, 172, 48), $"Guard  +  ({_guards})", deploying)) { _guards++; }
        if (Button(new Rect(218, 422, 172, 48), $"Archer  +  ({_archers})", deploying)) { _archers++; }
        if (Button(new Rect(402, 422, 165, 48), "Start battle", deploying || (_state == State.Deploy && _guards + _archers == 4))) { StartBattle(); }
        if (Button(new Rect(579, 422, 155, 48), "Reset stage")) { ResetBattle(false); }
        if (Button(new Rect(746, 422, 180, 48), "Next stage", _state == State.Cleared && _stage == 1)) { ResetBattle(true); }
        Label(new Rect(34, 485, 892, 36), "Four deployment slots. Guards absorb damage. Archers finish the fight faster.", Pale);
    }

    public override void CheckScenario()
    {
        StartBattle();
        Require(_state == State.Deploy, "Empty squad started");
        _guards = 1;
        _archers = 2;
        StartBattle();
        Require(_state == State.Search, "Search phase missing");
        Tick(0.01f);
        Require(_state == State.Move, "Move phase missing");
        Tick(2f);
        Require(_state == State.Attack, "Attack phase missing");
        for (int i = 0; i < 6; i++)
        {
            Tick(0.5f);
        }

        Require(_state == State.Cleared, "Stage one did not clear");
        ResetBattle(true);
        Require(_stage == 2 && _state == State.Deploy, "Stage two did not unlock");
        _stage = 1;
        ResetBattle(false);
    }
}
