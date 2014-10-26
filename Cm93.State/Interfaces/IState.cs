using System;
using Cm93.Model.Interfaces;

namespace Cm93.State.Interfaces
{
	public interface IState
	{
		string Name { get; set; }
		Guid Key { get; }
		
		DateTime Created { get; }
		DateTime Modified { get; set; }

		IModel Model { get; }
	}
}
