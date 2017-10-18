using CanoePoloLeagueOrganiser;
using Syncfusion.ListView.XForms;
using Syncfusion.SfDataGrid.XForms;
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
                BackgroundColor = Color.Fuchsia,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                AddSomeChildren = new List<View>{
                    new Button { Text = "Center", HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, MinimumWidthRequest = 1 },
                    new Button { Text = "Start", HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, MinimumWidthRequest = 1 },
                    new Button { Text = "End", HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center, MinimumWidthRequest = 1 },
                    OptimiseButton(),
                    AddNewGame(),
                    Games(games)
                }
            };

            var listView = new SfDataGrid();
            listView.ItemsSource = games;
            Content = listView;

            //Content = Games(games);

            //Content = new Button { Text = "Up", HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center,  MinimumWidthRequest = 0, MinimumHeightRequest = 0 };
        }

        static View OptimiseButton()
        {
            return new Button { Text = "Optimise" };
        }

        View AddNewGame()
        {
            return new GridCedd()
            {
                ColumnDefinitions = new ColumnDefinitionCollection {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) } // home team
                    ,new ColumnDefinition { Width = GridLength.Auto } // " v "
                    ,new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) } // away team
                    ,new ColumnDefinition { Width = GridLength.Auto } // Add
                },
                AddSomeRow = new List<View> {
                    new Editor { HorizontalOptions = LayoutOptions.EndAndExpand },
                    new Label { Text = " v " },
                    new Editor { HorizontalOptions = LayoutOptions.StartAndExpand },
                    new Button { MinimumWidthRequest = 0, Text = "Add", HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center }
                }
            };
        }

        View Games(List<Game> games)
        {
            //var grid = new Grid
            //{
            //    BackgroundColor = Color.Aqua,
            //    ColumnDefinitions = new ColumnDefinitionCollection {
            //        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) } // home team
            //        ,new ColumnDefinition { Width = GridLength.Auto } // " v "
            //        ,new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) } // away team
            //        ,new ColumnDefinition { Width = GridLength.Auto } // up
            //        ,new ColumnDefinition { Width = GridLength.Auto } // down
            //        ,new ColumnDefinition { Width = GridLength.Auto } // delete
            //    }
            //    ,RowDefinitions = new RowDefinitionCollection
            //    {
            //        new RowDefinition { Height = GridLength.Auto }
            //    },
            //};

            //grid.Children.Add(new Label { Text = "clapham", BackgroundColor = Color.Blue }, 0, 0);
            //grid.Children.Add(new Label { Text = " v ", BackgroundColor = Color.Blue }, 1, 0);
            //grid.Children.Add(new Label { Text = "ulu", BackgroundColor = Color.Blue }, 2, 0);
            //grid.Children.Add(new Button { MinimumWidthRequest = 0, MinimumHeightRequest = 0, Text = "Up", BackgroundColor = Color.Blue, HorizontalOptions = LayoutOptions.Start }, 3, 0);
            //grid.Children.Add(new Button { MinimumWidthRequest = 0, MinimumHeightRequest = 0, Text = "Down", BackgroundColor = Color.Blue }, 4, 0);
            //grid.Children.Add(new Button { MinimumWidthRequest = 0, MinimumHeightRequest = 0, Text = "Remove", BackgroundColor = Color.Blue }, 5, 0);

            //return grid;
            return new GridCedd
            {
                BackgroundColor = Color.Aqua,
                ColumnDefinitions = new ColumnDefinitionCollection {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) } // home team
                    ,new ColumnDefinition { Width = GridLength.Auto } // " v "
                    ,new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) } // away team
                    ,new ColumnDefinition { Width = GridLength.Auto } // up
                    ,new ColumnDefinition { Width = GridLength.Auto } // down
                    ,new ColumnDefinition { Width = GridLength.Auto } // delete
                    
                },
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = GridLength.Auto }
                },
                AddSomeRow = games.Select(g => GameRow(g)).First()//.ToList(),
            };
        }

        IReadOnlyList<View> GameRow(Game game)
        {
            return new List<View> {
             new Label { Text = game.HomeTeam.Name, BackgroundColor = Color.Blue },
             new Label { Text = " v ", BackgroundColor = Color.Blue },
             new Label { Text = game.AwayTeam.Name, BackgroundColor = Color.Blue },
             new Button { MinimumWidthRequest = 0, Text = "Up", BackgroundColor = Color.Blue },
             new Button { MinimumWidthRequest = 0, Text = "Down", BackgroundColor = Color.Blue },
             new Button { MinimumWidthRequest = 0, Text = "Remove", BackgroundColor = Color.Blue }
            };
        }
    }
}