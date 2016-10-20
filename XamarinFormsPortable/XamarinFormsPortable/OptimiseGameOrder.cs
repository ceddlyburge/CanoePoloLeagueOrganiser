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

            Content = new StackLayoutCedd
            {
                AddSomeChildren = new List<View>{
                    OptimiseButton(),
                    AddNewGame(),
                    Games(games)
                }
            };
        }

        View AddNewGame()
        {
            return new GridCedd()
            {
                ColumnDefinitions = new ColumnDefinitionCollection {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) } // home team
                    ,new ColumnDefinition { Width = new GridLength(0, GridUnitType.Auto) } // " v "
                    ,new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) } // away team
                    ,new ColumnDefinition { Width = new GridLength(0, GridUnitType.Auto) } // Add
                },
                AddSomeRow = new List<View> {
                    new Editor { HorizontalOptions = LayoutOptions.EndAndExpand },
                    new Label { Text = " v " },
                    new Editor { HorizontalOptions = LayoutOptions.StartAndExpand },
                    new Button { MinimumWidthRequest = 0, Text = "Add", HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center }
                }
            };
        }

        static View OptimiseButton()
        {
            return new Button { Text = "Optimise" };
        }

        View Games(List<Game> games)
        {
            return new GridCedd()
            {
                ColumnDefinitions = new ColumnDefinitionCollection {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) } // home team
                    ,new ColumnDefinition { Width = new GridLength(0, GridUnitType.Auto) } // " v "
                    ,new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) } // away team
                    ,new ColumnDefinition { Width = new GridLength(0, GridUnitType.Auto) } // up
                    ,new ColumnDefinition { Width = new GridLength(0, GridUnitType.Auto) } // down
                    ,new ColumnDefinition { Width = new GridLength(0, GridUnitType.Auto) } // delete
                    
                },
                AddSomeRows = games.Select(g => GameRow(g))
            };
        }

        IEnumerable<View> GameRow(Game game)
        {
            return new List<View> {
             new Label { Text = game.HomeTeam.Name, HorizontalOptions = LayoutOptions. EndAndExpand },
             new Label { Text = " v " },
             new Label { Text = game.AwayTeam.Name, HorizontalOptions = LayoutOptions.StartAndExpand },
             new Button {MinimumWidthRequest = 0, WidthRequest = 10, Text = "Up", HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center },
             new Button { Text = "Down", HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Start},
             new Button { Text = "Remove", HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill }
            };
        }
    }
}