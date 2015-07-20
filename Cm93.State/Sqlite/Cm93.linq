<Query Kind="Statements">
  <Connection>
    <ID>0241f0b7-4911-4c92-b776-0b9622a6faf4</ID>
    <Persist>true</Persist>
    <Driver Assembly="IQDriver" PublicKeyToken="5b59726538a49684">IQDriver.IQDriver</Driver>
    <Provider>System.Data.SQLite</Provider>
    <CustomCxString>Data Source=D:\dev\Cm93\Cm93.State\Sqlite\Cm93_State_Empty.sqlite;FailIfMissing=True</CustomCxString>
    <AttachFileName>D:\dev\Cm93\Cm93.State\Sqlite\Cm93_State_Empty.sqlite</AttachFileName>
    <DriverData>
      <StripUnderscores>false</StripUnderscores>
      <QuietenAllCaps>false</QuietenAllCaps>
      <MapIntToLong>true</MapIntToLong>
    </DriverData>
  </Connection>
</Query>

//var teams = new [] {
//	new { TeamId = 0, TeamName = "Sothbury Wanderers FC", PrimaryColour = 4286611584, SecondaryColour = 4294956800, Balance = 10032412 },
//	new { TeamId = 1, TeamName = "Bicester Royals FC", PrimaryColour = 4287245282, SecondaryColour = 4278239231, Balance = 12734794 },
//	new { TeamId = 2, TeamName = "Caddington City FC", PrimaryColour = 4284456608, SecondaryColour = 4278255615, Balance = 43462412 },
//	new { TeamId = 3, TeamName = "Uthmalton Town FC", PrimaryColour = 4294907027, SecondaryColour = 4278190080, Balance = 1439622 },
//};

//foreach (var team in teams)
//{
//	Teams.InsertOnSubmit(new Teams
//		{
//			TeamId = team.TeamId,
//			TeamName = team.TeamName,
//			PrimaryColour = team.PrimaryColour,
//			SecondaryColour = team.SecondaryColour
//		});

//	TeamBalances.InsertOnSubmit(new TeamBalances
//		{
//			TeamId = team.TeamId,
//			StateId = 0,
//			Balance = team.Balance
//		});
//}

//Teams.Session.SubmitChanges();
//TeamBalances.Session.SubmitChanges();

//Teams.Dump();
//TeamBalances.Dump();

//var teamLookup = Teams.ToDictionary(t => t.TeamName);

//var players = new [] {
//	new { Age = 21, ReleaseValue = 40000000, NumericValue = 23000000, FirstName = "John", LastName = "McMasterson", Rating = 92.4, Number = 9, Position = 0x00100, TeamId = teamLookup["Sothbury Wanderers FC"].TeamId, LocationX = 0.13d, LocationY = 0.2d },
//	new { Age = 24, ReleaseValue = 4000000, NumericValue = 6000000, FirstName = "Ted", LastName = "Eddington", Rating = 60.3, Number = 3, Position = 0x11000, TeamId = teamLookup["Sothbury Wanderers FC"].TeamId, LocationX = 0.5d, LocationY = 0.4d },
//	new { Age = 27, ReleaseValue = 15000000, NumericValue = 13000000, FirstName = "Bill", LastName = "Formica", Rating = 79.3, Number = 1, Position = 0x01110, TeamId = teamLookup["Sothbury Wanderers FC"].TeamId, LocationX = 0.7d, LocationY = 0.7d },
//	new { Age = 22, ReleaseValue = 20000000, NumericValue = 19000000, FirstName = "Sam", LastName = "Cosmic", Rating = 83.5, Number = 10, Position = 0x01101, TeamId = teamLookup["Bicester Royals FC"].TeamId, LocationX = 0.2d, LocationY = 0.3d },
//	new { Age = 28, ReleaseValue = 2000000, NumericValue = 3000000, FirstName = "Tarquin", LastName = "Frederick", Rating = 41.2, Number = 8, Position = 0x10000, TeamId = teamLookup["Bicester Royals FC"].TeamId, LocationX = 0.8d, LocationY = 0.3d },
//	new { Age = 27, ReleaseValue = 750000, NumericValue = 1000000, FirstName = "Philip", LastName = "Thomas", Rating = 28.5, Number = 2, Position = 0x00111, TeamId = teamLookup["Bicester Royals FC"].TeamId, LocationX = 0.5, LocationY = 0.6d },
//	new { Age = 24, ReleaseValue = 2000000, NumericValue = 2500000, FirstName = "Elliot", LastName = "Cloud", Rating = 55.7, Number = 23, Position = 0x11011, TeamId = teamLookup["Caddington City FC"].TeamId, LocationX = 0.13d, LocationY = 0.2d },
//	new { Age = 20, ReleaseValue = 5000000, NumericValue = 4500000, FirstName = "Bob", LastName = "Spire", Rating = 66.4, Number = 4, Position = 0x00000, TeamId = teamLookup["Caddington City FC"].TeamId, LocationX = 0.34d, LocationY = 0.4d },
//	new { Age = 33, ReleaseValue = 500000, NumericValue = 850000, FirstName = "Terrence", LastName = "Nottingham", Rating = 26.5, Number = 1, Position = 0x10011, TeamId = teamLookup["Caddington City FC"].TeamId, LocationX = 0.54d, LocationY = 0.7d },
//	new { Age = 36, ReleaseValue = 15000000, NumericValue = 11000000, FirstName = "Bastion", LastName = "Rockton", Rating = 86.9, Number = 5, Position = 0x01000, TeamId = teamLookup["Uthmalton Town FC"].TeamId, LocationX = 0.2d, LocationY = 0.4d },
//	new { Age = 19, ReleaseValue = 3000000, NumericValue = 2000000, FirstName = "Huppert", LastName = "Strafer", Rating = 47.7, Number = 6, Position = 0x00100, TeamId = teamLookup["Uthmalton Town FC"].TeamId, LocationX = 0.7d, LocationY = 0.5d },
//	new { Age = 17, ReleaseValue = 3000000, NumericValue = 2500000, FirstName = "Fergus", LastName = "Mystic", Rating = 56.3, Number = 2, Position = 0x00110, TeamId = teamLookup["Uthmalton Town FC"].TeamId, LocationX = 0.7d, LocationY = 0.75d },
//};

//foreach (var player in Enumerable.Range(0, players.Length).Zip(players, (a, b) => new { PlayerId = a, Player = b}))
//{
//	Players.InsertOnSubmit(new Players
//		{
//			PlayerId = player.PlayerId,
//			StateId = 0,
//			LocationX = (float) player.Player.LocationX,
//			LocationY = (float) player.Player.LocationY,
//			Number = player.Player.Number,
//			NumericValue = player.Player.NumericValue,
//			ReleaseValue = player.Player.ReleaseValue,
//			TeamId = player.Player.TeamId
//		});

//	PlayerStats.InsertOnSubmit(new PlayerStats
//		{
//			PlayerId = player.PlayerId,
//			Age = player.Player.Age,
//			FirstName = player.Player.FirstName,
//			LastName = player.Player.LastName,
//			Position = player.Player.Position,
//		});
//}

//Players.Session.SubmitChanges();
//PlayerStats.Session.SubmitChanges();

//Players.Dump();
//PlayerStats.Dump();

//Competitions.InsertOnSubmit(new Competitions { CompetitionId = 0, CompetitionName = "Cm93 Competition League", CompetitionType = "League" });
//Competitions.Session.SubmitChanges();

//Competitions.Dump();

//var fixtures = new []
//	{
//		new Fixtures { HomeTeamId = teamLookup["Sothbury Wanderers FC"].TeamId, AwayTeamId = teamLookup["Bicester Royals FC"].TeamId, Week = 1, CompetitionId = 0, StateId = 0 },
//		new Fixtures { HomeTeamId = teamLookup["Caddington City FC"].TeamId, AwayTeamId = teamLookup["Uthmalton Town FC"].TeamId, Week = 1, CompetitionId = 0, StateId = 0 },
//		new Fixtures { HomeTeamId = teamLookup["Sothbury Wanderers FC"].TeamId, AwayTeamId = teamLookup["Caddington City FC"].TeamId, Week = 2, CompetitionId = 0, StateId = 0 },
//		new Fixtures { HomeTeamId = teamLookup["Uthmalton Town FC"].TeamId, AwayTeamId = teamLookup["Bicester Royals FC"].TeamId, Week = 2, CompetitionId = 0, StateId = 0 },
//		new Fixtures { HomeTeamId = teamLookup["Bicester Royals FC"].TeamId, AwayTeamId = teamLookup["Caddington City FC"].TeamId, Week = 3, CompetitionId = 0, StateId = 0 },
//		new Fixtures { HomeTeamId = teamLookup["Uthmalton Town FC"].TeamId, AwayTeamId = teamLookup["Sothbury Wanderers FC"].TeamId, Week = 3, CompetitionId = 0, StateId = 0 },
//	};

//foreach (var fixture in fixtures)
//	Fixtures.InsertOnSubmit(fixture);

//Fixtures.Session.SubmitChanges();

//Fixtures.Dump();

//foreach (var player in Enumerable.Range(0, players.Length).Zip(players, (a, b) => new { PlayerId = a, Player = b}))
//	Ratings.InsertOnSubmit(new Ratings { PlayerId = player.PlayerId, Rating = (float) player.Player.Rating });

//Ratings.Session.SubmitChanges();

//Ratings.Dump();

//foreach (var team in Teams)
//	Divisions.InsertOnSubmit(new Divisions
//		{
//			CompetitionId = 0,
//			Draws = 0,
//			GoalDifference = 0,
//			GoalsAgainst = 0,
//			GoalsFor = 0,
//			Losses = 0,
//			Points = 0,
//			TeamId = team.TeamId,
//			StateId = 0,
//			Wins = 0
//		});

//Divisions.Session.SubmitChanges();

//Divisions.Dump();