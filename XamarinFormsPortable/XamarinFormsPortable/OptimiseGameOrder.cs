using CanoePoloLeagueOrganiser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamarinFormsPortable
{
    class OptimiseGameOrder : ContentPage
    {
        public OptimiseGameOrder()
        {
            var games = new List<Game> {
                new Game("Clapham", "Surrey"),
                new Game("Clapham", "ULU"),
                new Game("Clapham", "Meridian"),
                new Game("Blackwater", "Clapham"),
                new Game("ULU", "Blackwater"),
                new Game("Surrey", "Castle"),
                new Game("ULU", "Meridian"),
                new Game("Letchworth", "ULU"),
                new Game("Castle", "Blackwater"),
                new Game("Surrey", "Letchworth"),
                new Game("Meridian", "Castle"),
                new Game("Blackwater", "Letchworth"),
                new Game("Meridian", "Surrey"),
                new Game("Castle", "Letchworth")
             };

            var content = new StackLayout();
            //var content = new StackLayout
            //{ 
            //    Children = games.Select(g => new Label { Text = g.HomeTeam.Name + " v " + g.AwayTeam.Name })
            //};

            foreach (var game in games)
                content.Children.Add(GameRow(game));

            Content = content;

        }

        View GameRow(Game game)
        {
            var row = new Grid();

            row.ColumnDefinitions = new ColumnDefinitionCollection {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) } // home team
                ,new ColumnDefinition { Width = new GridLength(0, GridUnitType.Auto) } // " v "
                ,new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) } // away team
                ,new ColumnDefinition { Width = new GridLength(0, GridUnitType.Auto) } // up
                ,new ColumnDefinition { Width = new GridLength(0, GridUnitType.Auto) } // down
                ,new ColumnDefinition { Width = new GridLength(0, GridUnitType.Auto) } // delete
            };

            row.Children.Add(new Label { Text = game.HomeTeam.Name, HorizontalOptions = LayoutOptions. EndAndExpand }, 0, 0);
            row.Children.Add(new Label { Text = " v " }, 1, 0);
            row.Children.Add(new Label { Text = game.AwayTeam.Name, HorizontalOptions = LayoutOptions.StartAndExpand }, 2, 0);
            row.Children.Add(new Button {MinimumWidthRequest = 0, WidthRequest = 10, Text = "Up", HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center }, 3, 0);
            row.Children.Add(new Button { Text = "Down", HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Start}, 4, 0);
            row.Children.Add(new Button { Text = "Remove", HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill }, 5, 0);

            return row;
        }
    }
}