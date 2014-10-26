using System;
using System.Collections.Generic;
using Cm93.Model.Enumerations;
using Cm93.Model.Interfaces;
using Cm93.Model.Structures;
using Cm93.State.Interfaces;

namespace Cm93.State.Game
{
	public class State : IState
	{
		public string Name { get; set; }
		public Guid Key { get; private set; }
		public DateTime Created { get; private set; }
		public DateTime Modified { get; set; }
		public IModel Model { get; private set; }

		public State(string name)
		{
			Name = name;
			Key = Guid.NewGuid();
			Created = DateTime.UtcNow;
			Modified = DateTime.UtcNow;

			Model = CreateModel();
		}

		private static IModel CreateModel()
		{
			//	TODO: Create from configuration or load from some text file or... anything else

			var model = new Model
				{
					Teams = new Dictionary<string, Team>
						{
							//	Look at System.Windows.Media.KnownColor for uint values of common colours
							{
								"Sothbury Wanderers FC",
								new Team
									{
										Balance = 10032412d,
										PrimaryColourInt = 4286611584U,
										SecondaryColourInt = 4294956800U,
										TeamName = "Sothbury Wanderers FC"
									}
							},
							{
								"Bicester Royals FC",
								new Team
									{
										Balance = 12734794d,
										PrimaryColourInt = 4287245282U,
										SecondaryColourInt = 4278239231U,
										TeamName = "Bicester Royals FC"
									}
							},
							{
								"Caddington City FC",
								new Team
									{
										Balance = 43462412d,
										PrimaryColourInt = 4284456608U,
										SecondaryColourInt = 4278255615U,
										TeamName = "Caddington City FC"
									}
							},
							{
								"Uthmalton Town FC",
								new Team
									{
										Balance = 1439622d,
										PrimaryColourInt = 4294907027U,
										SecondaryColourInt = 4278190080U,
										TeamName = "Uthmalton Town FC"
									}
							},
						}
				};

			model.Players = new List<Player>
				{
					new Player { Age = 21, ReleaseValue = 40000000, NumericValue = 23000000, FirstName = "John", LastName = "McMasterson", Rating = 92.4, Number = 9, Positions = new List<Position> { Position.CentreBack }, Team = model.Teams["Sothbury Wanderers FC"], Location = new Coordinate { X = 0.13d, Y = 0.2d } },
					new Player { Age = 24, ReleaseValue = 4000000, NumericValue = 6000000, FirstName = "Ted", LastName = "Eddington", Rating = 60.3, Number = 3, Positions = new List<Position> { Position.CentreMid, Position.Roaming, Position.Forward }, Team = model.Teams["Sothbury Wanderers FC"], Location = new Coordinate { X = 0.5d, Y = 0.4d } },
					new Player { Age = 27, ReleaseValue = 15000000, NumericValue = 13000000, FirstName = "Bill", LastName = "Formica", Rating = 79.3, Number = 1, Positions = new List<Position> { Position.LeftWinger, Position.LeftWingback }, Team = model.Teams["Sothbury Wanderers FC"], Location = new Coordinate { X = 0.7d, Y = 0.7d } },
					new Player { Age = 22, ReleaseValue = 20000000, NumericValue = 19000000, FirstName = "Sam", LastName = "Cosmic", Rating = 83.5, Number = 10, Positions = new List<Position> { Position.RightWinger, Position.RightFullBack }, Team = model.Teams["Bicester Royals FC"], Location = new Coordinate { X = 0.2d, Y = 0.3d } },
					new Player { Age = 28, ReleaseValue = 2000000, NumericValue = 3000000, FirstName = "Tarquin", LastName = "Frederick", Rating = 41.2, Number = 8, Positions = new List<Position> { Position.Striker }, Team = model.Teams["Bicester Royals FC"], Location = new Coordinate { X = 0.8d, Y = 0.3d } },
					new Player { Age = 27, ReleaseValue = 750000, NumericValue = 1000000, FirstName = "Philip", LastName = "Thomas", Rating = 28.5, Number = 2, Positions = new List<Position> { Position.RightFullBack, Position.LeftFullBack }, Team = model.Teams["Bicester Royals FC"], Location = new Coordinate { X = 0.5, Y = 0.6d } },
					new Player { Age = 24, ReleaseValue = 2000000, NumericValue = 2500000, FirstName = "Elliot", LastName = "Cloud", Rating = 55.7, Number = 23, Positions = new List<Position> { Position.Roaming }, Team = model.Teams["Caddington City FC"], Location = new Coordinate { X = 0.13d, Y = 0.2d } },
					new Player { Age = 20, ReleaseValue = 5000000, NumericValue = 4500000, FirstName = "Bob", LastName = "Spire", Rating = 66.4, Number = 4, Positions = new List<Position> { Position.GoalKeeper }, Team = model.Teams["Caddington City FC"], Location = new Coordinate { X = 0.34d, Y = 0.4d } },
					new Player { Age = 33, ReleaseValue = 500000, NumericValue = 850000, FirstName = "Terrence", LastName = "Nottingham", Rating = 26.5, Number = 1, Positions = new List<Position> { Position.Forward, Position.Striker }, Team = model.Teams["Caddington City FC"], Location = new Coordinate { X = 0.54d, Y = 0.7d } },
					new Player { Age = 36, ReleaseValue = 15000000, NumericValue = 11000000, FirstName = "Bastion", LastName = "Rockton", Rating = 86.9, Number = 5, Positions = new List<Position> { Position.CentreMid }, Team = model.Teams["Uthmalton Town FC"], Location = new Coordinate { X = 0.2d, Y = 0.4d } },
					new Player { Age = 19, ReleaseValue = 3000000, NumericValue = 2000000, FirstName = "Huppert", LastName = "Strafer", Rating = 47.7, Number = 6, Positions = new List<Position> { Position.CentreBack }, Team = model.Teams["Uthmalton Town FC"], Location = new Coordinate { X = 0.7d, Y = 0.5d } },
					new Player { Age = 17, ReleaseValue = 3000000, NumericValue = 2500000, FirstName = "Fergus", LastName = "Mystic", Rating = 56.3, Number = 2, Positions = new List<Position> { Position.LeftWingback }, Team = model.Teams["Uthmalton Town FC"], Location = new Coordinate { X = 0.7d, Y = 0.75d } },
				};

			model.Cmcl = new Division
			{
				CompetitionName = "Cm93 Competition League",
				Week = 0,
				Teams = model.Teams
			};

			model.CmclFixtures = new List<Fixture>
				{
					new Fixture { TeamHome = model.Teams["Sothbury Wanderers FC"], TeamAway = model.Teams["Bicester Royals FC"], Week = 1, Competition = model.Cmcl },
					new Fixture { TeamHome = model.Teams["Caddington City FC"], TeamAway = model.Teams["Uthmalton Town FC"], Week = 1, Competition = model.Cmcl },
					new Fixture { TeamHome = model.Teams["Sothbury Wanderers FC"], TeamAway = model.Teams["Caddington City FC"], Week = 2, Competition = model.Cmcl },
					new Fixture { TeamHome = model.Teams["Uthmalton Town FC"], TeamAway = model.Teams["Bicester Royals FC"], Week = 2, Competition = model.Cmcl },
					new Fixture { TeamHome = model.Teams["Bicester Royals FC"], TeamAway = model.Teams["Caddington City FC"], Week = 3, Competition = model.Cmcl },
					new Fixture { TeamHome = model.Teams["Uthmalton Town FC"], TeamAway = model.Teams["Sothbury Wanderers FC"], Week = 3, Competition = model.Cmcl },
				};

			model.CmclPlaces = new Dictionary<Team, Place>
				{
					{ model.Teams["Sothbury Wanderers FC"], new Place { Team = model.Teams["Sothbury Wanderers FC"] } },
					{ model.Teams["Bicester Royals FC"], new Place { Team = model.Teams["Bicester Royals FC"] } },
					{ model.Teams["Caddington City FC"], new Place { Team = model.Teams["Caddington City FC"] } },
					{ model.Teams["Uthmalton Town FC"], new Place { Team = model.Teams["Uthmalton Town FC"] } }
				};

			return model;
		}
	}
}
