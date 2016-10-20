using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
    [Activity(Label = "Canoe Polo Organisr", MainLauncher = true, Icon = "@drawable/canoe_polo_ball")]
    public class GamesActivity : Activity
    {
        GameListAdapter GameListAdapter;
        Android.Widget.Button OptimiseButton;
        AutoCompleteTextView HomeTeamEntry;
        AutoCompleteTextView AwayTeamEntry;
        TextView Help;
        IOptimalGameOrder GameOrderCalculator;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var games = (savedInstanceState == null)
                ? new List<Game>()
                : new GamesSerialiser().DeSerialise(savedInstanceState.GetString("games", "[]"));

            SetContentView(Resource.Layout.Games);

            OptimiseButton = FindViewById<Android.Widget.Button>(Resource.Id.Optimise);
            OptimiseButton.Click += Optimise;

            Help = FindViewById<TextView>(Resource.Id.Help);
            Help.Text = "Add games, then click to optimise";

            HomeTeamEntry = FindViewById<AutoCompleteTextView>(Resource.Id.HomeTeam);
            AwayTeamEntry = FindViewById<AutoCompleteTextView>(Resource.Id.AwayTeam);
            var addButton = FindViewById<Android.Widget.Button>(Resource.Id.Add);
            addButton.Click += AddGame;

            // I might be able to ioc this if I move to xamarin forms
            GameOrderCalculator = new OptimalGameOrder(new TenSecondPragmatiser());
            var gameList = FindViewById<Android.Widget.ListView>(Resource.Id.Games);
            GameListAdapter = new GameListAdapter(games, this, GameOrderCalculator);
            gameList.Adapter = GameListAdapter;
            // I think this would work with the x platform xamarin forms, but i am using android specific stuff at the moment
            //var deleteAction = new MenuItem { Text = "Delete", IsDestructive = true };
            // deleteAction.SetBinding (MenuItem.CommandParameterProperty, new Binding (.));
            // deleteAction.Clicked + async (sender, e) => {
            //  var mi = ((MenuItem) sender);
            //  Debug.WriteLine($"{mi.CommandParameter}");
            //};
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            // ioc this later
            outState.PutString("games", new GamesSerialiser().Serialise(GameListAdapter.Games)); 
        }

        internal void Update(string optimisationExplanation)
        {
            if (Help != null)
                Help.Text = optimisationExplanation;

            if (OptimiseButton != null && GameListAdapter != null)
                    OptimiseButton.Enabled = (GameListAdapter.Games.Count > 2);
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
            var gameOrder = GameOrderCalculator.OptimiseGameOrder(GameListAdapter.Games);

            // the game list adapter will callback the update method with this optimisation method, not sure if this is the best way of doing it.
            // I am doing some logic about what to show to the user and what not to in the untested ui here. Not that good. Do something better when I move to generic xamarin project.
            GameListAdapter.SetGames(gameOrder.OptimisedGameOrder.GameOrder, (gameOrder.PragmatisationLevel == PragmatisationLevel.OutOfTime)? gameOrder.OptimisationMessage : "");
        }
    }
}