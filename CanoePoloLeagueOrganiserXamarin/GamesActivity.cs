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
using Newtonsoft.Json;

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
        TextView OptimisationExplanation;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // ioc this later
            var games = (savedInstanceState == null)
                ? new List<Game>()
                : new GamesSerialiser().DeSerialise(savedInstanceState.GetString("games", "[]"));

            SetContentView(Resource.Layout.Games);

            OptimiseButton = FindViewById<Button>(Resource.Id.Optimise);
            OptimiseButton.Click += Optimise;

            OptimisationExplanation = FindViewById<TextView>(Resource.Id.OptimisationExplanation);

            HomeTeamEntry = FindViewById<AutoCompleteTextView>(Resource.Id.HomeTeam);
            AwayTeamEntry = FindViewById<AutoCompleteTextView>(Resource.Id.AwayTeam);
            AddButton = FindViewById<Button>(Resource.Id.Add);
            AddButton.Click += AddGame;

            GameList = FindViewById<ListView>(Resource.Id.Games);
            GameListAdapter = new GameListAdapter(games, this);
            GameList.Adapter = GameListAdapter;
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            // ioc this later
            outState.PutString("games", new GamesSerialiser().Serialise(GameListAdapter.Games)); 
        }

        internal void Update(string optimisationExplanation)
        {
            if (OptimisationExplanation != null)
                OptimisationExplanation.Text = optimisationExplanation;

            if (OptimiseButton != null && this.GameListAdapter != null)
                    OptimiseButton.Enabled = (this.GameListAdapter.Games.Count > 2);
        }

        void AddGame(object sender, EventArgs e)
        {
            GameListAdapter.AddGame(homeTeam: HomeTeamEntry.Text, awayTeam: AwayTeamEntry.Text);

            HomeTeamEntry.Text = "";
            AwayTeamEntry.Text = "";
            HomeTeamEntry.RequestFocus();
        }

        void Optimise(object sender, EventArgs e)
        {
            var gameOrder = new TournamentDayCalculator(GameListAdapter.Games, new TenSecondPragmatiser()).CalculateGameOrder();

            // the game list adapter will callback the update method with this optimisation method, not sure if this is the best way of doing it.
            GameListAdapter.SetGames(gameOrder.OptimisedGameOrder.GameOrder, gameOrder.OptimisationMessage);
        }
    }
}