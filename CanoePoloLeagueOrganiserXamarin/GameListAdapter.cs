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

    internal class JavaGame : Java.Lang.Object
    {
        public Game Game;
    }

    public class GameListAdapter : BaseAdapter<Game>
    {
        readonly List<Game> games;
        readonly GamesActivity context;
        readonly IOptimalGameOrder gameOrderCalculator;

        public GameListAdapter(List<Game> games, GamesActivity context, IOptimalGameOrder gameOrderCalculator)
        {
            Contract.Requires(gameOrderCalculator != null);
            Contract.Requires(games != null);
            Contract.Requires(context != null);

            this.gameOrderCalculator = gameOrderCalculator;
            this.context = context;
            this.games = new List<Game>();
            CalculateOriginalGameOrderAndSetGames(games);
        }

        public IReadOnlyList<Game> Games => this.games;

        public override Game this[int position] => this.games[position];

        public override int Count => this.games.Count;

        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            Contract.Ensures(Contract.Result<View>() != null);

            var game = this.games[position];

            var view = convertView ?? context.LayoutInflater.Inflate(Resource.Layout.GameRow, null);
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

        private void DeleteGame(Game game)
        {
            this.games.Remove(game);
            CalculateOriginalGameOrderAndSetGames(this.games);
        }

        private void MoveGameUp(Game game)
        {
            Contract.Requires(this.games.IndexOf(game) > 0);

            var index = this.games.IndexOf(game);
            this.games.Remove(game);
            this.games.Insert(index - 1, game);
            CalculateOriginalGameOrderAndSetGames(this.games);
        }

        private void MoveGameDown(Game game)
        {
            Contract.Requires(this.games.IndexOf(game) + 1 < this.games.Count);

            var index = this.games.IndexOf(game);
            this.games.Remove(game);
            this.games.Insert(index + 1, game);
            CalculateOriginalGameOrderAndSetGames(this.games);
        }

        internal void AddGame(string homeTeam, string awayTeam)
        {
            this.games.Add(new Game(homeTeam, awayTeam));
            CalculateOriginalGameOrderAndSetGames(this.games);
        }

        internal void CalculateOriginalGameOrderAndSetGames(IReadOnlyList<Game> games)
        {
            SetGames(gameOrderCalculator.CalculateOriginalGameOrder(games).GameOrder, "");
        }

        internal void SetGames(IReadOnlyList<Game> newGames, string optimisationExplanation)
        {
            this.games.Clear();
            this.games.AddRange(newGames);

            NotifyDataSetChanged();
            this.context.Update(optimisationExplanation);
        }
    }
}