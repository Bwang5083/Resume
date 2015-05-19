using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Media.Animation;
using System.Timers;
using BookApplication.XAML;

namespace BookApplication
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    ///
    public partial class MainWindow : Window
    {
        public TreeViewDataMV treeviewMV = new TreeViewDataMV();
        public AdvertisementMV advertisementMV = new AdvertisementMV();
        //create new object for database operation
        public DBOperation dbOperation = new DBOperation();
        //generate object out of model view class
        public MVVM objMVVM = new MVVM();

        public MainWindow()
        {
            InitializeComponent();

            //load tool bar
            InitializeToolBar();

            //connect database
            dbOperation.ConnectDB();

            //Read first level tree node from database
            List<TreeNode> firstLevelNode = null;
            dbOperation.ReadTreeViewData("select * from TreeView where ParentID = 0", out firstLevelNode);
            treeviewMV.FirstLevelNode = firstLevelNode;

            //Read second level tree node from database
            for(int i=0; i<firstLevelNode.Count; i++)
            {
                //read second level tree nodes of a particular first tree node
                List<TreeNode> secondLevelNode = null;
                int parentID = firstLevelNode[i].NodeID;
                dbOperation.ReadTreeViewData(string.Format("select * from TreeView where ParentID = {0:d}", parentID), out secondLevelNode);
                //each first level tree node corresponds to a number of second level tree nodes, so use Dictionary to persist
                treeviewMV.secondLevelNode.Add(parentID, secondLevelNode);
            }

            //the initial display of GridView should be LocalShelf table from database
            List<Book> bookList = null;
            dbOperation.ReadGridViewData("select * from LocalShelf", out bookList);
            for (int i = 0; i < bookList.Count; i++)
            {
                //for book description module, each book has a corresponding HTML page
                string url = string.Format("file:///D:/ProjectC%23/BookApplication/BookApplication/HTML/book{0}.html", i + 1);
                //calculate reading progress
                int progress = 15 + i * 4;
                objMVVM.BookCollection.Books.Add(new Book(bookList[i].BookID, bookList[i].BookName, bookList[i].BookType, bookList[i].OrderID, bookList[i].Price, bookList[i].Size, bookList[i].Status, bookList[i].Icon, url, progress));
            };

            //Read advertisement from database
            List<string> Adcontent = null;
            dbOperation.ReadAdvertisement("select Content from Advertisement", out Adcontent);
            for (int i = 0; i < Adcontent.Count; i++)
            {
                objMVVM.AdervertiseCollection.Adervertises.Add(new AdvertisementMV(Adcontent[i]));
            }
            advertisementMV.Advertisement = Adcontent;
            if (advertisementMV.Advertisement.Count > 0)
                advertisementMV.CurrentAdvertisement = advertisementMV.Advertisement[0];

            //initialize treeview data
            treeviewMV.InitializeTreeViewData();

            //change advertisement every 30 seconds
            aTimer = new System.Timers.Timer(30000);
            //OnTimedEvent is the function of timer
            aTimer.Elapsed += OnTimedEvent;
            aTimer.Enabled = true;

            //bind treeview model object and booklist view object with main window's DataContext
            //so the user interface would update as soon as the data change
            DataContext = new
            {
                treeviewMV.DigitalResources,
                treeviewMV.ShoppingMall,
                objMVVM.BookCollection,
                objMVVM.AdervertiseCollection
            };

            //bind advertisement model object with control TextBlock's DataContext
            advertise.DataContext = advertisementMV;
        }

        //initialize tool bar on main window
        private void InitializeToolBar()
        {
            //bind a ToolBarInterface object with each button, Name attribute is not used
            Import.Content = new ToolBarInterface { Name = "Import", ImagePath = "../pictures/import.png" };
            Delete.Content = new ToolBarInterface { Name = "Delete", ImagePath = "../pictures/delete.png" };
            MultiSelect.Content = new ToolBarInterface { Name = "MultiSelect", ImagePath = "../pictures/multichoice.png" };
            button4.Content = new ToolBarInterface { Name = "USA", ImagePath = "../pictures/USA.bmp" };
            ListStyle.Content = new ToolBarInterface { Name = "ListStyle", ImagePath = "../pictures/list.png" };
            BookDescription.Content = new ToolBarInterface { Name = "BookDescription", ImagePath = "../pictures/detail.png" };
            button7.Content = new ToolBarInterface { Name = "paste", ImagePath = "../Images/paste.png" };
        }

        private void OnConnectDB(object sender, ExecutedRoutedEventArgs e)
        {
            //dbOperation.ConnectDB();
            //dbOperation.ExecuteReader("select * from TreeView");
        }

        private void OnAboutDlg(object sender, ExecutedRoutedEventArgs e)
        {
            About dlg = new About();
            dlg.Show();
        }

        private void timer_callback(object state)
        {
            /*
            advertisementMV.CurrentAdvertisement = advertisementMV.Advertisement[advertisementMV.Count];
            advertisementMV.Count++;
            if (advertisementMV.Count == advertisementMV.Advertisement.Count)
                advertisementMV.Count = 0;
            */
        }

        //realize the rolling advertisement on the bottom
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Display advertisement on status bar
            //Timer time = new Timer(timer_callback, null, 1000, 100000);
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = -advertise.ActualWidth; //start from left
            doubleAnimation.To = canMain.ActualWidth; //move to the right
            doubleAnimation.RepeatBehavior = RepeatBehavior.Forever; //endless rolling from left to right
            doubleAnimation.Duration = new Duration(TimeSpan.Parse("0:0:24")); //rolling time, the shorter time, the faster rolling speed
            advertise.BeginAnimation(Canvas.LeftProperty, doubleAnimation); //start animation
        }

        //My Digital Resources tree view selection change
        private void TreeViewItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //clear bookCollection container, then the GridView would be cleared as well
            objMVVM.BookCollection.Books.Clear();

            //identify the corresponding table name of selected tree node
            TreeNode dd = (TreeNode)TreeViewData.SelectedItem;
            string tablename = dd.TableName;

            //read GridView data from database
            List<Book> bookList = null;
            dbOperation.ReadGridViewData("select * from " + tablename, out bookList);

            //update bookCollection container
            for (int i = 0; i < bookList.Count; i++)
            {
                //URL point to a HTML page(BookApplication\HTML) which displayed in book description function
                string url = string.Format("file:///D:/ProjectC%23/BookApplication/BookApplication/HTML/book{0}.html", i + 1);
                //calculate reading progress
                int progress = 15 + i * 4;
                //add all the book objects to book list in MVVM object
                objMVVM.BookCollection.Books.Add(new Book(bookList[i].BookID, bookList[i].BookName, bookList[i].BookType, bookList[i].OrderID, bookList[i].Price, bookList[i].Size, bookList[i].Status, bookList[i].Icon, url, progress));
            };
        }

        //Online Shopping Mall tree view selection change
        private void TreeViewItem_MouseLeftButtonUp1(object sender, MouseButtonEventArgs e)
        {
            TreeNode node = (TreeNode)TreeViewData.SelectedItem;
            if(node.NodeName == "JD Website")
            {
                JDwebsite pwnd = new JDwebsite();
                pwnd.Show();
            }
        }

        //double click book list GridView, open the reading window
        private void GridViewData_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Book book = (Book)GridViewData.SelectedItem;
            Content pwnd = new Content(book.BookID);
            pwnd.Show();
        }

        private void MultiSelect_Click(object sender, RoutedEventArgs e)
        {
            if (30 == selcolumn.Width)
                selcolumn.Width = 0;
            else
                selcolumn.Width = 30;
        }

        //delete the selected books from both database and model
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < GridViewData.Items.Count; i++)
            {
                Book book = (Book)GridViewData.Items[i];
                if(book.Check)
                {
                    string sql = string.Format("delete from LocalShelf where BookID={0}", book.BookID);
                    dbOperation.DeleteBook(sql);
                    objMVVM.BookCollection.Books.RemoveAt(i);
                    i--;
                }
            }
        }

        private void GridViewData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Book book = (Book)GridViewData.SelectedItem;
        }

        private void ListStyle_Click(object sender, RoutedEventArgs e)
        {
            ListData.Width = this.Width;
            DetailView.Width = 0;
            TabView.Width = 0;
        }

        //detailed book list selection change
        private void DetailedBookView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //get the selected book object
            Book book = (Book)DetailedBookView.SelectedItem;
            //get url from book object
            Uri source = new Uri(book.Url, UriKind.Absolute);
            //display HTML file
            BookPage.Source = source;
        }

        //when click description tool bar, show the detailed book list which is hidden initially
        private void BookDescription_Click(object sender, RoutedEventArgs e)
        {
            //show the detailed book list by setting width, the original width is 0
            DetailView.Width = 250;
            //TabView is a frame which is used to display HTML page
            TabView.Width = 900;
            //set the original book list GridView's width to 0 to hide it
            ListData.Width = 0;
        }

        public System.Timers.Timer aTimer { get; set; }
        //loop the advertisement list, start from beginning as soon as reaching the end
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            //change current advertisement periodically
            advertisementMV.CurrentAdvertisement = advertisementMV.Advertisement[advertisementMV.Count];
            advertisementMV.Count++;
            //if get to the last advertisement, then restart from the first advertisement
            if (advertisementMV.Count == advertisementMV.Advertisement.Count)
                advertisementMV.Count = 0;
        }

        private void ListStyle_MouseMove(object sender, MouseEventArgs e)
        {
            ListStyle.Content = new ToolBarInterface { Name = "ListStyle", ImagePath = "../pictures/list1.png" };
        }

        private void ListStyle_MouseLeave(object sender, MouseEventArgs e)
        {
            ListStyle.Content = new ToolBarInterface { Name = "ListStyle", ImagePath = "../pictures/list.png" };
        }

        private void Delete_MouseMove(object sender, MouseEventArgs e)
        {
            Delete.Content = new ToolBarInterface { Name = "Delete", ImagePath = "../pictures/delete1.png" };
        }

        private void Delete_MouseLeave(object sender, MouseEventArgs e)
        {
            Delete.Content = new ToolBarInterface { Name = "Delete", ImagePath = "../pictures/delete.png" };
        }

        //Import tool bar's mouse over picture
        private void Import_MouseMove(object sender, MouseEventArgs e)
        {
            Import.Content = new ToolBarInterface { Name = "Import", ImagePath = "../pictures/import1.png" };
        }

        //Import tool bar's mouse leave picture
        private void Import_MouseLeave(object sender, MouseEventArgs e)
        {
            Import.Content = new ToolBarInterface { Name = "Import", ImagePath = "../pictures/import.png" };
        }

        //"start" button on book list GridView
        private void StartRead_Click(object sender, RoutedEventArgs e)
        {
            Book book = (Book)GridViewData.SelectedItem;
            Content pwnd = new Content(book.BookID);
            pwnd.Show();
        }

        //function of menu "Exit"
        private void OnExit(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void MultiSelect_MouseMove(object sender, MouseEventArgs e)
        {
            MultiSelect.Content = new ToolBarInterface { Name = "MultiSelect", ImagePath = "../pictures/multichoice1.png" };
        }

        private void MultiSelect_MouseLeave(object sender, MouseEventArgs e)
        {
            MultiSelect.Content = new ToolBarInterface { Name = "MultiSelect", ImagePath = "../pictures/multichoice.png" };
        }

        private void BookDescription_MouseMove(object sender, MouseEventArgs e)
        {
            BookDescription.Content = new ToolBarInterface { Name = "BookDescription", ImagePath = "../pictures/detail1.png" };
        }

        private void BookDescription_MouseLeave(object sender, MouseEventArgs e)
        {
            BookDescription.Content = new ToolBarInterface { Name = "BookDescription", ImagePath = "../pictures/detail.png" };
        }
    }
}
