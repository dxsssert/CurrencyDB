using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.Configuration;
using CurrencyDB.Models;
using CurrencyDB.Repositories;

namespace CurrencyDB.Desktop;

public partial class MainWindow : Window
{
    private readonly BankRepository _bankRepo;
    private readonly CurrencyRepository _currencyRepo;
    private readonly QuoteRepository _quoteRepo;
    private readonly ReportRepository _reportRepo;

    public MainWindow()
    {
        InitializeComponent();
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var context = new DapperContext(configuration);

        _bankRepo = new BankRepository(context);
        _currencyRepo = new CurrencyRepository(context);
        _quoteRepo = new QuoteRepository(context);
        _reportRepo = new ReportRepository(context);

        Loaded += async (s, e) => await RefreshAllData();
    }

    private async Task RefreshAllData()
    {
        try
        {
            StatusText.Text = "Обновление данных...";
            var banks = (await _bankRepo.GetAllBanksAsync()).ToList();
            var currencies = (await _currencyRepo.GetAllCurrencyAsync()).ToList();
            var quotes = (await _quoteRepo.GetAllQuotesAsync()).ToList();

            BanksGrid.ItemsSource = banks;
            CurrenciesGrid.ItemsSource = currencies;
            QuotesGrid.ItemsSource = quotes;

            CmbQuoteBank.ItemsSource = banks;
            CmbQuoteCurrency.ItemsSource = currencies;

            StatusText.Text = $"Последнее обновление: {DateTime.Now:HH:mm:ss}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            StatusText.Text = "Ошибка при обновлении.";
        }
    }

    private void OnlyNumbers_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !char.IsDigit(e.Text, 0);
    }

    private async void RefreshBtn_Click(object sender, RoutedEventArgs e) => await RefreshAllData();

    private async void CreateCurrency_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtCurrencyName.Text)) return;

        var currency = new Currency
        {
            CurrencyName = TxtCurrencyName.Text,
            AlphaCode = TxtAlphaCode.Text,
            DigitalCode = int.TryParse(TxtDigitalCode.Text, out int dc) ? dc : 0,
            IsCrypto = ChkIsCrypto.IsChecked ?? false
        };

        await _currencyRepo.CreateCurrencyAsync(currency);


        TxtCurrencyName.Clear();
        TxtAlphaCode.Clear();
        TxtDigitalCode.Clear();
        ChkIsCrypto.IsChecked = false;

        await RefreshAllData();
    }

    private void CurrenciesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

        if (CurrenciesGrid.SelectedItem is Currency selected)
        {
            TxtCurrencyName.Text = selected.CurrencyName!;
            TxtAlphaCode.Text = selected.AlphaCode!;
            TxtDigitalCode.Text = selected.DigitalCode.ToString();
            ChkIsCrypto.IsChecked = selected.IsCrypto;
            StatusText.Text = $"Выбрана валюта: {selected.CurrencyName} (ID: {selected.CurrencyId})";
        }
    }

    private async void UpdateCurrency_Click(object sender, RoutedEventArgs e)
    {
        if (CurrenciesGrid.SelectedItem is Currency selected)
        {
            try
            {
                selected.CurrencyName = TxtCurrencyName.Text;
                selected.AlphaCode = TxtAlphaCode.Text;
                selected.DigitalCode = int.TryParse(TxtDigitalCode.Text, out int dc) ? dc : 0;
                selected.IsCrypto = ChkIsCrypto.IsChecked ?? false;

                await _currencyRepo.UpdateCurrencyAsync(selected);
                await RefreshAllData();

                StatusText.Text = "Данные успешно обновлены";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления: {ex.Message}");
            }
        }
        else
        {
            MessageBox.Show("Сначала выберите валюту в таблице!");
        }
    }

    private async void DeleteCurrency_Click(object sender, RoutedEventArgs e)
    {
        if (CurrenciesGrid.SelectedItem is Currency selected)
        {
            var confirm = MessageBox.Show($"Вы уверены, что хотите удалить {selected.CurrencyName}?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (confirm == MessageBoxResult.Yes)
            {
                await _currencyRepo.DeleteCurrencyAsync(selected.CurrencyId);
                await RefreshAllData();
                StatusText.Text = "Запись удалена";
            }
        }
    }

    private void BanksGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (BanksGrid.SelectedItem == null) return;
        if (BanksGrid.SelectedItem is Bank selected)
        {
            TxtBankName.Text = selected.BankName!;
            TxtBankCode.Text = selected.BankCode!;
            ChkBankActive.IsChecked = selected.IsActive;


            foreach (ComboBoxItem item in CmbBankType.Items)
            {
                if (item.Content.ToString()?.ToLower() == selected.BankType?.ToLower())
                {
                    CmbBankType.SelectedItem = item;
                    break;
                }
            }

            StatusText.Text = $"Выбран банк: {selected.BankName}";
        }
    }

    private async void CreateBank_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtBankName.Text)) return;

        var selectedContent = (CmbBankType.SelectedItem as ComboBoxItem)?.Content.ToString();

        var bank = new Bank
        {
            BankName = TxtBankName.Text,
            BankCode = TxtBankCode.Text,
            BankType = selectedContent?.ToLower(),
            IsActive = ChkBankActive.IsChecked ?? true
        };

        await _bankRepo.CreateBankAsync(bank);


        TxtBankName.Clear();
        TxtBankCode.Clear();
        await RefreshAllData();
    }

    private async void UpdateBank_Click(object sender, RoutedEventArgs e)
    {

        if (BanksGrid.SelectedItem is Bank selected)
        {
            try
            {

                selected.BankName = TxtBankName.Text;
                selected.BankCode = TxtBankCode.Text;
                selected.BankType = (CmbBankType.SelectedItem as ComboBoxItem)?.Content.ToString()?.ToLower();
                selected.IsActive = ChkBankActive.IsChecked ?? true;

                await _bankRepo.UpdateBankAsync(selected);
                await RefreshAllData();

                StatusText.Text = $"Банк '{selected.BankName}' успешно обновлен";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении банка: {ex.Message}", "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        else
        {

            MessageBox.Show("Сначала выберите банк в таблице!",
                "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }


    private async void DeleteBank_Click(object sender, RoutedEventArgs e)
    {
        if (BanksGrid.SelectedItem is Bank selected)
        {
            var result = MessageBox.Show($"Удалить {selected.BankName}?", "Внимание",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                await _bankRepo.DeleteBankAsync(selected.BankId);
                await RefreshAllData();
            }
        }
    }

    private void QuotesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (QuotesGrid.SelectedItem == null) return;

        if (QuotesGrid.SelectedItem is Quote selected)
        {

            CmbQuoteBank.SelectedValue = selected.BankId;
            CmbQuoteCurrency.SelectedValue = selected.CurrencyId;


            TxtRateBuy.Text = selected.RateBuy.ToString();
            TxtRateSell.Text = selected.RateSell.ToString();

            StatusText.Text = $"Выбрана котировка ID: {selected.QuoteId}";
        }
    }

    private async void CreateQuote_Click(object sender, RoutedEventArgs e)
    {
        if (CmbQuoteBank.SelectedValue == null || CmbQuoteCurrency.SelectedValue == null)
        {
            MessageBox.Show("Выберите банк и валюту!");
            return;
        }

        try
        {
            var quote = new Quote
            {
                BankId = (int)CmbQuoteBank.SelectedValue,
                CurrencyId = (int)CmbQuoteCurrency.SelectedValue,
                RateBuy = decimal.TryParse(TxtRateBuy.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out decimal rb)
                    ? rb
                    : 0,
                RateSell = decimal.TryParse(TxtRateSell.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out decimal rs)
                    ? rs
                    : 0,
                QuoteDate = DateTime.Now
            };

            await _quoteRepo.CreateQuoteAsync(quote);
            await RefreshAllData();

            TxtRateBuy.Clear();
            TxtRateSell.Clear();
            StatusText.Text = "Запись успешно добавлена";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}");
        }
    }

    private async void UpdateQuote_Click(object sender, RoutedEventArgs e)
    {
        if (QuotesGrid.SelectedItem is Quote selected)
        {
            try
            {
                selected.RateBuy = decimal.TryParse(TxtRateBuy.Text.Replace(',', '.'),
                    System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture,
                    out decimal rb)
                    ? rb
                    : 0;
                selected.RateSell = decimal.TryParse(TxtRateSell.Text.Replace(',', '.'),
                    System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture,
                    out decimal rs)
                    ? rs
                    : 0;

                selected.CurrencyId = (int)CmbQuoteCurrency.SelectedValue;
                selected.BankId = (int)CmbQuoteBank.SelectedValue;

                await _quoteRepo.UpdateQuoteAsync(selected);
                await RefreshAllData();

                StatusText.Text = $"Запись {selected.QuoteId} успешно обновлена";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления котировки: {ex.Message}");
            }
        }
        else
        {
            MessageBox.Show("Пожалуйста, выберите запись в таблице!");
        }
    }

    private async void DeleteQuote_Click(object sender, RoutedEventArgs e)
    {
        if (QuotesGrid.SelectedItem is Quote selectedQuote)
        {
            await _quoteRepo.DeleteQuoteAsync(selectedQuote.QuoteId);
            await RefreshAllData();
        }
    }
    
    private async void GetCurrentRate_Click(object sender, RoutedEventArgs e)
    {
        var code = TxtReportCurrency.Text.ToUpper();
        var data = await _reportRepo.GetCurrentRateAsync(code);
       
        ReportsGrid.ItemsSource = data.ToList();
        TxtReportTitle.Text = $"Актуальные курсы {code} на сегодня";
    }


    private async void GetSpreadReport_Click(object sender, RoutedEventArgs e)
    {
        var code = TxtReportCurrency.Text.ToUpper();
        var date = DateReport.SelectedDate ?? DateTime.Today;
        var data = await _reportRepo.GetSpreadReportAsync(code, date);
        
        ReportsGrid.ItemsSource = data.ToList();
        TxtReportTitle.Text = $"Анализ спредов {code} за {date:dd.MM.yyyy}";
    }


    private async void GetDynamics_Click(object sender, RoutedEventArgs e)
    {
        var code = TxtReportCurrency.Text.ToUpper();
        var end = DateReport.SelectedDate ?? DateTime.Today;
        var start = end.AddDays(-30);
        var data = await _reportRepo.GetDynamicsAsync(code, start, end);
        
        ReportsGrid.ItemsSource = data.ToList();
        TxtReportTitle.Text = $"Динамика {code} (последние 30 дней)";
    }


    private async void GetCrossRate_Click(object sender, RoutedEventArgs e)
    {
        var baseC = TxtBaseCurr.Text.ToUpper();
        var targetC = TxtTargetCurr.Text.ToUpper();
        var date = DateReport.SelectedDate ?? DateTime.Today;
    
        var result = await _reportRepo.GetCrossRateAsync(baseC, targetC, date);
        
        TxtCrossRateResult.Text = result.HasValue ? result.Value.ToString("N4") : "Н/Д";
    }
}