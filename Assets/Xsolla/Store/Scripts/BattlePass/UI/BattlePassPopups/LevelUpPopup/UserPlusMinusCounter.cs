using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class UserPlusMinusCounter : MonoBehaviour
    {
		[SerializeField] private GameObject CounterHolder;
		[SerializeField] private SimpleButton CounterPlusButton;
		[SerializeField] private SimpleButton CounterMinusButton;
		[SerializeField] private Text CounterValue;

		private int _currentCounterValue = 0;
		private int _lowerLimit;
		private int _upperLimit;

		public event Action<int> CounterChanged;

		private void Awake()
		{
			CounterPlusButton.onClick += () => OnCounterChange(1);
			CounterMinusButton.onClick += () => OnCounterChange(-1);
		}

		public void Initialize(int initialValue, int lowerLimit, int upperLimit)
		{
			_currentCounterValue = initialValue;
			_lowerLimit = lowerLimit;
			_upperLimit = upperLimit;

			CounterValue.text = _currentCounterValue.ToString();
		}

		public void ShowCounter(bool show)
		{
			CounterHolder.SetActive(show);
		}

		private void OnCounterChange(int delta)
		{
			var newCounterValue = _currentCounterValue + delta;

			if (newCounterValue >= _lowerLimit && newCounterValue <= _upperLimit)
			{
				_currentCounterValue = newCounterValue;
				CounterValue.text = newCounterValue.ToString();
				if (CounterChanged != null)
					CounterChanged.Invoke(newCounterValue);
			}
		}
	}
}
