using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using CanoePoloLeagueOrganiser;
using System.Linq;

namespace CanoePoloLeagueOrganiserXamarin
{
    [Activity(Label = "CanoePoloLeagueOrganiserXamarin",  Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private EditText GameOrderControl;
        private MultiAutoCompleteTextView MatchesControl;
        private Button OptimiseGameOrderControl;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            MatchesControl = FindViewById<MultiAutoCompleteTextView>(Resource.Id.Matches);
            MatchesControl.Text = 
@"Castle v Battersea
Castle v ULU
Castle v Avon
Castle v Braintree
Castle v Blackwater
Battersea v ULU
Avon v Braintree
Blackwater v Battersea";

            OptimiseGameOrderControl = FindViewById<Button>(Resource.Id.OptimiseGameOrder);
            OptimiseGameOrderControl.Click += OptimiseGameOrder;

            GameOrderControl = FindViewById<EditText>(Resource.Id.GameOrder);

        }

        private void OptimiseGameOrder(object sender, EventArgs e)
        {
            var teams = new Dictionary<string, Team>();
            var games = new List<Game>();

            foreach (var game in MatchesControl.Text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                var teamsInGame = game.Split(new string[] { " v " }, StringSplitOptions.RemoveEmptyEntries);
                var team1Name = teamsInGame[0].Trim();
                var team2Name = teamsInGame[1].Trim();

                if (teams.ContainsKey(team1Name) == false) teams.Add(team1Name, new Team(team1Name));
                if (teams.ContainsKey(team2Name) == false) teams.Add(team2Name, new Team(team2Name));

                games.Add(new Game(teams[team1Name], teams[team2Name]));
            }

            var gameOrder = new TournamentDayCalculator(games, new TenSecondPragmatiser()).CalculateGameOrder().OptimisedGameOrder;

            var text = 
$@"Max consecutive matches by any team: {gameOrder.MaxConsecutiveMatchesByAnyTeam}
Occurences of teams playing in consecutive matches: {gameOrder.OccurencesOfTeamsPlayingConsecutiveMatches}
Total number of games spent waiting around by all teams: {gameOrder.GamesNotPlayedBetweenFirstAndLast}

";
            GameOrderControl.Text = text + gameOrder.GameOrder.Select(g => $"{g.HomeTeam.Name} v {g.AwayTeam.Name}\n").Aggregate("", (s1, s2) => s1 + s2);
        }
    }
}

