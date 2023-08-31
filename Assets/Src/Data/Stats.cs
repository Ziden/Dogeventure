using System;

namespace Src.Data
{
	
	public class Stats
	{
		public event Action<int> OnHealthChanged;
		public event Action<int> OnMaxHealthChanged;

		public StatData Data = new();

		public short MaxLife
		{
			get => Data.MaxLife;
			set
			{
				Data.MaxLife = value;
				OnMaxHealthChanged?.Invoke(Data.MaxLife);
			}
		}

		public short Life
		{
			get => Data.Life;
			set
			{
				if (value < 0) value = 0;
				if (value > MaxLife) value = MaxLife;
				Data.Life = value;
				OnHealthChanged?.Invoke(Data.Life);
			
			}
		}
	}
}