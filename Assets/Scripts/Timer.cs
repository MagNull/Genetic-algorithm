using System;

namespace DefaultNamespace
{
    public class Timer
    {
        private Action _timerAction;
        private float _currentTick;

        public Timer(float time, Action action)
        {
            _currentTick = time;
            _timerAction = action;
        }

        public void Tick(float tickDelta)
        {
            _currentTick -= tickDelta;
            if (_currentTick <= 0)
            {
                _timerAction?.Invoke();
            }
        }
    }
}