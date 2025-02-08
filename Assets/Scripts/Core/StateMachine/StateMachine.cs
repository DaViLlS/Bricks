using System;
using UnityEngine;

namespace _Project.Development.Core.StateMachine
{
    public abstract class StateMachine : MonoBehaviour
    {
        public event Action OnPaused;
        public event Action OnResumed;
        
        private IState _currentState;
        
        public IState CurrentState => _currentState;
        private bool _isPaused;

        public abstract void Initialize();

        public void ChangeState(IState nextState)
        {
            if (_isPaused)
                return;
            
            _currentState?.OnExitState();
            _currentState = nextState;
            _currentState.OnEnterState();
        }

        private void Update()
        {
            ExecuteState();
        }

        private void FixedUpdate()
        {
            FixedExecuteState();
        }

        public void FixedExecuteState()
        {
            if (!_isPaused)
                _currentState.FixedExecute();
        }

        public void ExecuteState()
        {
            if (!_isPaused)
                _currentState.Execute();
        }

        public void Pause()
        {
            _isPaused = true;
            OnPaused?.Invoke();
        }

        public void Resume()
        {
            _isPaused = false;
            OnResumed?.Invoke();
        }
    }
}
