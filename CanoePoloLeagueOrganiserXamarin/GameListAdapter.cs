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

namespace CanoePoloLeagueOrganiserXamarin
{
    internal class JavaGame : Java.Lang.Object
    {
        public Game Game;
    }

    public class GameListAdapter : BaseAdapter<Game>
    {
        readonly List<Game> games;
        readonly Activity context;

        public GameListAdapter(List<Game> games, Activity context)
        {
            Contract.Requires(games != null);
            Contract.Requires(context != null);

            this.games = games;// new List<Game>();
            this.context = context;
        }

        public List<Game> Games => this.games;

        public override Game this[int position] => this.games[position];

        public override int Count => this.games.Count;

        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var game = this.games[position];

            // not reusing views here. This list won't be long and it is tricky to remove the old click event handler
            var view = convertView ?? context.LayoutInflater.Inflate(Resource.Layout.GameRow, null);
            if (convertView == null)
                view.FindViewById<Button>(Resource.Id.Remove).Click += (s, e) => DeleteGame((((s as Button).Tag) as JavaGame).Game);

            view.FindViewById<TextView>(Resource.Id.HomeTeam).Text = game.HomeTeam.Name;
            view.FindViewById<TextView>(Resource.Id.AwayTeam).Text = game.AwayTeam.Name;
            view.FindViewById<Button>(Resource.Id.Remove).Tag = new JavaGame { Game = game };
            

            return view;
        }

        private void DeleteGame(Game game)
        {
            this.games.Remove(game);
            NotifyDataSetChanged();
        }

        internal void SetGames(IEnumerable<Game> games)
        {
            this.games.Clear();
            this.games.AddRange(games);
            NotifyDataSetChanged();
        }

        internal void AddGame(string homeTeam, string awayTeam)
        {
            this.games.Add(new Game(new Team(homeTeam), new Team(awayTeam)));
            NotifyDataSetChanged();
        }
    }
}