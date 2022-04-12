using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

using TelephoneDirectoryF.Models;

namespace TelephoneDirectoryF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public TelephoneDirectoryEntities _context;
        RecordList _records;
        bool _sorted;

        public MainWindow()
        {
            InitializeComponent();
            _context = new TelephoneDirectoryEntities();
            _records = new RecordList(_context.Records.ToList());
            _sorted = false;

            ListBoxRecords.ItemsSource = _records;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (AddRecord(TextBoxName.Text, TextBoxPhoneNumber.Text, TextBoxAddress.Text,
                      TextBoxSchedule.Text, TextBoxField.Text)) return;

            Load();
        }

        public bool AddRecord(string name, string phoneNumber, string address, string schedule, string fieldActivity)
        {
            if (!IsPhoneNumber(phoneNumber))
            {
                MessageBox.Show("Некорректный номер телефона!");
                return false;
            }

            Records newRecord = new Records()
            {
                Name = name,
                PhoneNumber = phoneNumber.TrimStart('+'),
                Address = address,
                Schedule = schedule,
                FieldActivity = fieldActivity
            };

            _context.Records.Add(newRecord);
            _context.SaveChanges();

            return true;
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxRecords.SelectedItem == null)
            {
                MessageBox.Show("Не выбрана запись!");
                return;
            }

            Records selectedRecord = (Records)ListBoxRecords.SelectedItem;

            if (UpdateRecord(ref selectedRecord, TextBoxName.Text, TextBoxPhoneNumber.Text, TextBoxAddress.Text,
                      TextBoxSchedule.Text, TextBoxField.Text)) return;

            _context.SaveChanges();
            Load();
        }

        public bool UpdateRecord(ref Records selectedRecord, string name, string phoneNumber, string address, string schedule, string fieldActivity)
        {
            if (!IsPhoneNumber(phoneNumber))
            {
                MessageBox.Show("Некорректный номер телефона!");
                return false;
            }

            selectedRecord.Name = name;
            selectedRecord.PhoneNumber = phoneNumber.TrimStart('+');
            selectedRecord.Address = address;
            selectedRecord.Schedule = schedule;
            selectedRecord.FieldActivity = fieldActivity;

            return true;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxRecords.SelectedItem == null)
            {
                MessageBox.Show("Не выбрана запись!");
                return;
            }

            Records record = (Records)ListBoxRecords.SelectedItem;

            DeleteRecord(ref record);
            _context.SaveChanges();
            Load();
        }

        public bool DeleteRecord(ref Records record)
        {
            _context.Records.Remove(record);

            return true;
        }

        private void BtnSort_Click(object sender, RoutedEventArgs e)
        {
            if (_sorted)
            {
                BtnSort.Content = "By field";
                Load();
                _sorted = false;
            }
            else
            {
                ListBoxRecords.ItemsSource = _records.SortByFieldAcitivity();
                BtnSort.Content = "By default";
                _sorted = true;
            }
        }

        private void ListBoxRecords_Selected(object sender, RoutedEventArgs e)
        {
            var selectedItem = ListBoxRecords.SelectedItem;
            if (selectedItem is Records)
            {
                Records record = (Records)selectedItem;

                TextBoxName.Text = record.Name;
                TextBoxPhoneNumber.Text = record.PhoneNumber;
                TextBoxAddress.Text = record.Address;
                TextBoxSchedule.Text = record.Schedule;
                TextBoxField.Text = record.FieldActivity;
            }
        }

        private void TextBoxSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ListBoxRecords.ItemsSource = _records.Filter(TextBoxSearch.Text);
        }

        private bool IsPhoneNumber(string phoneNumber)
        {
            phoneNumber = phoneNumber.TrimStart('+');

            if (phoneNumber.Length != 11) return false;

            for (int i =0; i< phoneNumber.Length; i++)
            {
                if (!Char.IsDigit(phoneNumber[i])) return false;
            }

            return true;
        }

        private void Load()
        {
            _records = new RecordList(_context.Records.ToList());
            ListBoxRecords.ItemsSource = _records;
        }
    }

    public class RecordList : IEnumerable<Records>
    {
        public List<Records> records { get; set; }

        public RecordList(List<Records> context)
        {
            records = context;
        }

        public List<Records> SortByFieldAcitivity()
        {
            return records.OrderBy(r => r.FieldActivity).ToList();
        }

        public List<Records> Filter(string str)
        {
            if (str.Length == 0) return records;
            string pattern = $@"{str}*";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return records.Where(r => regex.IsMatch(r.Name) || regex.IsMatch(r.phoneNumber)).ToList();
        }

        public IEnumerator<Records> GetEnumerator() => records.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return records.GetEnumerator();
        }
    }
}
