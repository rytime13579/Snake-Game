using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BackgroundWorker bgWorker = new BackgroundWorker(); //loop runner moves the snake
        private int length = 1; //length of snake
        private char direction = 'd'; //direction the snake is moving (WASD)
  
        //Snake game window components
        private Window gameWindow; //name of window
        private Grid mainGrid; //grid that houses elements
        private Canvas mainGame; //canvas that has game
        private Canvas[] tail; //tail retangles
        private Canvas head; //head canvas
        private TextBox inputLine; //takes keyboard input
        private Label instructions; //instruction label
        private Canvas food;
        //List of location the tail follows
        private List<double[]> tailLoc;
        //List of X and Y boarder locations for food
        private int[] foodX = new int[11];
        private int[] foody = new int[11];
        //Speed of the game
        private int speed;
        public MainWindow()
        {

            InitializeComponent();
            initGameWindow();

            double[] headLocTemp = { Canvas.GetTop(this.head), Canvas.GetLeft(this.head)}; //finds location of the head
            tailLoc = new List<double[]>(); //list of location of every box in the tail

            tailLoc.Add(headLocTemp);


            //initiiallizes background worker
            bgWorker.DoWork += BgWorker_DoWork;
            bgWorker.ProgressChanged += BgWorker_ProgressChanged;
            bgWorker.RunWorkerCompleted += BgWorker_RunWorkerCompleted;

            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.WorkerReportsProgress = true;

        }

        private void initGameWindow()
        {
            //crates main window
            this.gameWindow = new Window();
            this.gameWindow.Title = "Snake Game"; //names it
            //crate grid that will align the other elements
            this.mainGrid = new Grid();
            this.mainGrid.Height = 500; //height of grid
            this.mainGrid.VerticalAlignment = VerticalAlignment.Center; //align center
            //game canvas
            this.mainGame = new Canvas(); //init
            this.mainGame.Height = 300; //height
            this.mainGame.Width = 800; //width
            this.mainGame.Background = Brushes.Black; //color

            this.tail = new Canvas[100]; //array of canvas's that will be the tail peices

            for (int i = 0; i < 100; i++) //for loop to init each tail peice
            {
                this.tail[i] = new Canvas();
                this.tail[i].Height = 10;
                this.tail[i].Width = 10;
                this.tail[i].Background = Brushes.White; //color, height, width...
                Canvas.SetLeft(this.tail[i], -10); //starting location of each
                Canvas.SetTop(this.tail[i], -10);
                this.mainGame.Children.Add(tail[i]); //adds each tail peice to the main game canvas
            }

            //head of the snake
            this.head = new Canvas();
            this.head.Height = 10;
            this.head.Width = 10; //height, width, color
            this.head.Background = Brushes.Red;
            Canvas.SetLeft(this.head, 100); //loaction
            Canvas.SetTop(this.head, 100);
            this.mainGame.Children.Add(head); //adds to main game
            //sets initial location of food
            Random rnd = new Random();
            this.food = new Canvas();
            this.food.Height = 10;
            this.food.Width = 10;
            this.food.Background = Brushes.Yellow;
            updateFood();
            this.mainGame.Children.Add(food);

            //add game to grid
            this.mainGrid.Children.Add(mainGame);
            //add text box where input will go
            this.inputLine = new TextBox(); //init
            this.inputLine.Width = 200; //height width and locaction
            this.inputLine.Height = 50;
            this.inputLine.Margin = new Thickness(278, 360, 278, 0);
            //key down event for gathering keyboard input
            this.inputLine.KeyDown += nameSpaceKeyDown;

            //add to grid
            this.mainGrid.Children.Add(inputLine);
            //label for instructions
            this.instructions = new Label();
            this.instructions.Content = "Enter name then press enter";
            this.instructions.Margin = new Thickness(450, 450, 278, 0);
            //add to grid
            this.mainGrid.Children.Add(instructions);
            //add grid to window
            this.gameWindow.Content = mainGrid;
        }

        private void startGameEasy(object sender, RoutedEventArgs e)
        {
            this.gameWindow.Show(); //shows game window when button is clicked from main window
            this.speed = 100;
        }
        private void startGameMed(object sender, RoutedEventArgs e)
        {
            this.gameWindow.Show();
            this.speed = 60;
        }
        private void startGameHard(object sender, RoutedEventArgs e)
        {
            this.gameWindow.Show();
            this.speed = 40;
        }

        private void startGameInsane(object sender, RoutedEventArgs e)
        {
            this.gameWindow.Show();
            this.speed = 30;
        }

        private void nameSpaceKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

            if (e.Key == Key.Enter) //once you click enter the game starts running
            {
                
                if (!bgWorker.IsBusy) //changes text and prints the length while the game is running, triggered when enter is pressed
                {
                    this.inputLine.Height = 0;
                    bgWorker.RunWorkerAsync();
                    this.instructions.Content = "Running... Tail Length: " + this.length;
                }
                else
                {
                    this.inputLine.Height = 50;
                    bgWorker.CancelAsync();
                    this.instructions.Content = "Paused...";
                }
            }
            // detects keyboard input and changes the global variable
            if (e.Key == Key.W)
            {
                this.direction = 'w';
            }
            else if (e.Key == Key.S)
            {
                this.direction = 's';
            }
            else if (e.Key == Key.D)
            {
                this.direction = 'd';
            }
            else if (e.Key == Key.A)
            {
                this.direction = 'a';
            }
        }
        private void updateFood() 
        {
            Random rnd = new Random();
            int x = rnd.Next(1, 71) * 11; //getes a random multiple of 11 for the next locatdion of the food
            int y = rnd.Next(1, 25) * 11; //for both x and y values
            foodX[5] = x; // adds to a list of values that correspond to the location of the edges of the food
            foody[5] = y; // used for detecting collision between the head of the snake and the food

            for (int i = 0; i < 5; i++) //populates the list of edge locations starting from the middle and working it's way out
            {
                this.foodX[i] = x - (5 - i); //from index 0 - 5;
                this.foody[i] = y - (5 - i);
            }

            for (int i = 6; i < 11; i++) //from index 6 - 11
            {
                this.foodX[i] = (x - 5) + i;
                this.foody[i] = (y - 5) + i;
            }

            Canvas.SetLeft(this.food, x); //sets location of the food
            Canvas.SetTop(this.food, y);
        }

        private void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.inputLine.Text = "Press enter to resume"; // when the worker is paused sets text 
        }

        private void BgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) //direct updates to canvas happen here
        {
            //game logic code

            //updates the number that represents the length of the tail
            this.instructions.Content = "Length = " + (this.length - 1);


            //---handles direction of snake--
            if (this.direction == 's') 
            {
                Canvas.SetTop(head, Canvas.GetTop(head) + 11); //would of done 10 but this looks cleaner
            }
            else if (this.direction == 'a')
            {
                Canvas.SetLeft(head, Canvas.GetLeft(head) - 11);
            } 
            else if (this.direction == 'w')
            {
                Canvas.SetTop(head, Canvas.GetTop(head) - 11);
            }
            else if (this.direction == 'd')
            {
                Canvas.SetLeft(head, Canvas.GetLeft(head) + 11);
            }
            //updates tail
            double[] temp = { Canvas.GetTop(head), Canvas.GetLeft(head) }; //gets current location of head
            this.tailLoc.Insert(0, temp); //inserts it into the beggining of the list

            for (int i = 0; i < this.length; i++) //updates the tail based on the length of the snake
            {
                Canvas.SetTop(this.tail[i], this.tailLoc[i][0]);
                Canvas.SetLeft(this.tail[i], this.tailLoc[i][1]);
            }

            for (int i = this.length + 1; i < this.tailLoc.Count; i++) //removes excess members of the tail if there are any
            {
                this.tailLoc.RemoveAt(i);
            }

            //detect if collision between snake and food occurs

            for (int i = 0; i < this.foodX.Length; i++) //iterates through the food and head of snake's head location
            {
                if (Canvas.GetLeft(this.head) == this.foodX[i]) //checks
                { 
                    for (int j = 0; j < this.foody.Length; j++) //if true it checks y locations
                    {
                        if (Canvas.GetTop(this.head) == this.foody[j]) //checks
                        {
                            length++; //if true length gets updated
                            updateFood(); //food gets re asigned
                        }
                    }
                }
            }

            //detects if collision between snake and tail occurs

            for (int i = 1; i < this.length; i ++)
            {
                if (Canvas.GetLeft(this.head) == Canvas.GetLeft(this.tail[i]))
                {
                    if (Canvas.GetTop(this.head) == Canvas.GetTop(this.tail[i]))
                    {
                        gameOver();
                    }
                }
            }

            //detecs collision between head and the edge of the map

            if (Canvas.GetLeft(this.head) <= 0)
            {
                gameOver();
            } else if (Canvas.GetLeft(this.head) >= 791)
            {
                gameOver();
            } else if (Canvas.GetTop(this.head) >= 291)
            {
                gameOver();
            } else if (Canvas.GetTop(this.head) <= 0)
            {
                gameOver();
            }
        }
        private void gameOver()
        {
            gameWindow.Hide();
            bgWorker.CancelAsync();
            this.lastScore.Content = "Last Score: " + (this.length - 1);
            this.length = 1;
            this.direction = 'd';
            initGameWindow();
            
        }

        private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int i = 0;
            while (0 == 0) //main game loop
            {
                Console.WriteLine(i);

                bgWorker.ReportProgress(i);

                System.Threading.Thread.Sleep(this.speed);

               

                if (bgWorker.CancellationPending)
                {
                    Console.WriteLine("Thread is exiting....");
                    e.Cancel = true;
                    break;
                }
                i++;
            }
        }

    }
}