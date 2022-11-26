using GoodsTracker.DataCollector.Common.Configs;
using GoodsTracker.DataCollector.Common.Mappers.Interfaces;
using GoodsTracker.DataCollector.Common.Parsers.Interfaces;
using GoodsTracker.DataCollector.Common.Scrapers.Interfaces;
using GoodsTracker.DataCollector.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace GoodsTracker.DataCollector.Common.Scrapers.Implementaions;

// TODO: Abscract class should work better for scraper methods 
public class EmallScraper : IScraper
{
    private const int contentTimeLoad = 20;
    private const int itemRecourceStartIndex = 16;
    private const string noItemSelectedUriFlag = "#nop";
    private IWebDriver _driver;
    private IItemMapper _mapper;
    private ScraperConfig _config;
    private ILogger<EmallScraper> _logger;
    private IItemParser _parser;
    public EmallScraper(
        ScraperConfig config,
        ILogger<EmallScraper> logger,
        IItemParser parser,
        IItemMapper mapper,
        IWebDriver driver)
    {
        _driver = driver;
        _mapper = mapper;
        _config = config;
        _logger = logger;
        _parser = parser;
        _logger.LogInformation("Emall scraper was created");
    }

    public ScraperConfig GetConfig()
    {
        return _config;
    }

    public Task<IEnumerable<Item>> GetItems()
    {
        var categories = GetCategoryLinksAsync();
        var items = new List<Item>();

        var i = 0;
        foreach (var category in categories)
        {
            i++;
            items.AddRange(ProcessCategoryPage(category.CategoryLink, category.CategoryName));
            if (i > 0) break;
        }

        return Task.FromResult<IEnumerable<Item>>(items);
    }

    private IEnumerable<(string CategoryLink, string CategoryName)> GetCategoryLinksAsync()
    {
        var links = new List<(string CategoryLink, string CategoryName)>();
        _driver.Navigate().GoToUrl(_config.ShopUrl + _config.ShopStartRecource);

        var page = new HtmlDocument();
        page.LoadHtml(_driver.PageSource);

        var rawLinks =
            page
                .DocumentNode
                .SelectNodes("//ul[@class='catalog_menu catalog_menu_visible']/li/a");
        foreach (var raw in rawLinks)
        {
            links.Add((
                raw.Attributes["href"].Value,
                raw.InnerText));
        }

        return links;
    }

    private IEnumerable<Item> ProcessCategoryPage(string categoryLink, string categoryName)
    {
        _driver.Navigate().GoToUrl(categoryLink);


        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(contentTimeLoad));
        var parsedItems = new List<Item>();
        var notLastPage = true;
        do
        {
            // page should be scrolled fully down to trigger loading of all items
            LoadItems();
            var pageUrl = _driver.Url;

            var itemForms = _driver.FindElements(By.XPath("//div[@class='form_wrapper']/form/div[@class='title']/a"));
            var localDom = new HtmlDocument();
            foreach (var itemForm in itemForms)
            {
                _driver.Navigate().GoToUrl(pageUrl + GetItemRecourceFromLink(itemForm.GetAttribute("href")));
                try
                {
                    wait.Until(drv => drv.FindElement(
                    By.XPath("//div[@class='mfp-wrap mfp-close-btn-in mfp-auto-cursor mfp-zoom-in dialog mfp-ready']")));
                }
                catch (WebDriverTimeoutException)
                {
                    _logger.LogWarning($"item didn't appear from the link {_driver.Url}");
                    continue;
                }

                try
                {
                    var itemContent = _driver.FindElement(By.ClassName("mfp-content"));
                    localDom.LoadHtml(itemContent.GetAttribute("innerHTML"));
                }
                catch (NoSuchElementException)
                {
                    continue;
                }
                try
                {
                    var itemFields = _parser.ParseItem(localDom);
                    parsedItems.Add(_mapper.MapItemFields(itemFields));
                }
                catch (InvalidDataException dataException)
                {
                    _logger.LogWarning($"Couldn't parse item page from {categoryLink}: {dataException.Message}");
                }
                catch (IndexOutOfRangeException indexException)
                {
                    _logger.LogWarning($"Regex error: {indexException.Message}");
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex.Message);
                }
            }

            _driver.Navigate().GoToUrl(pageUrl + noItemSelectedUriFlag);
            // trying to fund the button of the next page, if not, exit the process
            try
            {
                _driver
                    .Navigate()
                    .GoToUrl(_driver.FindElement(By.ClassName("next_page_link")).GetAttribute("href"));
            }
            catch (NoSuchElementException)
            {
                notLastPage = false;
            }
        } while (notLastPage);

        return parsedItems;
    }

    private void LoadItems()
    {
        var jsExecutor = (IJavaScriptExecutor)_driver;
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(3));
        while (true)
        {
            try
            {
                var element = _driver.FindElement(By.ClassName("show_more"));
                jsExecutor.ExecuteScript("arguments[0].scrollIntoView(true)", element);
                wait.Until(ExpectedConditions.StalenessOf(element));
            }
            catch (WebDriverException)
            {
                return;
            }
        }
    }

    private string GetItemRecourceFromLink(string elementUri)
    {
        return $"#{elementUri.Substring(itemRecourceStartIndex)}";
    }
}