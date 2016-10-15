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
using System.Diagnostics.Contracts;
using Android.Graphics;

namespace CanoePoloLeagueOrganiserXamarin
{
    public class Dummy { }

    class JavaGame : Java.Lang.Object
    {
        public Game Game;
    }

    public class GameListAdapter : BaseAdapter<Game>
    {
        List<Game> GamesMutable { get; }
        GamesActivity Context { get; }
        IOptimalGameOrder GameOrderCalculator { get; }

        public GameListAdapter(List<Game> games, GamesActivity context, IOptimalGameOrder gameOrderCalculator)
        {
            Contract.Requires(gameOrderCalculator != null);
            Contract.Requires(games != null);
            Contract.Requires(context != null);

            GameOrderCalculator = gameOrderCalculator;
            Context = context;
            GamesMutable = new List<Game>();
            CalculateOriginalGameOrderAndSetGames(games);
        }

        public IReadOnlyList<Game> Games => GamesMutable;

        public override Game this[int position] => this.GamesMutable[position];

        public override int Count => this.GamesMutable.Count;

        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            Contract.Ensures(Contract.Result<View>() != null);

            var game = GamesMutable[position];

            var view = convertView ?? Context.LayoutInflater.Inflate(Resource.Layout.GameRow, null);
            if (convertView == null)
            {
                view.FindViewById<Button>(Resource.Id.Remove).Click += (s, e) => DeleteGame((((s as Button).Tag) as JavaGame).Game);
                view.FindViewById<Button>(Resource.Id.Up).Click += (s, e) => MoveGameUp((((s as Button).Tag) as JavaGame).Game);
                view.FindViewById<Button>(Resource.Id.Down).Click += (s, e) => MoveGameDown((((s as Button).Tag) as JavaGame).Game);
            }

            view.FindViewById<TextView>(Resource.Id.HomeTeam).Text = game.HomeTeam.Name;
            view.FindViewById<TextView>(Resource.Id.HomeTeam).SetTextColor(game.HomeTeamPlayingConsecutively ? Color.Red : Color.White);

            view.FindViewById<TextView>(Resource.Id.AwayTeam).Text = game.AwayTeam.Name;
            view.FindViewById<TextView>(Resource.Id.AwayTeam).SetTextColor(game.AwayTeamPlayingConsecutively ? Color.Red : Color.White);

            view.FindViewById<Button>(Resource.Id.Remove).Tag = new JavaGame { Game = game };
            view.FindViewById<Button>(Resource.Id.Up).Tag = new JavaGame { Game = game };
            view.FindViewById<Button>(Resource.Id.Down).Tag = new JavaGame { Game = game };

            view.FindViewById<Button>(Resource.Id.Up).Enabled = (position > 0);
            view.FindViewById<Button>(Resource.Id.Down).Enabled = (position < this.Games.Count - 1);

            return view;
        }

        void DeleteGame(Game game)
        {
            GamesMutable.Remove(game);
            CalculateOriginalGameOrderAndSetGames(this.GamesMutable);
        }

        void MoveGameUp(Game game)
        {
#pragma warning disable S2692 // "IndexOf" checks should not be for positive numbers
            Contract.Requires(GamesMutable.IndexOf(game) > 0);
#pragma warning restore S2692 // "IndexOf" checks should not be for positive numbers

            var index = GamesMutable.IndexOf(game);
            GamesMutable.Remove(game);
            GamesMutable.Insert(index - 1, game);
            CalculateOriginalGameOrderAndSetGames(GamesMutable);
        }

        void MoveGameDown(Game game)
        {
            Contract.Requires(GamesMutable.IndexOf(game) + 1 < GamesMutable.Count);

            var index = GamesMutable.IndexOf(game);
            GamesMutable.Remove(game);
            GamesMutable.Insert(index + 1, game);
            CalculateOriginalGameOrderAndSetGames(GamesMutable);
        }

        internal void AddGame(string homeTeam, string awayTeam)
        {
            GamesMutable.Add(new Game(homeTeam, awayTeam));
            CalculateOriginalGameOrderAndSetGames(GamesMutable);
        }

        internal void CalculateOriginalGameOrderAndSetGames(IReadOnlyList<Game> games)
        {
            SetGames(GameOrderCalculator.CalculateOriginalGameOrder(games).GameOrder, "");
        }

        internal void SetGames(IReadOnlyList<Game> newGames, string optimisationExplanation)
        {
            GamesMutable.Clear();
            GamesMutable.AddRange(newGames);

            NotifyDataSetChanged();
            Context.Update(optimisationExplanation);
        }
    }
}