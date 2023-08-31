using System.Collections.Generic;
using UnityEngine;

namespace Src.Services
{
	public class MapService : IService
	{
		public void OnSceneChange()
		{
			ResetMapRuntimeData();
		}

		public MapService()
		{
			//Main.Services.Scenes.OnMapLoaded += _ => ResetMapRuntimeData();
		}
		
		private bool _gameFrozen = false;
		private HashSet<Monster> _aggro = new ();
		private HashSet<Interactible> _seeingInteractibles = new ();

		public void AddAggro(Monster monster) => _aggro.Add(monster);
		public void RemoveAggro(Monster monster) => _aggro.Remove(monster);
		public void SeeInteractible(Interactible i) => _seeingInteractibles.Add(i);
		public void UnseeInteractible(Interactible i) => _seeingInteractibles.Remove(i);
		public IReadOnlyCollection<Monster> Aggro => _aggro;
		public IReadOnlyCollection<Interactible> ViewingInteractibles => _seeingInteractibles;

		public void ResetMapRuntimeData()
		{
			_aggro.Clear();
			_seeingInteractibles.Clear();
		}
		
		public bool GameFrozen
		{
			get => _gameFrozen;
			set
			{
				Physics.simulationMode = value ? SimulationMode.Script : SimulationMode.FixedUpdate;
				_gameFrozen = value;
			}
		}
	}
}