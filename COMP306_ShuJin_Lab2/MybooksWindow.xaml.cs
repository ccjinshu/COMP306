using COMP306_ShuJin_Lab2.AWS;
using COMP306_ShuJin_Lab2.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace COMP306_ShuJin_Lab2
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class MybooksWindow : Window
    {
        AwsProxy _proxy ;
        private User _user;
        private List<Book> _books; 
        public MybooksWindow(User user)
        {
            InitializeComponent();
            //窗口居中 
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            _proxy = new AwsProxy();
            _books = new List<Book>();
            _user = user;

            // set labelUsername to the username of the logged in user
            labelUsername.Content = _user.Username;

            // register event handler for window closed event
            this.Closed += MybooksWindow_Closed;
        }

        private void MybooksWindow_Closed(object? sender, EventArgs e)
        {
            // exit the application
            Application.Current.Shutdown();
        }

        private async Task LoadBooks()
        {
            var data = await _proxy.GetBooksByUserId(this._user.UserId);
            BookListView.ItemsSource = data;

             

        }


        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
             LoadBooks();
        }
        private void BookListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (BookListView.SelectedItem is Book selectedBook)
            {
                OpenPdfWindow(selectedBook);
            }
        }

        private void OpenPdfWindow( Book book)
        { 
            var  pdfWindow = new PdfWindow(_user, book);
            //pdfWindow.LoadPdf();
            this.Hide();
            pdfWindow.ShowDialog();
            //refresh the book list
            LoadBooks();
            this.Show();

        }
        
    }
}
