using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiredGunsData;
using HGHelperUtilities;
using System.Windows.Forms;

namespace Hired_Guns_3_Map_Editor
{
    class StateManager
    {
        public int stateAmount = 10;
        private MainForm _mainForm; 
        private HGMapData[] _mapStates;
        private int _stateIndex;
        private int _stepIndex;
        public StateManager(MainForm mainForm)
        {
            this._mainForm = mainForm;
            ResetStates();
        }
        public void AddStep()
        {
            if(_mainForm == null || _mainForm.GetMapData() == null)
            {
                return;
            }
            /*
            if(_stateIndex == _mapStates.Length - 1)
            {
                _mapStates[0] = null;
            }
            else
            {
                _mapStates[_stateIndex + 1] = null;
            }
            _mapStates[_stateIndex] = (HGMapData)_mainForm.GetMapData().Clone();
            _stateIndex++;
            if(_stateIndex == _mapStates.Length)
            {
                _stateIndex = 0;
            }
            _stepIndex = 0;
            _mainForm.AddConsoleMessage($"[AddStep] _stateIndex: {_stateIndex}, _stepIndex: {_stepIndex}");
            */
            /*
            _mapStates[_mapStates.Length - 1] = (HGMapData)_mainForm.GetMapData().Clone();
            HGMapData[] _mapStateTemp = new HGMapData[_mapStates.Length];
            Array.Copy(_mapStates, 1, _mapStateTemp, 0, _mapStates.Length - 1);
            _mapStates = _mapStateTemp;
            _mapStates[_mapStates.Length - 1] = null;
            */
            /*
            HGMapData[] _mapStateTemp = new HGMapData[_mapStates.Length];
            Array.Copy(_mapStates, 1, _mapStateTemp, 0, _mapStates.Length - 1);
            _mapStates = _mapStateTemp;
            _mapStates[_mapStates.Length - 1] = (HGMapData)_mainForm.GetMapData().Clone();
            */
            HGMapData[] _mapStateTemp = new HGMapData[_mapStates.Length];
            int indexA = HelperUtil.Clamp<int>(1 + _stepIndex,0,1);
            int indexB = HelperUtil.Clamp<int>(Math.Abs(_stepIndex) - 1, 0, Math.Abs(_stepIndex) - 1);
            int length = HelperUtil.Clamp<int>(_mapStates.Length - indexB - 1,0, _mapStates.Length - indexB - 1);
            Array.Copy(_mapStates, indexA, _mapStateTemp, indexB, length);
            _mapStates = _mapStateTemp;
            _mapStates[_mapStates.Length - 1] = (HGMapData)_mainForm.GetMapData().Clone();
            //_mainForm.AddConsoleMessage($"_stepIndex: {_stepIndex}, indexA: {indexA}, indexB: {indexB}, length: {length}");
            
            _stepIndex = 0;
        }
        public HGMapData StepNext()
        {
            /*
            int _stepIndexTemp = _stepIndex;
            _stepIndex++;
            if (_stepIndex > _mapStates.Length)
            {
                _stepIndex = _mapStates.Length;
            }
            int stepIndexCur = _stateIndex + _stepIndex;
            if(stepIndexCur < 0)
            {
                stepIndexCur = _mapStates.Length - Math.Abs(_stepIndex);
            }
            else if (stepIndexCur >= _mapStates.Length)
            {
                stepIndexCur = Math.Abs(_stepIndex) - _mapStates.Length;
            }
            if (_mapStates[stepIndexCur] != null)
            {
                _mainForm.AddConsoleMessage($"[StepNext] stepIndexCur: {stepIndexCur}");
                return _mapStates[stepIndexCur];
            }
            else
            {
                _mainForm.AddConsoleMessage($"[StepNext] _mapStates[{stepIndexCur}] is null");
            }
            _stepIndex = _stepIndexTemp;
            */
            int stepIndexTemp = _stepIndex;
            _stepIndex++;
            if (_stepIndex > 0)
            {
                _stepIndex = 0;
            }
            int stateIndex = _mapStates.Length - 1 + _stepIndex;
            if (stateIndex < _mapStates.Length && _mapStates[stateIndex] != null)
            {
                return _mapStates[stateIndex];
            }
            else
            {
                _stepIndex = stepIndexTemp;
            }
            return null;
        }
        public HGMapData StepBack()
        {
            /*
            int _stepIndexTemp = _stepIndex;
            _stepIndex--;
            if(_stepIndex <= -_mapStates.Length)
            {
                _stepIndex = -_mapStates.Length;
            }
            int stepIndexCur = _stateIndex + _stepIndex;
            if (stepIndexCur < 0)
            {
                stepIndexCur = _mapStates.Length - Math.Abs(_stepIndex);
            }
            else if (stepIndexCur >= _mapStates.Length)
            {
                stepIndexCur = Math.Abs(_stepIndex) - _mapStates.Length;
            }
            if (_mapStates[stepIndexCur] != null)
            {
                _mainForm.AddConsoleMessage($"[StepBack] stepIndexCur: {stepIndexCur}");
                return _mapStates[stepIndexCur];
            }
            else
            {
                _mainForm.AddConsoleMessage($"[StepBack] _mapStates[{stepIndexCur}] is null");
            }
            _stepIndex = _stepIndexTemp;
            return null;
            */
            int stepIndexTemp = _stepIndex;
            _stepIndex--;
            if (_stepIndex < -_mapStates.Length + 1)
            {
                _stepIndex = -_mapStates.Length + 1;
            }
            int stateIndex = _mapStates.Length - 1 + _stepIndex;
            if (stateIndex < _mapStates.Length && _mapStates[stateIndex] != null)
            {
                return _mapStates[stateIndex];
            }
            else
            {
                _stepIndex = stepIndexTemp;
            }
            return null;
        }
        
        public void ResetStates()
        {
            _mapStates = new HGMapData[stateAmount];
            for (int i = 0; i < _mapStates.Length; i++)
            {
                _mapStates[i] = null;
            }
            
            _stateIndex = 0;
            _stepIndex = 0;
        }
    }
}
