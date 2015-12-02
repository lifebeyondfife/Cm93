Cm93
====

Simple football manager app in the vein of the early Championship Manager games.

![Intro](https://raw.githubusercontent.com/lifebeyondfife/Cm93/master/screenshots/cm93%20intro.png)
![Fixtures](https://raw.githubusercontent.com/lifebeyondfife/Cm93/master/screenshots/cm93%20fixtures.png)
![Team](https://raw.githubusercontent.com/lifebeyondfife/Cm93/master/screenshots/cm93%20team.png)
![Players](https://raw.githubusercontent.com/lifebeyondfife/Cm93/master/screenshots/cm93%20players.png)
![League](https://raw.githubusercontent.com/lifebeyondfife/Cm93/master/screenshots/cm93%20league.png)
![Match](https://raw.githubusercontent.com/lifebeyondfife/Cm93/master/screenshots/cm93%20match.png)

Plan
----

Cm93 isn't quite playable (well, enjoyable) at present, but a fairly well thrashed out beginning. Enough to get on GitHub certainly. The remainder of this README.md details what I'd like to have done before I can consider it a launchable game. Mainly adding functionality at the moment but in the long term would look to work with a WPF Blend expert to make something that looks nice too.

Non-developers can download an installer from the Releases link above.

* ~~Redo UI to side level menu bar buttons instead of quadrants~~

* ~~Create a player module for looking at detailed stats (team filter like the results page).~~
	- ~~Also allows bids for players (for first 4 weeks of season and midpoint)~~

* ~~Match module screen~~
	- ~~Show both team formations~~
	- ~~Visualise game simulation (nothing too spectacular)~~
	- ~~Allow for half-time formation change~~
	- ~~Allow three substitutions~~

* ~~Persist state of game (immediate and continuous auto-save)~~
	- ~~Make all the game model structures serialisable~~
	- ~~Read/write from an SQLite instance~~
	- ~~Add ability to have multiple save games~~

* ~~Playable datasets (more teams, 11-a-side formations)~~
	- ~~Create large number of teams and players~~
	- ~~Automate construction of fixtures~~
	- ~~Add non-league competitions (league cup, xfa cup, euro cups)~~ _future version_
	- ~~Add multiple divisions~~ _future version_

- AI
	- Create some stock formations (4-4-2, 3-5-2, 4-3-3, 4-2-3-1 etc.)
	- Algorithm for varying NPC teams' formations slightly
	- Algorithm for finding "best" starting lineup-formations
	- Algorithm for responsive substitutions / formation changes
	- Complex game simultator implementation

- Finishing touches
	- Highlight player's team in league table
	- Add concept of Manager name
	- Show extra info on team selection screen (shirt, ground name + capacity, budget)
	- Nicer shirt icons and pitch graphic
