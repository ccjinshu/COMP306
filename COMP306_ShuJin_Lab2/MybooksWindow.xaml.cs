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
        private List<BookItem> _books; 
        public MybooksWindow(User user)
        {
            InitializeComponent();

            _proxy = new AwsProxy();
            _books = new List<BookItem>();
            _user = user;

            // set labelUsername to the username of the logged in user
            labelUsername.Content = _user.Username;
        }
        private async Task LoadBooks()
        {
            var data = await _proxy.GetBooksByUserId(this._user.UserId);
            BookListView.ItemsSource = data;

             

        }


        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.LoadBooks();
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
            pdfWindow.ShowDialog();
        }
        
    }
}
