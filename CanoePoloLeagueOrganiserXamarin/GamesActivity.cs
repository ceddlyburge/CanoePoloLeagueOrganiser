using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CanoePoloLeagueOrganiser;

namespace CanoePoloLeagueOrganiserXamarin
{
    [Activity(Label = "Games", MainLauncher = true, Icon = "@drawable/icon")]
    public class GamesActivity : Activity
    {
        ListView GameList;
        GameListAdapter GameListAdapter;
        Button OptimiseButton;
        Button AddButton;
        AutoCompleteTextView HomeTeamEntry;
        AutoCompleteTextView AwayTeamEntry;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            var games = new List<Game> {
                new Game("castle", "battersea"),
                new Game("castle", "ulu"),
                new Game("castle", "braintree"),
                new Game("castle", "avon"),
                new Game("castle", "blackwater"),
                new Game("castle", "letchworth"),
                new Game("castle", "ulu"),
                new Game("braintree", "avon"),
                new Game("blackwater", "letchworth"),
            };
            SetContentView(Resource.Layout.Games);
            GameList = FindViewById<ListView>(Resource.Id.Games);
            GameListAdapter = new GameListAdapter(games, this);
            GameList.Adapter = GameListAdapter;

            OptimiseButton = FindViewById<Button>(Resource.Id.Optimise);
            OptimiseButton.Click += Optimise;

            HomeTeamEntry = FindViewById<AutoCompleteTextView>(Resource.Id.HomeTeam);
            AwayTeamEntry = FindViewById<AutoCompleteTextView>(Resource.Id.AwayTeam);
            AddButton = FindViewById<Button>(Resource.Id.Add);
            AddButton.Click += AddGame;
        }

        private void AddGame(object sender, EventArgs e)
        {
            GameListAdapter.AddGame(homeTeam: HomeTeamEntry.Text, awayTeam: AwayTeamEntry.Text);
        }

        private void Optimise(object sender, EventArgs e)
        {
            var gameOrder = new TournamentDayCalculator(GameListAdapter.Games, new TenSecondPragmatiser()).CalculateGameOrder();

            GameListAdapter.SetGames(gameOrder.OptimisedGameOrder.GameOrder);
        }
    }
}